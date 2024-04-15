using UnityEngine;

public class Street : Property
{
    [SerializeField] Color colorGroup;
    int buildCost;
    int numHouses; //5 Houses = 1 hotel
    public void addHouse()
    {
        if(owner.cash > buildCost)
        {
            if (numHouses < 5)
            {
                numHouses++;
                rent += buildCost / 2; //to be reviewed
                owner.cash -= buildCost;
            }

        }
    }

    override public void mortgage()
    {
        int mortgageValue = (price + numHouses * buildCost) / 2;
        unmortgagePrice = mortgageValue + (mortgageValue / 10);
        owner.cash += mortgageValue;
        isMortgaged = true;
    }
}