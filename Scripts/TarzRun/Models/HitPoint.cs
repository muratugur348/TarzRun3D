using System.Collections;
using System.Collections.Generic;
using TarzRun.Controllers;
using TarzRun.Managers;
using UnityEngine;

namespace TarzRun
{
    public class HitPoint : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                LevelManager.Instance.isFailed = true;
                FindObjectOfType<PlayerController>().DeathAnimation();
                FindObjectOfType<PlayerController>().DeathSound();
                LevelManager.Instance.isGameFinished = true;
                StartCoroutine(UIManager.Instance.CloseGamePlayCanvas());
                StartCoroutine(UIManager.Instance.OpenLoseCanvas());
            }
        }
    }
}