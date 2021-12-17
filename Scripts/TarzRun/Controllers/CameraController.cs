using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TarzRun.Managers;

namespace TarzRun.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private GameObject _leftArm;
        public CinemachineTargetGroup targetGroup;
        public GameObject thirdPersonPos;
        private float _chasingMoveCounter;
        private float _shootingTimeCounter;
        // Start is called before the first frame update
        void Start()
        {
            DOTween.Init();
            _leftArm= GameObject.FindGameObjectWithTag("LeftArm");
            _shootingTimeCounter = 1;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if(LevelManager.Instance.isGameFinished)
            {
                ChasingControl();
            }
            else if(LevelManager.Instance.isChasing)
            {
                ChasingControl();
                LookPoint();
            }

            else if (LevelManager.Instance.isGamePlayable)
            {
                SlingshotControl();
                LookPoint();
                //transform.position =  _leftArm.transform.position;
            }

        }
        public IEnumerator LookTarzan()
        {
            if (targetGroup.m_Targets[0].weight < 40)
            {
                targetGroup.m_Targets[0].weight++;
                yield return new WaitForSeconds(0.02f);
                StartCoroutine(LookTarzan());
            }
        }
        public void LookPoint()
        {
            //vcam.m_LookAt = _lookPoint.transform;
            if(targetGroup.m_Targets[0].weight!=0)
            targetGroup.m_Targets[0].weight = 0;
        }
        private void ChasingControl()
        {
            if (_shootingTimeCounter > 0)
                _shootingTimeCounter = 0;
            _chasingMoveCounter += Time.deltaTime;
            if (_chasingMoveCounter < .75f)
                transform.position = Vector3.Lerp(transform.position, thirdPersonPos.transform.position, 1 * Time.deltaTime);
            else if(!LevelManager.Instance.isFailed)
                transform.position = Vector3.Lerp(transform.position, new Vector3(thirdPersonPos.transform.position.x,
                        transform.position.y, thirdPersonPos.transform.position.z), 5 * Time.deltaTime);
            else if(LevelManager.Instance.isFailed)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(thirdPersonPos.transform.position.x,
                        transform.position.y, thirdPersonPos.transform.position.z-5), 0.5f * Time.deltaTime);
            }
        }
        private void SlingshotControl()
        {
            if (_chasingMoveCounter > 0)
                _chasingMoveCounter = 0;
            _shootingTimeCounter += Time.deltaTime;
            if (_shootingTimeCounter < 1)
                transform.position = Vector3.Lerp(transform.position, _leftArm.transform.position, 2.5f * Time.deltaTime);
            else
                transform.position = Vector3.Lerp(transform.position, _leftArm.transform.position, 50 * Time.deltaTime);
        }
    }
}