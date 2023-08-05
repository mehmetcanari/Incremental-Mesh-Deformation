using System;
using UnityEngine;

namespace Manager
{
    public sealed class StateManager : MonoBehaviour
    {
        #region PUBLIC FIELDS

        public static StateManager Instance;

        public GameState currentState;

        #endregion
        

        #region UNITY METHODS

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        #endregion

        #region PUBLIC METHODS

        public void ChangeState(GameState state) => currentState = state;

        #endregion
    }
    
    public enum GameState
    {
        OnBegin,
        OnPlay,
        OnEnd
    }
}