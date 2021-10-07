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
            _reelPanel.SpinReels();
            btnText.text = "Stop";
        }
        else
        {
            _reelPanel.SpinReels(false);
            btnText.text = "Start";
        }
    }

    private void OnDisable()
    {
        RemoveButtonListeners();
    }

    private void RemoveButtonListeners()
    {
        _playButton.onClick.RemoveListener(PlayGame);
    }
}
