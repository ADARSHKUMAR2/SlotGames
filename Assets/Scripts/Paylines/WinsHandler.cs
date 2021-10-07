using System.Collections;
using System.Collections.Generic;
using SlotGame;
using UnityEngine;

public class WinsHandler : MonoBehaviour
{
    [SerializeField] private PaylinesInfo paylinesInfo;
    [SerializeField] private ReelPanel reelPanel;

    private string symbolName;
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
        for (int i = 0; i < allPaylines.Count; i++)
        {
            var payline = allPaylines[i];
            counter = 1;
            for (int j = 0; j < payline.paylinePoints.Count; j++)
            {
                //get the symbol name from specific reel stored at payline points
                //get correct slot index
                
                // Debug.Log($"{payline.paylinePoints[j]}");
                
                var slotIndex = GetCorrectSlotIndex(payline, j); //payline and the reel num
                var symbol = reelPanel._allReels[j]._slots[slotIndex].symbolName;

                /*if (symbolName == symbol)
                {
                    counter++;        
                }
                else
                {
                    CheckIfWin(counter);
                    break;
                }
                symbolName = symbol;*/
                
                
            }

            symbolName = "";
        }
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
        slotIndex = reelPanel._allReels[j].GetCorrectSlot(j); 
        
        // Debug.Log($"{slotIndex} - slot index");
        return slotIndex;
    }

    private void CheckIfWin(int count)
    {
        // Debug.Log($"{symbolName} - {count}");
    }
}