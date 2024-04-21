using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class Merchant : MonoBehaviour
{
    public string _name;
    int currentPos = 0;
    public int cash { get; set; }
    private List<Property> properties = new List<Property>();

    private void Start()
    {
        cash = 1500;
    }
    public void move(int diceValue)
    {
        currentPos = (currentPos + diceValue) % 40;
        transform.position = Board.board.spaces[currentPos].transform.position;
        Property space;
        Board.board.spaces[currentPos].TryGetComponent(out space);
        if (space != null) { space.host(this); }
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
        foreach(Property property in properties)
        {
            property.mortgage();
            if (cash >= amount)
            {
                cash -= amount;
                return true;
            }
        }
        foreach (Property property in properties) property.isMortgaged = false;
        return false;
    }

    public void buy(Property property)
    {
        property.sell(this);
        properties.Add(property);
        Debug.Log($"{this._name} has bought {property._name}");
    }

    public bool chooseToBuy(int value, int price)
    {
        if (price > value)
        {
            if (Random.Range(0.1f, 1f) <= 0.1f) return true;
            return false;
        }
        return true;
    }
}