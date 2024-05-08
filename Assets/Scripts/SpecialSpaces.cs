using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSpaces : MonoBehaviour, IHost
{
    [SerializeField] SpaceType type;
    [SerializeField] int tax;

    enum SpaceType
    {
        GoToJail,
        Tax,
        CommunityChest,
        Chance
    }
    public void host(Merchant merchant)
    {
        if (type == SpaceType.GoToJail) merchant.goToJail();

        if (type == SpaceType.Tax)
        {
            merchant.pay(tax);
            Debug.Log($"{merchant._name} has paid ${tax} tax");
        }
        if (type == SpaceType.CommunityChest) { }
        if (type == SpaceType.Chance) { }
    }
}
