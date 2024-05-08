using UnityEngine;
public class Station : Property
{

    override protected void getRent(Merchant merchant)
    {
        //calculate rent
        int numStations = 0;
        foreach (Station station in counterparts)
        {
            if (station.owner == owner) numStations++;
            rent = 25 * (int)Mathf.Pow(2, numStations);
        }
        base.getRent(merchant);
    }
}