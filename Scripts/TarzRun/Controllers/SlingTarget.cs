using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TarzRun.Controllers
{
    public class SlingTarget : MonoBehaviour
    {
        private GameObject _rightHandTarget;
        // Start is called before the first frame update
        void Start()
        {
            _rightHandTarget = GameObject.FindGameObjectWithTag("RightHandTarget");
        }

        // Update is called once per frame
        void Update()
        {
            ChangePositionAccordingToRightHand();
        }
        private void ChangePositionAccordingToRightHand()
        {
            transform.position = new Vector3(0 - _rightHandTarget.transform.localPosition.x * 75,
                2f + Mathf.Abs(_rightHandTarget.transform.localPosition.y - 1.4f) * 75,
                _rightHandTarget.transform.position.z + 20);
        }
    }
}