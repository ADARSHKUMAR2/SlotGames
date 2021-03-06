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
    [Range(1,4)] [SerializeField] private int reelSize;
    [SerializeField] private ReelPanel _reelPanel;
    
    private Coroutine _spinCoroutine;
    private int curSlotIndex;
    private Action onBtnInteractionChange;
    public bool isReelStopped;
    public int topSymbolIndex { private set; get; } // the index of the symbol which at the top position in slot
    //Reel data
    private int startPos = 3;
    private int endPos = -3;
    private float gapBetweenSlots;
    
    private void Start()
    {
        EnableSlotsAsPerReelSize();//Enable slots as per size entered -> (+2) of i/p & Update positions of slots as per size
    }

    private void EnableSlotsAsPerReelSize()
    {
        for (int i = 0; i < reelSize + 2; i++)
            _slots[i].gameObject.SetActive(true);
        UpdatePosition(reelSize + 2);
    }

    private void UpdatePosition(int totalSlots)
    {
        var maxGap = startPos - endPos;
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
        for (int i = 0; i < reelSize + 2; i++)
            _slots[i].transform.Translate(Vector3.down * 0.1f);
        UpdateSlotPosition(reelSize + 2);
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
        for (int i = 0; i < reelSize + 2; i++)
        {
            var pos = _slots[i].transform.position;
            var finalPos = (Mathf.Round(pos.y * 10f)) / 10.0f;
            LeanTween.moveY(_slots[i].gameObject, finalPos+bounceOffset , 0.35f).setEase(LeanTweenType.easeSpring);
        }
        Invoke(nameof(GetTopSlotIndex),0.4f);
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
        InvokeBtnEnableAction();
    }

    private void InvokeBtnEnableAction()
    {
        _reelPanel.UpdateReelsStoppedCount();
        if (_reelPanel.totalReelsStopped==_reelPanel._allReels.Count)
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
    public void CheckForCustomPos(int stopPosition,int reelNum,[CanBeNull] Action btnInteraction)
    {
        isReelStopped = false;
        onBtnInteractionChange = btnInteraction;
        StartCoroutine(CheckStopPos(stopPosition, reelNum));
    }

    private IEnumerator CheckStopPos(int stopPosition, int reelNum)
    {
        var stopPos = 0f;
        stopPos = startPos - gapBetweenSlots - bounceOffset;
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
                        yield return null;
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
        for (int i = 0; i < reelSize + 2; i++)
        {
            _slots[i].ShowHighlight(false);
            _reelPanel.UpdateLine(_reelNumber,Vector3.zero);
            _slots[i].PlayAnimation(false);
            _slots[i].transform.localScale = Vector3.one;
        }
    }
}
