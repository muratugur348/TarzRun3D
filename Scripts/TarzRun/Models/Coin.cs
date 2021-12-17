using System.Collections;
using System.Collections.Generic;
using TarzRun.Managers;
using UnityEngine;
namespace TarzRun
{
    public class Coin : MonoBehaviour
    {
        public GameObject blast;
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
            if(other.gameObject.CompareTag("Player"))
            {
                ScoreManager.Instance.IncreaseCoin();
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<AudioSource>().Play();
                Destroy(transform.GetChild(0).gameObject);
                Destroy(Instantiate(blast, transform.position, transform.rotation), 2);
                Destroy(gameObject,2);
            }
        }
    }
}