using UnityEngine;
class Station : Property
{
    virtual protected void OnTrigerEnter2D(Collider2D collision)
    {
        if (owner == null)
        {
            //choose to buy or auction
            Debug.Log("choose to buy or auction");
        }
        else if (owner != collision.GetComponent<Merchant>())
        {
            //calculate rent
            int numStations = 0;
            foreach (Station station in FindObjectsOfType<Station>())
            {
                if (station.owner == owner) numStations++;
                rent = 25 * (int)Mathf.Pow(2, numStations);
            }
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
}