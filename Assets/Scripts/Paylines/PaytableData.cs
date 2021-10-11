﻿using System;
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

    public int GetWinAmount(string symbolName, int count)
    {
        foreach (var allWins in payTable.winsCategory)
        {
            // Debug.Log($"Name - {allWins.symbolName} , {symbolName}");
            if (allWins.symbolName == symbolName)
            {
                var maxAmt = 0;
                foreach (var symbolWins in allWins.symbolWinsList)
                {
                    if (count >= symbolWins.winCombo)
                    {
                        Debug.Log($" Wins for {symbolName} - {symbolWins.winAmount} for {symbolWins.winCombo} of a kind");
                        if (symbolWins.winAmount > maxAmt)
                            maxAmt = symbolWins.winAmount;

                    }
                }
                return maxAmt;
            }
        }

        return 0;
    }
}
