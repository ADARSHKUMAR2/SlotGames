using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int index; //TODO: Make it a property after testing
    public string symbolName; //TODO: Make it a property after testing

    public SpriteRenderer _symbolImage { private set; get; }
    private void Awake()
    {
        _symbolImage = GetComponent<SpriteRenderer>();
    }

    public void UpdateIndex(int value)
    {
        index = value;
    }

    public void SetSymbolName(string pName)
    {
        symbolName = pName;
    }
}
