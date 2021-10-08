using System;
using System.Collections;
using System.Collections.Generic;
using SlotGame;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private ReelPanel _reelPanel;
    [SerializeField] private Text winMsgText;
    [SerializeField] private Text totalWinAmountText;
    [SerializeField] private WinsHandler winsHandler;
    
    private void OnEnable()
    {
        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        _playButton.onClick.AddListener(PlayGame);
    }

    public void ChanePlayBtnInteraction(bool value)
    {
        _playButton.interactable = value;
    }
    private void PlayGame()
    {
        var btnText = _playButton.GetComponentInChildren<Text>();
        if (btnText.text.Contains("Start"))
        {
            ResetUiData();
            _reelPanel.SpinReels();
            btnText.text = "Stop";
        }
        else
        {
            _reelPanel.SpinReels(false);
            btnText.text = "Start";
        }
    }

    public void UpdateWinMsg(int winAmt, int payLineNum)
    {
        winMsgText.text = $"You won {winAmt} on payLine {payLineNum} ";
    }

    public void UpdateTotalWinAmt(int winAmt)
    {
        totalWinAmountText.text = $"Total Win - {winAmt}";
    }

    private void OnDisable()
    {
        RemoveButtonListeners();
    }

    private void RemoveButtonListeners()
    {
        _playButton.onClick.RemoveListener(PlayGame);
    }

    private void ResetUiData()
    {
        winMsgText.text = $"";
        totalWinAmountText.text = $"";
        winsHandler.ResetData();
    }
}
