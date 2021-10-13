using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    public int index; //TODO: Make it a property after testing
    public string symbolName; //TODO: Make it a property after testing

    public SpriteRenderer _symbolImage { private set; get; }
    private SpriteRenderer highlightImage;
    
    private void Awake()
    {
        _symbolImage = GetComponent<SpriteRenderer>();
        GetComponent<Animator>().enabled = false;
        highlightImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        ShowHighlight(false);
    }

    public void ShowHighlight(bool value)
    {
        highlightImage.enabled = value;
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
