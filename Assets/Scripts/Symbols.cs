using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbols : MonoBehaviour
{
    [SerializeField] private List<SymbolInfo> _allSymbols;

    public Sprite SetSymbolImage(string sName)
    {
        foreach (var symbol in _allSymbols)
        {
            if (symbol._symbolName == sName)
                return symbol.symbolSprite;
        }

        return null;
    }
    
}
