using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PayTable
{
    public List<WinCategory> winsCategory;
}

[Serializable]
public class WinCategory
{   
    public string symbolName;
    public List<SymbolWins> symbolWinsList;
}

[Serializable]
public class SymbolWins
{
    public int winCombo;
    public int winAmount;
}

public class PaytableData : MonoBehaviour
{
    [SerializeField] private PayTable payTable;
    
}
