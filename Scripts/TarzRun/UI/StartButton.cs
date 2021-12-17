using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TarzRun.Controllers;
using TarzRun.Managers;
using Core.UI;

namespace TarzRun.UI
{
    public class StartButton : UIButton
    {
        // Start is called before the first frame update
        private void Start()
        {


        }
        protected override void DoAction()
        {

            StartGame();
            Invoke("OpenGamePlay",1f);
            
        }

        private void OpenGamePlay()
        {
            RUIPanel.Open("Gameplay");
        }
        private void StartGame()
        {
            LevelManager.Instance.isGameStarted = true;
            StartCoroutine(FindObjectOfType<CameraController>().LookTarzan());
            FindObjectOfType<Tarzan>().StartGame();
            StartCoroutine(UIManager.Instance.DoStartUIChanges());
            FindObjectOfType<Settings>().AssignSliders();
            GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
    }
}