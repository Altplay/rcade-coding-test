using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC.Core;

namespace NPC.Pedestrian
{
    public class PedestrianState_Flee : NPCState
    {
        private PedestrianStateController _stateController;

        private float _fleeDistance = 15f;
        private float _stopDistance = 1f;
        private Vector3 _fleeDestination;

        public PedestrianState_Flee(PedestrianStateController stateController)
        {
            _stateController = stateController;
        }

        public override void OnEnter()
        {
            Vector3 fleeDirection = (_stateController.transform.position - _stateController.detectedDogPosition);
            _fleeDestination = _stateController.transform.position + fleeDirection.normalized * _fleeDistance;
            _fleeDestination.y = _stateController.transform.position.y;

            _stateController.pointer.SetPositionAndRotation(_fleeDestination, Quaternion.identity);

            _stateController.ChangeSpeed(true);
            _stateController.animatorController.SetToRun();
        }

        public override void HandleUpdate()
        {
            Vector3 directionToDestination = _fleeDestination - _stateController.transform.position;

            Vector3 destination = _stateController.transform.position;
            Quaternion targetRotation = directionToDestination != Vector3.zero ?
                Quaternion.Slerp(_stateController.transform.rotation, Quaternion.LookRotation(directionToDestination), _stateController.rotationSpeed * Time.deltaTime) :
                _stateController.transform.rotation;

            if (directionToDestination.sqrMagnitude <= _stopDistance)
            {
                _stateController.ChangeSpeed();
                _stateController.patrolMover.SetNearestWaypointAsCurrent();
                _stateController.BeginIdle();
            }
            else
                destination += directionToDestination.normalized * _stateController.currentSpeed * Time.deltaTime;

            _stateController.transform.SetPositionAndRotation(destination, targetRotation);
        }

        public override void OnExit()
        {

        }
    }
}
