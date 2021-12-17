using System.Collections;
using System.Collections.Generic;
using TarzRun.Managers;
using UnityEngine;

namespace TarzRun
{
    public class Bullet : MonoBehaviour
    {
        public float moveForwardSpeed;
        public GameObject explosion;
        private Rigidbody _rb;
        private GameObject _slingHandleLeft;
        private GameObject _player;
        private bool _isShooted=false;
        private Vector3 _targetPos;
        // Start is called before the first frame update
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _slingHandleLeft = GameObject.FindGameObjectWithTag("SlingHandleLeft");
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if(LevelManager.Instance.isHolding && !_isShooted)
            {
                transform.position = _slingHandleLeft.transform.position;
            }
            else
            {
                MoveToTarget();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Tarzan"))
            {
                ScoreManager.Instance.DecreaseTarzanHealth();
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        private void MoveToTarget()
        {
            if (!_isShooted)
            {
                if (!_player.GetComponent<LineRenderer>().enabled)
                    Destroy(gameObject);
                _player.GetComponent<LineRenderer>().enabled = false;
                _targetPos = _player.GetComponent<LineRenderer>().GetPosition(1);
                transform.LookAt(_targetPos);
                _isShooted = true;
            }
            _rb.velocity = transform.forward * moveForwardSpeed;
        }
    }
}