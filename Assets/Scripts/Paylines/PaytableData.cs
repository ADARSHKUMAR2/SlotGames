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

    public void GetWinAmount(string symbolName, int count)
    {
        foreach (var allWins in payTable.winsCategory)
        {
            if (allWins.symbolName == symbolName)
            {
                foreach (var symbolWins in allWins.symbolWinsList)
                {
                    Debug.Log($" Wins for {symbolName} - {symbolWins.winAmount} for {symbolWins.winCombo} of a kind");
                }
            }
        }
    }
}
