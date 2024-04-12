using UnityEngine;

class Merchant : MonoBehaviour
{
    string _name;
    public int cash { get; set; }
    public bool pay(int amount)
    {
        return true;
    }

    public void buy(Property property)
    {
        property.sell(this);
    }
}