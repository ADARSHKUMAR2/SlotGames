using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SlotGame;
using UnityEngine;

public class Reel : MonoBehaviour, IReel
{
    [SerializeField] private  List<Slot> _slots;
    [Header("Enter SymbolNames")]
    public List<string> reelStrip;
    [SerializeField] private Symbols _symbols;

    [SerializeField] private int _reelNumber;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private float bounceOffset = 1f;
    [Range(1,5)]
    [SerializeField] private int reelSize;
    
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
            _slots[i]._symbolImage.sprite = _symbols.SetSymbolImage(reelStrip[value]);
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

    private void UpdateSlotIndex(Slot slot)
    {
        curSlotIndex--;
        if (curSlotIndex < 0)
            curSlotIndex = reelStrip.Count - 1;
        slot.UpdateIndex(curSlotIndex);
        slot._symbolImage.sprite = _symbols.SetSymbolImage(reelStrip[curSlotIndex]);
        slot.SetSymbolName(reelStrip[curSlotIndex]);
    }

    private void LerpSlots()
    {
        // Debug.Log($"Gap on {_reelNumber} - {gapBetweenSlots}");
        
        var totalSlots = reelSize + 2;
        for (int i = 0; i < totalSlots; i++)
        {
            var pos = _slots[i].transform.position;
            var finalPos = (Mathf.Round(pos.y * 100f)) / 100.0f;
            LeanTween.moveY(_slots[i].gameObject, finalPos-1f , 0.35f).setEase(LeanTweenType.easeSpring);
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
        var reelParent = GetComponentInParent<ReelPanel>();
        reelParent.UpdateReelsStoppedCount();
        
        var count = 5;
        if (reelParent.totalReelsStopped==count)
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
        yield return new WaitForSeconds(_reelNumber * 0.2f);
        InvokeRepeating(nameof(StopRandomly), 0.5f, 0.001f);
    }

    private void StopRandomly()
    {
        var centrePos = (startPos + endPos) / 2f;
        var centreSlot = (reelSize + 2) / 2;
        var pausePos = 0f;
        
        if (reelSize % 2 == 1) //odd
            pausePos = centrePos;
        else
        {
            pausePos = startPos - gapBetweenSlots;
            centreSlot -= 1;
        }

        pausePos += 1f;
        
        for (int i = 0; i < _slots.Count; i++)
        {
            if (Math.Abs(_slots[centreSlot].transform.position.y - (pausePos)) < 0.0001f) // - (-1f)
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
        if (_reelNumber == reelNum) // which reel
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (Math.Abs(_slots[i].transform.position.y - (0.5f)) < 0.01f) //means 2nd pos from top
                {
                    if (_slots[i].index == stopPosition % reelStrip.Count) //slotPos == stopPos
                    {
                        if (_spinCoroutine != null)
                        {
                            topSymbolIndex = _slots[i].index;
                            StopCoroutine(_spinCoroutine);
                            LerpSlots();
                            isReelStopped = true;
                        }
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
        if (payLinePoint == 0)
            index = (topSymbolIndex) % reelStrip.Count;
        
        else if (payLinePoint == 1)
            index = (topSymbolIndex + 1) % reelStrip.Count;
        
        else
            index = (topSymbolIndex + 2) % reelStrip.Count;

        return index;
    }

    public void HighlightSlot(int index)
    {
        foreach (var slot in _slots)
            if (slot.index == index)
            {
                slot.ShowHighlight(true);
                var reelParent = GetComponentInParent<ReelPanel>();
                reelParent.UpdateLine(_reelNumber,slot.transform.position);
                slot.transform.localScale = Vector3.one;
                slot.GetComponent<Animator>().enabled = true;
            }
    }

    public void RemoveHighlight()
    {
        var totalSlots = reelSize + 2;
        for (int i = 0; i < totalSlots; i++)
        {
            _slots[i].ShowHighlight(false);
            var reelParent = GetComponentInParent<ReelPanel>();
            reelParent.UpdateLine(_reelNumber,Vector3.zero);
            _slots[i].GetComponent<Animator>().enabled = false;
            _slots[i].transform.localScale = Vector3.one;
        }
    }
}
