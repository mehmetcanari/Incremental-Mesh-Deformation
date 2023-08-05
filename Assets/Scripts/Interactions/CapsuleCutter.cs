using DynamicMeshCutter;
using UnityEngine;

namespace Interactions
{
    public class CapsuleCutter : MonoBehaviour, IInteractable
    {
        #region INSPECTOR FIELDS

        public Collider boxCollider;

        #endregion

        #region PRIVATE PROPERTIES
        private PlaneBehaviour GetPlaneBehaviour => transform.parent.GetComponent<PlaneBehaviour>();

        #endregion

        #region PUBLIC METHODS

        public void Interact()
        {
            DisableCollider();
            CallCut();
        }

        #endregion

        #region PRIVATE METHODS

        private void CallCut()
        {
            var planeBehaviour = GetPlaneBehaviour;
            
            if (planeBehaviour == null) return;
            planeBehaviour.Cut();
        }
        
        private void DisableCollider()
        {
            boxCollider.enabled = false;
        }

        #endregion
        
    }
}