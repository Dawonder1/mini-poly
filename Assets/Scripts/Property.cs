using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Property : MonoBehaviour, IHost
{
    public Property[] counterparts;
    public bool isMortgaged = false;
    public int mortgageValue;
    [SerializeField] public string _name;
    public Merchant owner;
    [SerializeField] public int price;
    virtual public int rent {  get; protected set; }
    protected int unmortgagePrice;
    public int value;
    
    virtual protected void Start()
    {
        mortgageValue = price / 2;
        unmortgagePrice = (int)(mortgageValue * 1.1);
    }

    public void host(Merchant merchant)
    {
        if (owner == null)
        {
            //choose to buy or auction
            Debug.Log("choose to buy or auction");
            if (merchant.chooseToBuy(this, price))
            {
                if (merchant.pay(price)) merchant.buy(this);
            }
            else
            {
                auction(merchant, false);
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
            owner.cash += rent;
            Debug.Log($"{merchant.GetComponent<Merchant>()._name} has paid ${rent} to {owner._name}");
        }
        else
        {
            //relinquish all properties
            List<Property> allProperties = merchant.properties;
            foreach (Property property in merchant.properties)
            {
                property.owner = owner;
            }
            owner.cash += merchant.cash;
            Debug.Log($"{merchant._name} is bankrupt. {owner._name} has taken over.");
            owner.updateMonopolies();
            Dice.dice.removeMerchant(merchant);
            Destroy(merchant.gameObject);
        }
    }

    public bool unmortgage()
    {
        if (owner.cash >= unmortgagePrice)
        {
            owner.cash -= unmortgagePrice;
            unmortgagePrice = 0;
            isMortgaged = false;
            Debug.Log($"{owner._name} has unmortgaged {this._name}");
            return true;
        }
        else return false;
    }

    virtual public int mortgage()
    {
        owner.cash += mortgageValue;
        isMortgaged = true;
        Debug.Log($"{owner._name} has mortgaged {this._name}");
        return mortgageValue;
    }

    public void sell(Merchant merchant)
    {
        owner = merchant;
    }

    public void auction(Merchant seller, bool hasSeller)
    {
        int maxPrice = int.MinValue;
        Merchant maxBidder = null;
        foreach(Merchant merchant in FindObjectsOfType<Merchant>())
        {
            int price = merchant == seller ? 0 : merchant.setAuctionPrice(this);
            if (price > maxPrice)
            {
                maxPrice = price;
                maxBidder = merchant;
            }
        }
        //highest bidder pays
        if(maxPrice > price && maxBidder.pay(maxPrice))
        {
            if (hasSeller) seller.cash += maxPrice;
            maxBidder.buy(this);
            return;
        }
        //if bidder cannot pay
        Debug.Log($"{maxBidder._name} could not pay his bid of {maxPrice}.");
        Merchant[] excluded = {seller, maxBidder};
        if (hasSeller) seller.cash += auction(excluded);
    }

    public int auction(Merchant[] excluded)
    {
        int maxPrice = int.MinValue;
        Merchant maxBidder = null;
        foreach (Merchant merchant in FindObjectsOfType<Merchant>())
        {
            int price = excluded.Contains(merchant) ? 0 : merchant.setAuctionPrice(this);
            if (price > maxPrice)
            {
                maxPrice = price;
                maxBidder = merchant;
            }
        }
        if (maxPrice < price) return 0;
        if (maxBidder.pay(maxPrice))
        {
            maxBidder.buy(this);
            return maxPrice;
        }
        Debug.Log($"{maxBidder._name} could not pay his bid of {maxPrice}.");
        return auction(excluded.Append(maxBidder).ToArray());
    }
}