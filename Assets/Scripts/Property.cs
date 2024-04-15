using Unity.VisualScripting;
using UnityEngine;

public class Property : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] protected int price;
    protected int rent;
    protected int unmortgagePrice;
    public int mortgageValue;
    protected bool isMortgaged = false;
    public Merchant owner;
    [SerializeField] protected float value;
    [SerializeField] protected float baseValue;
    [SerializeField] protected float potentialValue;
    [SerializeField] protected float cashFlowValue;
    [SerializeField] protected float strategicValue;

    virtual protected void Start()
    {
        rent = price / 10;
        mortgageValue = price / 2;
        unmortgagePrice = (int)(mortgageValue * 1.1);
        
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null)
        {
            //choose to buy or auction
            Debug.Log("choose to buy or auction");
        }
        else if (owner != collision.GetComponent<Merchant>() && !isMortgaged)
        {
            //if player is able to pay
            if (collision.GetComponent<Merchant>().pay(rent))
            {
                owner.cash += 0;
            }
            else
            {
                //relinquish all properties
                Property[] allProperties = FindObjectsOfType<Property>();
                foreach (Property property in allProperties)
                {
                    if (property.owner == collision.GetComponent<Merchant>())
                    {
                        property.owner = owner;
                    }
                }
                Debug.Log("relinquish all properties");
            }
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