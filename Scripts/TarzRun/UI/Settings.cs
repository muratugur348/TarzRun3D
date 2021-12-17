using System.Collections;
using System.Collections.Generic;
using TarzRun.Controllers;
using TarzRun.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace TarzRun.UI
{
    public class Settings : MonoBehaviour, IPointerDownHandler
    {
        private bool _isActive = false;
        public Slider slingShootAimSlider;
        public Slider HorizontalMoveSlider;
        public Slider ThirdPersonCamYSlider;
        public Slider ThirdPersonCamZSlider;
        private GameObject _cameraPosition;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void AssignSliders()
        {
            slingShootAimSlider.value = FindObjectOfType<PlayerController>().slingShootMoveSpeed *10;
            HorizontalMoveSlider.value = FindObjectOfType<PlayerController>().moveHorizontalSpeed/20;
            _cameraPosition = GameObject.FindGameObjectWithTag("CameraPosition");
            ThirdPersonCamYSlider.value = _cameraPosition.transform.localPosition.y/10;
            ThirdPersonCamZSlider.value = Mathf.Abs(_cameraPosition.transform.localPosition.z/10);
        }
        private void AssignValues()
        {
            FindObjectOfType<PlayerController>().slingShootMoveSpeed = (slingShootAimSlider.value/10)+0.01f;
            FindObjectOfType<PlayerController>().moveHorizontalSpeed = (HorizontalMoveSlider.value * 20)+2;
            _cameraPosition.transform.localPosition = new Vector3(0, ThirdPersonCamYSlider.value*10, -ThirdPersonCamZSlider.value*10);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isActive)
            {
                UIManager.Instance.Settings.SetActive(false);
                Time.timeScale = 1;
                _isActive = false;
                AssignValues();
            }
            else
            {
                UIManager.Instance.Settings.SetActive(true);
                _isActive = true;
                Time.timeScale = 0;
            }
        }
    }
}