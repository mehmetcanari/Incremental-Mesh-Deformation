using System;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Interactions
{
    public class EntityInteraction : MonoBehaviour
    {
        #region PUBLIC FIELDS

        public UnityEvent onInteract;

        #endregion
        
        #region UNITY METHODS

        private void Awake()
        {
            onInteract.AddListener(() =>
            {
                EntityInteract();
                DestroyComponent();
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.TryGetComponent(out IInteractable otherInteractable)) return;
            otherInteractable.Interact();
            onInteract.Invoke();
        }

        #endregion
        
        #region PRIVATE METHODS

        private void EntityInteract()
        {
            StateManager.Instance.ChangeState(GameState.OnEnd);
        }
        
        private void DestroyComponent() => Destroy(this);

        #endregion
        
    }
}