using System.Collections;
using System.Collections.Generic;
using SlotGame;
using UnityEngine;

public class WinsHandler : MonoBehaviour
{
    [SerializeField] private PaylinesInfo paylinesInfo;
    [SerializeField] private ReelPanel reelPanel;
    [SerializeField] private PaytableData payTable;
    
    private string prevSymbol;
    private int counter;
    
    public void CalculateWinInfo()
    {
        GetSymbolsLocation();
    }

    private void GetSymbolsLocation()
    {
        
    }

    [ContextMenu("Win")]
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
                
                var slotIndex = GetCorrectSlotIndex(payline, j); //payLine and the reel num
                var symbolName = reelPanel._allReels[j].reelStrip[slotIndex];
                
                // Debug.Log($"{payline.paylinePoints[j]}");
                
                // Debug.Log($"Symbol details - Reel -> {j} , reelStrip index - {slotIndex} , {symbolName} , payLine - {payline.paylinePoints[j]}");

                if (j > 0) //start from 2nd reel or 2nd payLine position
                {
                    if (symbolName == prevSymbol)
                    {
                        // Debug.Log($"Same symbol");
                        counter++;        
                    }
                    else
                    {
                        // Debug.Log($"Diff symbol");
                        CheckIfWin(counter,i);
                        counter = 0;
                        break;
                    }
                    
                    //In-case if all symbols lie on payLine
                    if(counter==payline.paylinePoints.Count)
                        CheckIfWin(counter,i);
                }
                
                prevSymbol = symbolName;

            } //specific payLine

            prevSymbol = "";
        } // all payLines
    }

    /// <summary>
    /// j=0 -> Top , j=1-> Middle pos; j=2-> Bottom pos
    /// </summary>
    /// <param name="payline"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private int GetCorrectSlotIndex(Payline payline, int j)
    {
        var slotIndex = 0;
        slotIndex = reelPanel._allReels[j].GetCorrectSlot(payline.paylinePoints[j]); 
        
        // Debug.Log($"{slotIndex} - slot index");
        return slotIndex;
    }

    private void CheckIfWin(int count,int payLineNum)
    {
        Debug.Log($"Checking win - {prevSymbol} - {count} on payLine {payLineNum}");
        if (count > 2)
        {
            // Debug.Log($"Win on  {prevSymbol}");
            DecideWinType(count);
        }
        else
        {
            Debug.Log($"Sorry!! No Win -> Count {count}");
        }
    }

    private void DecideWinType(int count)
    {
        if (count == 3)
        {
            Debug.Log($"3 of a kind {prevSymbol}");
        }
        else if (count == 4)
        {
            Debug.Log($"4 of a kind {prevSymbol}");
        }
        else if(count==5)
        {
            Debug.Log($"5 of a kind {prevSymbol}");
        }
        
        // Decide which win to give based on payTable data
        var winAmtGiven = payTable.GetWinAmount(prevSymbol,count);
        Debug.Log($"<color=white>Win - {winAmtGiven} for {prevSymbol} </color>");
    }
}