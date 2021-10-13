using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SlotGame;
using UnityEngine;

public class Reel : MonoBehaviour, IReel
{
    [SerializeField] private  List<Symbol> _slots;
    [Header("Enter SymbolNames")]
    public List<string> reelStrip;
    [SerializeField] private SymbolsData symbolsData;

    [SerializeField] private int _reelNumber;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private float bounceOffset = 3f;
    [Range(1,5)]
    [SerializeField] private int reelSize;

    [SerializeField] private ReelPanel _reelPanel;
    
    private Coroutine _spinCoroutine;
    private int curSlotIndex;
    private Action onBtnInteractionChange;
    private bool isReelStopped;

    //TODO: make it a property
    public int topSymbolIndex; // the index of the symbol which at the top position in slot
    private LineRenderer lineRenderer;
    
    private int startPos = 3;
    private int endPos = -3;
    private int maxGap = 6;
    private float gapBetweenSlots;
    
    private void Start()
    {
        //Enable slots as per size entered -> (+2) of i/p
        // Update positions of slots as per size
        EnableSlotsAsPerReelSize();
    }

    private void EnableSlotsAsPerReelSize()
    {
        var totalSlots = reelSize + 2;
        for (int i = 0; i < totalSlots; i++)
            _slots[i].gameObject.SetActive(true);
        
        UpdatePosition(totalSlots);
        
    }

    private void UpdatePosition(int totalSlots)
    {
        gapBetweenSlots =   (maxGap * 1f)/(totalSlots - 1);
        for (int i = 0; i < totalSlots; i++)
        {
            var pos = _slots[i].transform.position;
            pos.y = startPos - (gapBetweenSlots*i);
            _slots[i].transform.position = pos;
        }
        
        InitialiseSlots(totalSlots);
    }

    private void InitialiseSlots(int totalSlots)
    {
        var stripLength = reelStrip.Count;
        for (int i = 0; i < totalSlots; i++)
        {
            int value = i % stripLength;
            _slots[i]._symbolImage.sprite = symbolsData.SetSymbolImage(reelStrip[value]);
            _slots[i].SetSymbolName(reelStrip[value]);
            _slots[i].UpdateIndex(i);
        }
        
    }

    public void StartSpin()
    {
        _spinCoroutine = StartCoroutine(SpinReel());
    }

    private IEnumerator SpinReel()
    {
        var totalSlots = reelSize + 2;
        
        for (int i = 0; i < totalSlots; i++)
            _slots[i].transform.Translate(Vector3.down * 0.1f);
        
        UpdateSlotPosition(totalSlots);
        yield return new WaitForSeconds(0.005f * _spinSpeed);
        _spinCoroutine = StartCoroutine(SpinReel());
    }

    private void UpdateSlotPosition(int totalSlots)
    {
        for (int i = 0; i < totalSlots; i++)
        {
            var pos = _slots[i].transform.position;
            if (pos.y <  endPos - gapBetweenSlots)
            {
                pos.y = startPos; //moves the last slot pos to the top
                UpdateSlotIndex(_slots[i]);
            }
            _slots[i].transform.position = pos;
        }
    }

    private void UpdateSlotIndex(Symbol symbol)
    {
        curSlotIndex--;
        if (curSlotIndex < 0)
            curSlotIndex = reelStrip.Count - 1;
        symbol.UpdateIndex(curSlotIndex);
        symbol._symbolImage.sprite = symbolsData.SetSymbolImage(reelStrip[curSlotIndex]);
        symbol.SetSymbolName(reelStrip[curSlotIndex]);
    }

    private void LerpSlots()
    {
        // Debug.Log($"Gap on {_reelNumber} - {gapBetweenSlots}");
        
        var totalSlots = reelSize + 2;
        for (int i = 0; i < totalSlots; i++)
        {
            var pos = _slots[i].transform.position;
            var finalPos = (Mathf.Round(pos.y * 100f)) / 100.0f;
            LeanTween.moveY(_slots[i].gameObject, finalPos+bounceOffset , 0.35f).setEase(LeanTweenType.easeSpring);
        }

        InvokeBtnEnableAction();
        Invoke(nameof(GetTopSlotIndex),0.35f);
    }

    private void GetTopSlotIndex()
    {
        var totalSlots = reelSize + 2;
        var pos = 3f;
        var minDis = 100f;

        for (int i = 0; i < totalSlots; i++)
        {
            if(_slots[i].transform.position.y < pos)
            {
                var dis = startPos - _slots[i].transform.position.y ;
                if (dis < minDis)
                {
                    minDis = dis;
                    topSymbolIndex = _slots[i].index;
                }
            }    
        }
        
    }

    private void InvokeBtnEnableAction()
    {
        _reelPanel.UpdateReelsStoppedCount();
        
        var count = 5;
        if (_reelPanel.totalReelsStopped==count)
        {
            onBtnInteractionChange?.Invoke();
            onBtnInteractionChange = null;
        }   
        
    }
    
    #region RandomStop

    public void StopSpin([CanBeNull] Action btnInteraction)
    {
        StartCoroutine(SequentialStop());
        onBtnInteractionChange = btnInteraction;
    }

    private IEnumerator SequentialStop()
    {
        yield return new WaitForSeconds(_reelNumber * 0.3f);
        InvokeRepeating(nameof(StopRandomly), 0.5f, 0.001f);
    }

    private void StopRandomly()
    {
        var centrePos = (startPos + endPos) / 2f;
        // var centreSlot = (reelSize + 2) / 2;
        var pausePos = 0f;
        
        if (reelSize % 2 == 1) //odd
            pausePos = centrePos;
        else
        {
            pausePos = startPos - gapBetweenSlots;
            // centreSlot -= 1;
        }

        pausePos -= bounceOffset;
        
        for (int i = 0; i < _slots.Count; i++)
        {
            if (Math.Abs(_slots[i].transform.position.y - (pausePos)) < 0.0001f) 
            {
                if (_spinCoroutine != null)
                    StopCoroutine(_spinCoroutine);
                
                CancelInvoke();
                LerpSlots();
                return;
            }
        }

    }

    #endregion

    #region CustomStop

    public void CheckForCustomPos(int stopPosition, int reelNum,[CanBeNull] Action btnInteraction)
    {
        isReelStopped = false;
        onBtnInteractionChange = btnInteraction;
        StartCoroutine(CheckStopPos(stopPosition, reelNum));
    }

    private IEnumerator CheckStopPos(int stopPosition, int reelNum)
    {
        var stopPos = 0f;
        stopPos = startPos-gapBetweenSlots - bounceOffset;
        
        if (_reelNumber == reelNum) // which reel
        {
            for (int i = 0; i < reelSize+2; i++)
            {
                if (Math.Abs( _slots[i].transform.position.y - (stopPos)) < 0.001f && _slots[i].index == stopPosition % reelStrip.Count) //means 2nd pos from top && slotPos == stopPos
                {
                    if (_spinCoroutine != null)
                    {
                        topSymbolIndex = _slots[i].index;
                        StopCoroutine(_spinCoroutine);
                        LerpSlots();
                        isReelStopped = true;
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.0001f);
        if(!isReelStopped)
            StartCoroutine(CheckStopPos(stopPosition, reelNum));
    }

    #endregion

    public int GetCorrectSlot(int payLinePoint)
    {
        var index = 0;
        if (payLinePoint > reelSize-1)
            return -1;

        index = (topSymbolIndex + payLinePoint) % reelStrip.Count;
        return index;
    }

    public void HighlightSlot(int index)
    {
        for (int i = 0; i < reelSize+2; i++)
        {
            if (_slots[i].index == index)
            {
                _slots[i].ShowHighlight(true);
                _reelPanel.UpdateLine(_reelNumber,_slots[i].transform.position);
                _slots[i].transform.localScale = Vector3.one;
                _slots[i].PlayAnimation(true);
            }
        }
    }

    public void RemoveHighlight()
    {
        var totalSlots = reelSize + 2;
        for (int i = 0; i < totalSlots; i++)
        {
            _slots[i].ShowHighlight(false);
            _reelPanel.UpdateLine(_reelNumber,Vector3.zero);
            _slots[i].PlayAnimation(false);
            _slots[i].transform.localScale = Vector3.one;
        }
    }
}
