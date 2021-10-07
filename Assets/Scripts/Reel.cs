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
    
    private Coroutine _spinCoroutine;
    private int curSlotIndex;
    private Action onBtnInteractionChange;
    private bool isReelStopped;

    public int topSymbolIndex; // the index of the symbol which at the top position in slot
    
    private void Start()
    {
        InitialiseSlots();
    }
    
    private void InitialiseSlots()
    {
        var stripLength = reelStrip.Count;
        for (int i = 0; i < _slots.Count; i++)
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
        foreach (var slot in _slots)
            slot.transform.Translate(Vector3.down * 0.1f);

        UpdateSlotPosition();
        yield return new WaitForSeconds(0.005f * _spinSpeed);
        _spinCoroutine = StartCoroutine(SpinReel());
    }

    private void UpdateSlotPosition()
    {
        foreach (var slot in _slots)
        {
            var pos = slot.transform.position;
            //To reset position
            if (pos.y < -4.5f)
            {
                pos.y = 3f; //moves the last slot pos to the top
                UpdateSlotIndex(slot);
            }
            slot.transform.position = pos;
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
        foreach (var slot in _slots)
        {
            var pos = slot.transform.position;
            LeanTween.moveY(slot.gameObject, pos.y + bounceOffset, 0.35f).setEase(LeanTweenType.easeSpring);
        }

        InvokeBtnEnableAction();
        GetTopSlotIndex();
    }

    private void GetTopSlotIndex()
    {
        foreach (var slot in _slots)
        {
            if (Math.Abs(slot.transform.position.y - (0.5f)) < 0.2f)
                topSymbolIndex = slot.index;
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
        for (int i = 0; i < _slots.Count; i++)
        {
            if (Math.Abs(_slots[i].transform.position.y - (-1f)) < 0.0001f)
            {
                if (_spinCoroutine != null)
                    StopCoroutine(_spinCoroutine);
                
                CancelInvoke();
                LerpSlots();
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

    public int GetCorrectSlot(int paylinePoint)
    {
        // Debug.Log($"Payline points - {paylinePoint}");
        var index = 0;
        if (paylinePoint == 0)
        {
            // Debug.Log($"Case 1 {topSymbolIndex}");
            index = (topSymbolIndex) % reelStrip.Count;
        }
        else if (paylinePoint == 1)
        {
            // Debug.Log($"Case 2 {topSymbolIndex}");
            index = (topSymbolIndex + 1) % reelStrip.Count;
        }
        else
        {
            // Debug.Log($"Case 3 {topSymbolIndex}");
            index = (topSymbolIndex + 2) % reelStrip.Count;
        }

        // Debug.Log($"<color=magenta>  reel - {_reelNumber} , Index - {index} </color>");
        return index;
    }
}
