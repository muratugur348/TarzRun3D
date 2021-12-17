using System.Collections;
using System.Collections.Generic;
using TarzRun.Managers;
using UnityEngine;

public class Swipe : MonoBehaviour
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
        if(gameObject.CompareTag("SwipeUp") && other.CompareTag("Player"))
        {
            StartCoroutine(UIManager.Instance.OpenSwipeUp());
        }
        else if(gameObject.CompareTag("SwipeDown") && other.CompareTag("Player"))
        {
            StartCoroutine(UIManager.Instance.OpenSwipeDown());
        }
    }
}
