using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] Merchant[] merchants;
    public Merchant merchantInTurn;
    public int value {  get; private set; }
    private int counter = 0;
    public static Dice dice { get; set; }
    private void Start()
    {
        if (dice == null) dice = this;
        else Destroy(this);
        counter = Random.Range(0, merchants.Length);
        merchantInTurn = merchants[counter];
    }
    public void roll()
    {
        int die1 = Random.Range(1, 6);
        int die2 = Random.Range(1, 6);
        value = die1 + die2;
    }

    private void OnMouseDown()
    {
        //roll dice
        counter = ++counter % 4;
        value = Random.Range(2, 12);
        Debug.Log($"{merchantInTurn._name} rolled {value}");
        merchantInTurn.move(value);
        merchantInTurn = merchants[counter];
    }
}