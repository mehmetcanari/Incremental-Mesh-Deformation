using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class LevelManager : MonoBehaviour
    {
        #region PRIVATE FIELDS

        private int _currentLevel;

        #endregion

        #region UNITY METHIDS

        private void Update()
        {
            Execute();
        }

        #endregion
        
        #region PRIVATE METHODS

        private int GetCurrentLevelIndex() => SceneManager.GetActiveScene().buildIndex;
        
        private void ReloadCurrentScene() => SceneManager.LoadScene(GetCurrentLevelIndex());

        
        private void Execute()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;
            ReloadCurrentScene();
        }
        
        #endregion
    }
}