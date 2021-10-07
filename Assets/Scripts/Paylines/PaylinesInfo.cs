﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Payline
{
    public List<int> paylinePoints;
    public int winAmount;
}

public class PaylinesInfo : MonoBehaviour
{
    public List<Payline> _payLines;
    
}