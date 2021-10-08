using System;
using System.Collections;
using System.Collections.Generic;
using SlotGame;
using UnityEngine;

[Serializable]
public class PaylineWinData
{
    //TODO: Make these properties
    public int payLineNum;
    public int winAmt;
}

public class WinsHandler : MonoBehaviour
{
    [SerializeField] private PaylinesInfo paylinesInfo;
    [SerializeField] private ReelPanel reelPanel;
    [SerializeField] private PaytableData payTable;
    [SerializeField] private UiController uiController;
    
    private string prevSymbol;
    private int counter;
    private int totalWinAmt;

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
                //get the symbol name from specific reel stored at payLine points
                //get correct slot index
                
                var slotIndex = GetSlotIndex(payline, j); //payLine and the reel num
                var symbolName = reelPanel._allReels[j].reelStrip[slotIndex];
                
                // Debug.Log($"Symbol details - Reel -> {j} , reelStrip index - {slotIndex} , {symbolName} , payLine - {payline.paylinePoints[j]}");

                if (CheckWinFromSecondSymbol(j, symbolName, i, payline)) break;
                
                prevSymbol = symbolName;

            } //specific payLine

            prevSymbol = "";
            
        } // all payLines
        
        HighlightPayLines(); //Highlight the payLines
    }

    private bool CheckWinFromSecondSymbol(int paylinePoint, string symbolName, int currPayline, Payline payline)
    {
        if (paylinePoint > 0) //start from 2nd reel or 2nd payLine position
        {
            if (symbolName == prevSymbol)
            {
                // Debug.Log($"Same symbol");
                counter++;
            }
            else
            {
                // Debug.Log($"Diff symbol");
                //TODO: Check for wild here

                CheckIfWin(counter, currPayline);
                counter = 0;
                return true;
            }

            //In-case if all symbols lie on payLine
            if (counter == payline.paylinePoints.Count)
                CheckIfWin(counter, currPayline);
        }

        return false;
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
        
        // Debug.Log($"{slotIndex} - slot index");
        return slotIndex;
    }

    private void CheckIfWin(int count,int payLineNum)
    {
        // Debug.Log($"Checking win - {prevSymbol} - {count} on payLine {payLineNum}");
        if (count > 2)
        {
            // Debug.Log($"Win on  {prevSymbol}");
            var amt = CheckWinAmount(count);
            PaylineWinData line = new PaylineWinData();
            line.payLineNum = payLineNum;
            line.winAmt = amt;
            payLineWins.Add(line);
        }
        else
        {
            // Debug.Log($"Sorry!! No Win -> Count {count}");
        }
        
    }

    private int CheckWinAmount(int count)
    {
        /*if (count == 3)
        {
            Debug.Log($"3 of a kind {prevSymbol}");
        }
        else if (count == 4)
        {
            Debug.Log($"4 of a kind {prevSymbol}");
        }
        else if(count == 5)
        {
            Debug.Log($"5 of a kind {prevSymbol}");
        }*/
        
        // Decide which win to give based on payTable data
        var winAmtGiven = payTable.GetWinAmount(prevSymbol,count);
        totalWinAmt += winAmtGiven;
        Debug.Log($"<color=white> Win - {winAmtGiven} for {prevSymbol} </color>");
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