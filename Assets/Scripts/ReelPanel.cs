using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotGame
{
    public class ReelPanel : MonoBehaviour
    {
        [SerializeField] private UiController _uiController;
        [SerializeField] private WinsHandler winsHandler;
        [SerializeField] private LineDisplay lineDisplay;
        
        public List<Reel> _allReels;
        
        public List<int> stopPositions;
        public bool _stopAtSpecificPos;
        public int totalReelsStopped { private set; get; }
        public void SpinReels(bool startSpin = true)
        {
            foreach (var reel in _allReels)
            {
                if (startSpin)
                {
                    totalReelsStopped = 0;
                    reel.StartSpin();
                }
                else
                {
                    _uiController.ChanePlayBtnInteraction(false);
                    
                    if(_stopAtSpecificPos)
                        ForceStop(stopPositions,MakeBtnInteractable);
                    else
                        reel.StopSpin(MakeBtnInteractable); 
                }
            }
        }

        private void ForceStop(List<int> stopPositions,Action btnInteraction)
        {
            StartCoroutine(StartChecking(stopPositions,btnInteraction));
        }

        private IEnumerator StartChecking(List<int> stopPositions, Action btnInteraction)
        {
            for (int i = 0; i < stopPositions.Count; i++)
            {
                _allReels[i].CheckForCustomPos(stopPositions[i], i,btnInteraction); ;
                yield return new WaitForSeconds(0.75f); 
            }
        }


        private void MakeBtnInteractable()
        {
            _uiController.ChanePlayBtnInteraction(true);
            winsHandler.CheckWin();  //Calculate winInfo now
        }
        public void UpdateReelsStoppedCount()
        {
            totalReelsStopped++;
        }

        public void UpdateLine(int index, Vector3 pos)
        {
            lineDisplay.DrawLine(index,pos);
        }
    }

}

