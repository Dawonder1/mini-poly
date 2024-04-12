using UnityEngine;

class Utility : Property
{
    override protected void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (owner == null)
        {
            //choose to buy or auction
            Debug.Log("choose to buy or auction");
        }
        else if (owner != collision.GetComponent<Merchant>())
        {
            //Determine the rent whether the owner has a monopoly or not.
            Utility[] utilities = FindObjectsOfType<Utility>();
            for(int i = 0; i < utilities.Length; i++)
            {
                if(utilities[i].owner != owner)
                {
                    //rent = Dice.value * 4;
                    break;
                }
                //rent = Dice.value * 10;
            }

            //if player is able to pay
            if(collision.GetComponent<Merchant>().pay(rent))
            {
                owner.cash += 0;
            }
            else
            {
                //relinquish all properties
                Property[] allProperties = FindObjectsOfType<Property>();
                foreach(Property property in allProperties)
                {
                    if(property.owner == collision.GetComponent<Merchant>())
                    {
                        property.owner = owner;
                    }
                }
                Debug.Log("relinquish all properties");
            }
        }

    }
}