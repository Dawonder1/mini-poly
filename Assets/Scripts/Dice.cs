using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] List<Merchant> merchants;
    public Merchant merchantInTurn;
    public int value {  get; private set; }
    public bool gameOver = false;
    private int counter = 0;
    public static Dice dice { get; set; }
    private void Start()
    {
        if (dice == null) dice = this;
        else Destroy(this);
        counter = Random.Range(0, merchants.Count);
        merchantInTurn = merchants[counter];
    }
    private void OnMouseDown()
    {
        //roll dice
        if (gameOver) return;
        counter = ++counter % merchants.Count;
        value = Random.Range(2, 12);
        Debug.Log($"{merchantInTurn._name} rolled {value}");
        merchantInTurn.move(value);
        merchantInTurn = merchants[counter];
    }
    public void removeMerchant(Merchant merchant)
    {
        merchants.Remove(merchant);
        if (counter > 0) merchantInTurn = merchants[counter - 1];
        else merchantInTurn = merchants[merchants.Count - 1];
        if (merchants.Count == 1) gameOver = true;
    }
}