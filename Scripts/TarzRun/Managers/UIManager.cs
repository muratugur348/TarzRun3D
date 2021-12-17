using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Patterns.Creational;
using TarzRun.Controllers;
using DG.Tweening;

namespace TarzRun.Managers
{
    public class UIManager : Singleton<UIManager>
    {   public GameObject GamePlayCanvas;
        public GameObject Settings;
        public GameObject HoldAndDrag;
        public GameObject Swerve;
        public GameObject SwipeUp;
        public GameObject SwipeDown;
       
        public Text levelText;
        public Text coinText;
        
        public Image tarzanHealthImage;
        public Image tarzanHead;
        public Image tarzanWay;
        
        private int _levelNumber = 1;
        private int _coinNumber = 0;
        private float _decreaseValuePerShot;
        private float _differenceBetweenHandleAndTarzan;
        private float _tarzanStartZ;
        private float _currentTime;
        private GameObject _tarzan, _handle;
        
        // Start is called before the first frame update
        void Start()
        {
            DOTween.Init();
            _levelNumber = PlayerPrefs.GetInt("LevelNumber", 1);
            levelText.text =  "LEVEL " + _levelNumber.ToString();
            _coinNumber = PlayerPrefs.GetInt("CoinNumber", 0);
            coinText.text =  _coinNumber.ToString();
            ScoreManager.Instance.coinNumber = _coinNumber;
            if(_levelNumber!=1)
            {
                HoldAndDrag.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
        public void IncreaseLevelNumberText()
        {
            _levelNumber++;
            PlayerPrefs.SetInt("LevelNumber", _levelNumber);
        }
        
        public IEnumerator DoStartUIChanges()
        {
            yield return new WaitForSeconds(1.5f);
            HoldAndDrag.GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(2.5f);
            ChangeActivityOfObjects();
            PrepareTarzanHead();
            yield return new WaitForSeconds(1f);
            HoldAndDrag.SetActive(false);
        }
        private void ChangeActivityOfObjects()
        {
            levelText.gameObject.SetActive(false);
            tarzanWay.transform.parent.gameObject.SetActive(true);
            tarzanHealthImage.transform.parent.gameObject.SetActive(true);
            coinText.transform.parent.gameObject.SetActive(true);
        }
        private void PrepareTarzanHead()
        {
            _tarzan = GameObject.FindGameObjectWithTag("Tarzan");
            _handle = GameObject.FindGameObjectWithTag("Handle");
            _tarzanStartZ = _tarzan.transform.position.z;
            _differenceBetweenHandleAndTarzan = _handle.transform.position.z - _tarzan.transform.position.z;
            StartCoroutine(StartCalculateTarzanUIPosition());
        }
        private IEnumerator StartCalculateTarzanUIPosition()
        {
            tarzanWay.fillAmount = (float)(_tarzan.transform.position.z - _tarzanStartZ) / _differenceBetweenHandleAndTarzan;
            tarzanHead.rectTransform.localPosition= new Vector3(
                ((float)(_tarzan.transform.position.z - _tarzanStartZ) / _differenceBetweenHandleAndTarzan) * 470,
                tarzanHead.transform.localPosition.y,tarzanHead.transform.localPosition.z);
            yield return new WaitForEndOfFrame();
            StartCoroutine(StartCalculateTarzanUIPosition());
        }
        public void AssignIncreaseValue(float value)
        {
            _decreaseValuePerShot = value;
        }
        public void ChangeHealthScrollValue()
        {
            DOTween.To(x => tarzanHealthImage.fillAmount = x, tarzanHealthImage.fillAmount, tarzanHealthImage.fillAmount - _decreaseValuePerShot, 0.5f);
        }
        public IEnumerator CloseGamePlayCanvas()
        {
            yield return new WaitForSeconds(1);
            GamePlayCanvas.SetActive(false);
        }
        public IEnumerator OpenLoseCanvas()
        {
            yield return new WaitForSeconds(2);
            RUIPanel.Open("Lose");
        }
        public void UpdateCoinText(int value)
        {
            _coinNumber = value;
            coinText.text = _coinNumber.ToString();
            PlayerPrefs.SetInt("CoinNumber", _coinNumber);
        }
        public IEnumerator OpenSwerve()
        {
            if (_levelNumber == 1)
            {
                yield return new WaitForSeconds(1);
                Swerve.gameObject.SetActive(true);
                yield return new WaitForSeconds(2);
                Swerve.gameObject.SetActive(false);
            }
        }
        public IEnumerator OpenSwipeUp()
        {
            if (_levelNumber == 1)
            {
                SwipeUp.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                SwipeUp.gameObject.SetActive(false);
            }
        }
        public IEnumerator OpenSwipeDown()
        {
            if (_levelNumber == 1)
            {
                SwipeDown.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                SwipeDown.gameObject.SetActive(false);
            }
        }
    }
}