//using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board board { get; set; }
    public GameObject[] spaces = new GameObject[40];
    private void Awake()
    {
        if (board == null) board = this;
        else Destroy(this);
    }
}