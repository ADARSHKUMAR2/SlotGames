using System;
using System.Collections;
using System.Collections.Generic;
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

[Serializable]
public class Payline
{
    public List<int> paylinePoints;
}

public class PaylinesInfo : MonoBehaviour
{
    public List<Payline> _payLines;
    
}
