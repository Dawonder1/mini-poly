using UnityEngine;

public class Utility : Property
{
    protected override void getRent(Merchant merchant)
    {
        //Determine the rent whether the owner has a monopoly or not.
        if (counterparts[0].owner == this.owner) rent = Dice.dice.value * 10;
        else rent = Dice.dice.value * 4;
        base.getRent(merchant);
    }
}