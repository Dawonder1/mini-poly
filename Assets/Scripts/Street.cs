using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Street : Property
{
    [SerializeField] public int buildCost;
    [SerializeField] public int colorGroup;
    public int numHouses; //5 Houses = 1 hotel
    [SerializeField] int[] rents;
    override public int rent {  get { return rents[numHouses]; } }

    protected override void Start()
    {
        base.Start();
        numHouses = 0;
    }
    public void addHouse()
    {
        if(owner.cash >= buildCost)
        {
            if (numHouses < 5)
            {
                numHouses++;
                owner.cash -= buildCost;
                Debug.Log($"{owner._name} has bought a house on {this._name}");
            }
        }
    }

    public bool sellHouses(int amount)
    {
        Property[] monopoly = counterparts;
        monopoly.Append(this);
        while(numHouses > 0)
        {
            foreach (Street street in monopoly)
            {
                if(street.numHouses > 0)
                {
                    numHouses--;
                    street.owner.cash += buildCost / 2;
                    rent -= buildCost / 2;
                    if (street.owner.cash >= amount) return true;
                    Debug.Log($"{street.owner._name} has sold a house on {street._name}");
                }
            }
        }
        return false;
    }
}