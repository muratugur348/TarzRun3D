using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Patterns.Creational;
using UnityEngine.SceneManagement;
using TarzRun.Controllers;

namespace TarzRun.Managers
{
    public class LevelManager : Singleton<LevelManager>
    {
        public LevelHelper currentLevelHelper;
        public bool isGamePlayable;
        public bool isGameStarted;
        public bool isGameFinished;
        public bool isHolding;
        public bool isChasing=false;
        public bool isFailed=false;
        
        private int _levelIndex;
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 60;
            _levelIndex = PlayerPrefs.GetInt("LevelIndex", 1);
            CreateLevelHelper();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void CreateLevelHelper()
        {
            LevelHelper cloneLevel;
            if (_levelIndex <= LevelHelperRepository.LevelHelperCount())
            {
                cloneLevel = Instantiate(LevelHelperRepository.GetLevel(_levelIndex));
            }
            else
            {
                _levelIndex = 1;
                cloneLevel = Instantiate(LevelHelperRepository.GetLevel(_levelIndex));

            }
            currentLevelHelper = cloneLevel;
        }
        public void GetNextLevel()
        {
            _levelIndex++;
            PlayerPrefs.SetInt("LevelIndex", _levelIndex);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
