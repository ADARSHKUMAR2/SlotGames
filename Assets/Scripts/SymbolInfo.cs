using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolInfo : MonoBehaviour
{
    public string _symbolName;
    // public int _symbolId;
    public Sprite symbolSprite { private set; get; }

    private void Awake()
    {
        symbolSprite = GetComponent<SpriteRenderer>().sprite;
    }
}
