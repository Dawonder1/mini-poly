using Unity.VisualScripting;
using UnityEngine;

public class Property : MonoBehaviour
{
    [SerializeField] public string _name;
    [SerializeField] protected int price;
    protected int rent;
    protected int unmortgagePrice;
    public int mortgageValue;
    public bool isMortgaged = false;
    public Merchant owner;
    public int value;

    virtual protected void Start()
    {
        rent = price / 10;
        mortgageValue = price / 2;
        unmortgagePrice = (int)(mortgageValue * 1.1);
    }

    public void host(Merchant merchant)
    {
        if (owner == null)
        {
            //choose to buy or auction
            Debug.Log("choose to buy or auction");
            if (merchant.chooseToBuy(value, price))
            {
                if (merchant.pay(price)) merchant.buy(this);
            }
            else
            {
                int startingPrice = price;
                //auction
            }
        }
        else if (owner != merchant && !isMortgaged)
        {
            getRent(merchant);
        }
    }

    virtual protected void getRent(Merchant merchant)
    {
        if (merchant.GetComponent<Merchant>().pay(rent))
        {
            owner.cash += 0;
            Debug.Log($"{merchant.GetComponent<Merchant>()._name} has paid ${rent} to {owner._name}");
        }
        else
        {
            //relinquish all properties
            Property[] allProperties = FindObjectsOfType<Property>();
            foreach (Property property in allProperties)
            {
                if (property.owner == merchant.GetComponent<Merchant>())
                {
                    property.owner = owner;
                }
            }
            Debug.Log("relinquish all properties");
        }
    }

    public void unmortgage()
    {
        owner.cash -= unmortgagePrice;
        unmortgagePrice = 0;
        isMortgaged = false;
    }

    virtual public void mortgage()
    {
        int mortgageValue = price / 2;
        unmortgagePrice = mortgageValue + (mortgageValue / 10);
        owner.cash += mortgageValue;
        isMortgaged = true;
    }

    public void sell(Merchant merchant)
    {
        owner = merchant;
    }
}