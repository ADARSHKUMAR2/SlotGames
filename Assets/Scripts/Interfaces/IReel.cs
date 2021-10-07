using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReel
{
    void StartSpin();
    void StopSpin(Action btn);
}
