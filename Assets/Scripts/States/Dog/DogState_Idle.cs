using NPC.Core;

namespace NPC.Dogs
{
    public class DogState_Idle : NPCState
    {
        private DogStateController _stateController;

        public DogState_Idle(DogStateController stateController)
        {
            _stateController = stateController;
        }

        public override void OnEnter()
        {
            _stateController.patrolMover.IdleExitedEvent += OnIdleExited;
            _stateController.animatorController.SetToIdle();
        }

        public override void HandleUpdate()
        {
            if(_stateController.useStamina)
            {
                _stateController.RestoreStamina();
            }
            else
            {
                _stateController.patrolMover.IdleUpdate();
            }
        }

        public override void OnExit()
        {
        }

        private void OnIdleExited()
        {
            _stateController.patrolMover.IdleExitedEvent -= OnIdleExited;
            _stateController.BeginPatrol();
        }
    }
}
