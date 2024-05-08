using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Merchant : MonoBehaviour
{
    [SerializeField] public string _name;
    int currentPos = 0;
    int previousPos = 0;
    public bool inJail = false;
    public int numJailFree = 0;
    public int cash { get; set; }
    public List<Property> properties = new List<Property>();
    public List<Property[]> monopolies = new List<Property[]>();
    public List<Property> propertiesToRequest = new List<Property>();

    private void Start()
    {
        cash = 1500;
    }
    public void move(int diceValue)
    {
        if (inJail && numJailFree > 0) numJailFree--;
        else if (numJailFree == 0)
        {
            cash -= 50;
            inJail = false;
        }
        currentPos = (currentPos + diceValue) % 40;
        //receive salary after passing go
        if (currentPos < previousPos)
        {
            cash += 200;
            //choose where to build a house
            foreach (Property[] monopoly in monopolies)
            {
                if (!BuyHouses(monopoly)) break; 
            }
            //unmortgage mortgaged properties
            foreach (Property property in properties)
            {
                if (property.isMortgaged)
                {
                    if (!property.unmortgage()) break;
                }
            }
            //choose property to request
            //request properties that complete monopolies
            //and sell those that don't.
            foreach(Property property in properties)
            {
                Property[] mayRequest = new Property[0];
                bool shouldRequest = false;
                foreach (Property counterpart in property.counterparts)
                {
                    if (counterpart.owner == property.owner)
                    {
                        shouldRequest = true;
                    }
                    else mayRequest.Append(counterpart);
                }
                if (shouldRequest && mayRequest.Length > 0)
                {
                    foreach(Property request in mayRequest) propertiesToRequest.Append(request);
                }
                else if (!shouldRequest)
                {
                    if(chooseToSell(property)) property.auction(this, true);
                }
            }
            if ( propertiesToRequest.Count > 0) request(propertiesToRequest.First());
        }
        transform.position = Board.board.spaces[currentPos].transform.position;
        Property space; SpecialSpaces special;
        if (Board.board.spaces[currentPos].TryGetComponent(out space)) { if (space != null) space.host(this); }
        else if (Board.board.spaces[currentPos].TryGetComponent(out special)) { if (special != null) special.host(this); }

    }
    public bool pay(int amount)
    {
        //pay if merchant has enough cash
        if(cash >= amount)
        {
            cash -= amount;
            return true;
        }
        //mortgage properties if merchant does not have enough cash
        List<Property> previouslyMortgaged = new List<Property>();
        int culMortgageValue = 0;
        foreach(Property property in properties)
        {
            if (property.isMortgaged)
            {
                previouslyMortgaged.Add(property);
                continue;
            }
            if(property is not Street || (property is Street && ((Street)property).numHouses == 0))
            {
                culMortgageValue += property.mortgage();
                if (cash >= amount)
                {
                    cash -= amount;
                    return true;
                }
            }
            else
            {
                if (((Street)property).sellHouses(amount)) return true;
            }
        }
        foreach (Property property in properties)
        {
            if (previouslyMortgaged.Contains(property)) continue;
            property.isMortgaged = false;
        }
        cash -= culMortgageValue;
        return false;
    }
    public void buy(Property property)
    {
        property.sell(this);
        properties.Add(property);
        Debug.Log($"{this._name} has bought {property._name}");
        //if merchant has another property in the monopoly
        //request for the last piece.
        bool shouldRequest = false;
        bool monopoly = true;
        List<Property> prop = new List<Property>();
        for (int i = 0; i < property.counterparts.Length; i++)
        {
            if (properties.Contains(property.counterparts[i])) shouldRequest = true;
            else
            {
                prop.Add(property.counterparts[i]);
                monopoly = false;
            }
        }
        if (!monopoly && shouldRequest) request(prop.First());
        if (monopoly)
        {
            Property[] array = property.counterparts;
            array = array.Append(property).ToArray();
            monopolies.Add(array);
        }
    }
    public bool chooseToBuy(Property property, int price)
    {
        int rentRank = 0; int value = 0;
        rentRank = property is Street ? (Array.IndexOf(Board.board.spaces, property) % 10) + 1 : 2;
        int developmentalValue = property is Street ? ((Street)property).buildCost / 2 : property.price / 4;
        if (cash < price) developmentalValue *= -1;
        value = property.price + (rentRank * property.rent) + developmentalValue;
        Debug.Log($"{_name} is ready to buy {property._name} at ${value}");
        return value >= price;
    }
    public void request(Property property)
    {
        if (property.owner == null) return;
        if (property.owner.chooseToSell(property))
        {
            int price = property.owner.setPrice(property);
            if (chooseToBuy(property, price))
            {
                //if capable of paying, buy property
                if (pay(price)) buy(property);
            }
        }
    }
    public bool chooseToSell(Property property)
    {
        int index = 0;
        //if closer to a monopoly be reluctant
        foreach(Property counterpart in property.counterparts)
        {
            if (counterpart.owner == this) index--;
        }
        //if not a street be eager
        if (property is not Street) index++;
        //rank rent
        index += Array.IndexOf(Board.board.spaces, property) / 10 + 1;
        //be eager to sell mortgaged properties
        if (property.isMortgaged) index++;
        if (index > 0) return true;
        return false;
    }
    public int setPrice(Property property)
    {
        //index here is a price index not a value index
        int index = 0, value = 0;
        index += 4 - property.counterparts.Length;
        //increase the price index if the merchant is closer to a monopoly
        Property part = property;
        foreach (Property counterpart in property.counterparts)
        {
            if (counterpart.owner || part.owner) index++;
            part = counterpart;
        }
        value = index * property.rent;
        if (property.isMortgaged) value += property.price; else value += property.price / 2;
        Debug.Log($"{this} has evaluated {property._name} at {value}");
        return value;
    }
    public bool BuyHouses(Property[] monopoly)
    {
        //returns true if merchant can continue buying houses
        if (monopoly is not Street[]) return true;
        foreach (Street street in monopoly)
        {
            if (cash >= street.buildCost) { street.addHouse(); Debug.Log($"{_name} has bought a house on {street._name}"); }
            else return false;
        }
        
        return true;
    }
    public void goToJail()
    {
        currentPos = 10;
        transform.position = Board.board.spaces[currentPos].transform.position;
        inJail = true;
    }
    public void goBack3Spaces()
    {
        currentPos -= 3;
        transform.position = Board.board.spaces[currentPos].transform.position;
        Property property = null; SpecialSpaces special = null;
        if(Board.board.spaces[currentPos].TryGetComponent<Property>(out property)) property.host(this);
        else if(Board.board.spaces[currentPos].TryGetComponent<SpecialSpaces>(out special)) special.host(this);
    }
    public void goBackBellivie()
    {
        currentPos = 1;
        transform.position = Board.board.spaces[currentPos].transform.position;
        Board.board.spaces[currentPos].GetComponent<Property>().host(this);
    }
    public void updateMonopolies()
    {
        foreach(Property property in properties)
        {
            bool monopoly = true;
            Property[] newMonopoly = new Property[property.counterparts.Length + 1];
            newMonopoly[0] = property; int i = 1;
            foreach(Property part in property.counterparts)
            {
                if(!property.counterparts.Contains(part)) monopoly = false;
                newMonopoly[i++] = part;
            }
            if (monopoly && !monopolies.Contains(newMonopoly)) monopolies.Add(newMonopoly);
        }
    }
    public int setAuctionPrice(Property property)
    {
        int value = property.value;
        int index = 0;
        //add value per closeness to monopoly
        foreach(Property part in property.counterparts)
        {
            if (part.owner == this) index += 2;
        }
        //devalue property per poverty
        foreach(Property propterty in properties)
        {
            if (propterty.isMortgaged) index--;
        }
        //add value per liquidity
        index += cash / 100;
        value = property.price + (index * property.rent);
        Debug.Log($"{_name} has bidded {value} for {property._name}");
        return value;
    }
}