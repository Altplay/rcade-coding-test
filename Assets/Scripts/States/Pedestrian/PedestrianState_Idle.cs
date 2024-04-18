using NPC.Core;

namespace NPC.Pedestrian
{
    public class PedestrianState_Idle : NPCState
    {
        private PedestrianStateController _stateController;

        public PedestrianState_Idle(PedestrianStateController stateController)
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
            _stateController.patrolMover.IdleUpdate();
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
