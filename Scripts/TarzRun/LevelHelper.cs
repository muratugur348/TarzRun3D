using TarzRun.Controllers;
using System.Collections;
using System.Collections.Generic;
using Core.Patterns.Creational;
using UnityEngine;
using TarzRun.Managers;
using TMPro;
using TarzRun.UI;

namespace TarzRun
{
    public class LevelHelper : Singleton<LevelHelper>
    {
        public int levelIndex;
        public int tarzanHealth;
        private void Start()
        {
            ScoreManager.Instance.tarzanHealth = tarzanHealth;
            UIManager.Instance.AssignIncreaseValue(1 / (float)tarzanHealth);
        }
    }
}