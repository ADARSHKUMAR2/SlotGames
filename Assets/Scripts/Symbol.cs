using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    public int index { private set; get; }
    public string symbolName{ private set; get; }
    public SpriteRenderer _symbolImage { private set; get; }
    private SpriteRenderer highlightImage;
    private Animator anim;

    private void Awake()
    {
        GetRequiredComponents();
        PlayAnimation(false);
        ShowHighlight(false);
    }

    private void GetRequiredComponents()
    {
        _symbolImage = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        highlightImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(bool value)
    {
        anim.enabled = value;
        anim.Rebind();
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