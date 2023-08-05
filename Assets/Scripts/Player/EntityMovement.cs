using Manager;
using UnityEngine;

namespace Player
{
    public class EntityMovement : MonoBehaviour
    {
        #region PUBLIC FIELDS

        [SerializeField] private float speed;

        #endregion

        #region UNITY METHODS

        private void Update()
        {
            if (StateManager.Instance.currentState != GameState.OnPlay) return;
            Move();
        }


        #endregion

        #region PRIVATE METHODS

        private void Move()
        {
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }

        #endregion
    }
}