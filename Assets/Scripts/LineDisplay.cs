using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDisplay : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawLine(int index,Vector3 pos)
    {
        
        lineRenderer.SetPosition(index,pos);
    }

    public void RemoveLine()
    {
    }
}
