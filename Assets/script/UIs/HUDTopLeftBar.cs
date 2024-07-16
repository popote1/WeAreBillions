﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace script.UIs
{
    public class HUDTopLeftBar : MonoBehaviour
    {
        [Header("Zombie Counter")] 
        public Button _bpZombieCount;
        [SerializeField] private TMP_Text _txtZombieCount;
        [Header("Objectifs")] 
        [SerializeField] private GameObject _panelObjectif;
        [SerializeField] private Button _bpObjectif;
        [SerializeField] private TMP_Text _txtObjectifs;
        
        private void Start() {
            _bpObjectif.onClick.AddListener(ShowHideObjectifPanel);
        }

        private void Update() {
            ManageZombieCounts();
        }

        private void ManageZombieCounts() {
            _txtZombieCount.text = StaticData.ZombieCount.ToString();
        }

        private void ShowHideObjectifPanel() {
            _panelObjectif.SetActive(!_panelObjectif.activeSelf);
        }

        public void SetObjectifText(string text) {
            _txtObjectifs.text = text;
        }
        
    }
}