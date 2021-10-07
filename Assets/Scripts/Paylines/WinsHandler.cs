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

    private void CheckWin()
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
                // var slotIndex = payline.paylinePoints[j];
                
                var slotIndex = GetCorrectSlotIndex(payline, j); //payline and the reel num
                var symbol = reelPanel._allReels[j]._slots[slotIndex].symbolName;

                if (symbolName == symbol)
                {
                    counter++;        
                }
                else
                {
                    CheckIfWin(counter);
                }
                symbolName = symbol;
                
                
            }

            symbolName = "";
        }
    }

    private int GetCorrectSlotIndex(Payline payline, int j)
    {
        var slotIndex = 0;
        if (j == 0) // top position 
        {
            
        }
        else if(j==1) //middle position
        {
            
        }
        else //last position
        {
            
        }
        return slotIndex;
    }

    private void CheckIfWin(int count)
    {
        
    }
}
