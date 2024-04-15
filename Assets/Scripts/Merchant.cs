using UnityEditor.Build.Content;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public string _name;
    int currentPos = 0;
    public int cash { get; set; }
    public void move(int diceValue)
    {
        currentPos = (currentPos + diceValue) % 40;
        transform.position = Board.board.spaces[currentPos].transform.position;
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
        foreach(Property property in FindObjectsOfType<Property>())
        {
            if(property.owner == this)
            {
                property.mortgage();
                if(cash >= amount)
                {
                    cash -= amount;
                    return true;
                }
            }
        }
        return false;
    }

    public void buy(Property property)
    {
        property.sell(this);
    }
}