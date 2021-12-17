using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Patterns.Creational;
using TarzRun.Controllers;

namespace TarzRun.Managers
{
    public class ScoreManager : Singleton<ScoreManager>
    {

        [HideInInspector]
        public int tarzanHealth;
        [HideInInspector]
        public int coinNumber;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        }
        public void DecreaseTarzanHealth()
        {
            tarzanHealth--;
            if(tarzanHealth==0)
            {
                FindObjectOfType<Tarzan>().Death();
                FindObjectOfType<PlayerController>().WinAnimation();
                StartCoroutine(FindObjectOfType<CameraController>().LookTarzan());
                RUIPanel.Open("Win");
                LevelManager.Instance.isGameFinished = true;
                StartCoroutine(UIManager.Instance.CloseGamePlayCanvas());
            }
            UIManager.Instance.ChangeHealthScrollValue();
        }
        public void IncreaseCoin()
        {
            coinNumber += 10;
            UIManager.Instance.UpdateCoinText(coinNumber);
        }
    }
}
