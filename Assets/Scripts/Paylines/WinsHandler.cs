﻿using System;
using System.Collections;
using System.Collections.Generic;
using SlotGame;
using UnityEngine;

[Serializable]
public class PaylineWinData
{
    //TODO: Make these properties
    public int payLineNum;
    public string winSymbol;
    public int winAmt;
    public int winCombo;
}

public class WinsHandler : MonoBehaviour
{
    [SerializeField] private PaylinesInfo paylinesInfo;
    [SerializeField] private ReelPanel reelPanel;
    [SerializeField] private PaytableData payTable;
    [SerializeField] private UiController uiController;
    
    //TODO: Make them local variables
    private string prevSymbol;
    private string winSymbol = "";
    private int counter;
    private int totalWinAmt;
    //
    
    private Coroutine winCoroutine;
    
    //Contains the list of all payLines on which the player won
    public List<PaylineWinData> payLineWins; //TODO:Make it private later after testing

    #region LoginWin

    public void CheckWin()
    {
        var allPaylines = paylinesInfo._payLines;
        for (int i = 0; i < allPaylines.Count; i++) //all payLines
        {
            var payline = allPaylines[i];
            counter = 1;
            for (int j = 0; j < payline.paylinePoints.Count; j++) // specific payLine
            {
                //get the symbol name from specific reel stored at payLine points & get correct slot index
                
                var slotIndex = GetSlotIndex(payline, j); //payLine and the reel num
                var symbolName = reelPanel._allReels[j].reelStrip[slotIndex];
                
                if (CheckWinFromSecondSymbol(j, symbolName, i, payline)) break;
                
                prevSymbol = symbolName;

            } //specific payLine

            prevSymbol = "";
            winSymbol = "";
            
        } // all payLines
        
        HighlightPayLines(); //Highlight the payLines
    }

    private bool CheckWinFromSecondSymbol(int paylinePoint, string currSymbolName, int currPayline, Payline payline)
    {
        // if (currPayline == 7)
        // {
            if (paylinePoint > 0) //start from 2nd reel or 2nd payLine position
            {
                if (currSymbolName == prevSymbol) // W:Wild
                {
                    counter++;
                    winSymbol = currSymbolName;
                    Debug.Log($"Same symbol ->Current {currSymbolName} , Prev {prevSymbol} ,{counter}");
                }
                else
                {
                    Debug.Log($"Diff symbol ->Current {currSymbolName} , Prev {prevSymbol} ,{counter} , win {winSymbol}");
                    if (!CheckingForWild(currSymbolName, currPayline, payline)) return false;

                    CheckIfWin(counter, currPayline,winSymbol);
                    counter = 0;
                    return true;
                }

                //In-case if all symbols lie on payLine
                CheckOnWinComplete(currPayline, payline);
            }
        // }
        
        return false;
    }

    private bool CheckingForWild(string currSymbolName, int currPayline, Payline payline)
    {
        // if (currPayline == 6)
        // {
         
            if (prevSymbol == "W")
            {
                //check if the current symbol is the ongoing win symbol (if not null)
                if (winSymbol != "")
                {
                    if (winSymbol == currSymbolName)
                    {
                        Debug.Log($"Win symbol same as current symbol");
                        counter++;
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Win symbol is empty");
                    winSymbol = currSymbolName;
                    counter++;
                    return false;
                }
            }

            if (currSymbolName == "W")
            {
                Debug.Log($"Current symbol is wild");
                counter++;
                CheckOnWinComplete(currPayline, payline);
                return false;
            }
   
        // }
        
        return true;
    }

    private void CheckOnWinComplete(int currPayline, Payline payline)
    {
        if (counter == payline.paylinePoints.Count)
            CheckIfWin(counter, currPayline, winSymbol);
    }

    /// <summary>
    /// j=0 -> Top , j=1-> Middle pos; j=2-> Bottom pos
    /// </summary>
    /// <param name="payline"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private int GetSlotIndex(Payline payLine, int j)
    {
        var slotIndex = 0;
        slotIndex = reelPanel._allReels[j].GetCorrectSlot(payLine.paylinePoints[j]);
        return slotIndex;
    }

    private void CheckIfWin(int count, int payLineNum, string currSymbolName)
    {
        Debug.Log($"Checking win - {prevSymbol} - {count} on payLine {payLineNum}");
        if (count > 2)
        {
            var amt = CheckWinAmount(count,currSymbolName);
            PaylineWinData line = new PaylineWinData();
            line.payLineNum = payLineNum;
            line.winAmt = amt;
            line.winSymbol = currSymbolName;
            line.winCombo = count;
            payLineWins.Add(line);
        }
        
    }

    private int CheckWinAmount(int count, string currSymbolName)
    {
        var winAmtGiven = payTable.GetWinAmount(currSymbolName,count);
        totalWinAmt += winAmtGiven;
        Debug.Log($"<color=white> Win - {winAmtGiven} for {currSymbolName} </color>");
        return winAmtGiven;
    }

    #endregion
    
    #region Representation
    
    /// <summary>
    /// Representation of payLines
    /// </summary>
    private void HighlightPayLines()
    {
        uiController.UpdateTotalWinAmt(totalWinAmt);
        if (payLineWins.Count > 0)
            winCoroutine = StartCoroutine(Highlight());
    }

    private IEnumerator Highlight()
    {
        foreach (var payLine in payLineWins)
        {
            //Highlight the correct slotIndex of the specific reel
            var index = paylinesInfo._payLines[payLine.payLineNum].paylinePoints;
            
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < index.Count; i++)
            {
                // Debug.Log($"Points - {index[i]} -> PayLine {payLine}");
                var currReel = reelPanel._allReels[i];
                var slotIndex = currReel.GetCorrectSlot(index[i]); // reelNo. , position of slot
                
                currReel.HighlightSlot(slotIndex);
                uiController.UpdateWinMsg(payLine.winAmt,payLine.payLineNum);
            }
            yield return new WaitForSeconds(2f);
            RemoveHighlight();
            yield return new WaitForSeconds(2f);
        }

        HighlightPayLines(); 
        yield return null;
    }

    private void RemoveHighlight()
    {
        foreach (var reel in reelPanel._allReels)
            reel.RemoveHighlight();
    }

    #endregion

    public void ResetData()
    {
        if(winCoroutine!=null)
            StopCoroutine(winCoroutine);

        totalWinAmt = 0;
        RemoveHighlight();
        payLineWins.Clear();
    }
}