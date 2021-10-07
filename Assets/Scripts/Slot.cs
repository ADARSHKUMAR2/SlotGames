using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int index {private set; get; }

    public SpriteRenderer _symbolImage { private set; get; }
    public string symbolName { private set; get; }
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
