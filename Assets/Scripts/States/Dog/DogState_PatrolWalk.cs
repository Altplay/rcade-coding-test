using UnityEngine;
using NPC.Core;

namespace NPC.Dogs
{
    public class DogState_PatrolWalk : NPCState
    {
        private DogStateController _stateController;

        public DogState_PatrolWalk(DogStateController stateController)
        {
            _stateController = stateController;
        }

        public override void OnEnter()
        {
            _stateController.animatorController.SetToWalk();
        }

        public override void HandleUpdate()
        {
			_stateController.patrolMover.UpdateMover();

            Transform currentWaypoint = _stateController.patrolMover.GetCurrentWaypoint();

			if (currentWaypoint == null)
				return;

			Vector3 directionToCurrentWaypoint = currentWaypoint.position - _stateController.transform.position;
			Vector3 destination = _stateController.transform.position;
			Quaternion targetRotation = directionToCurrentWaypoint != Vector3.zero ?
				Quaternion.Slerp(_stateController.transform.rotation, Quaternion.LookRotation(directionToCurrentWaypoint), _stateController.rotationSpeed * Time.deltaTime) :
                _stateController.transform.rotation;

			if (directionToCurrentWaypoint.sqrMagnitude <= Mathf.Pow(_stateController.patrolMover.pickNextWaypointDistance, 2))
				destination = currentWaypoint.position;
			else
				destination += directionToCurrentWaypoint.normalized * _stateController.currentSpeed * Time.deltaTime;

            _stateController.transform.SetPositionAndRotation(destination, targetRotation);
		}

        public override void OnExit()
        {
            
        }
    }
}

