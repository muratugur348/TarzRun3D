using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.UI;
using TarzRun.Managers;

namespace TarzRun.UI

{
    public class RestartButton : UIButton
    {
        // Start is called before the first frame update
        protected override void DoAction()
        {
            Restart();
        }

        private void Restart()
        {
            LevelManager.Instance.RestartLevel();
        }
    }
}