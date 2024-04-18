using System.Collections.Generic;
using UnityEngine;
using NPC.Core;

namespace NPC.Pedestrian
{
    public class PedestrianStateController : MonoBehaviour
    {
        //----------------PUBLIC VARIABLES---------------------------
        public WaypointMover patrolMover;
        public Transform pointer;
        public PedestrianAnimatorController animatorController => _animatorController;
        public Vector3 detectedDogPosition => _detectedDogPosition;
        public float currentSpeed => _currentSpeed;
        public float rotationSpeed => _rotationSpeed;
        public bool canMove => _canMove;

        //----------------SERIALIZED VARIABLES---------------------------
        [SerializeField] private PedestrianAnimatorController _animatorController;
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private CollisionMessageRelay _dogsDetectorTrigger;


        //----------------PRIVATE VARIABLES---------------------------
        private Vector3 _detectedDogPosition;
        private float _currentSpeed;

        private bool _canMove = true;

        private PedestrianState_Idle state_Idle;
        private PedestrianState_Patrol state_Patrol;
        private PedestrianState_Flee state_Flee;

        private Dictionary<PedestrianStateEnums, NPCState> _statesLookup;

        private PedestrianStateEnums _currentStateEnum;
        private NPCState _currentState;

        private void Awake()
        {
            state_Idle = new PedestrianState_Idle(this);
            state_Patrol = new PedestrianState_Patrol(this);
            state_Flee = new PedestrianState_Flee(this);

            _statesLookup = new Dictionary<PedestrianStateEnums, NPCState>();
            _statesLookup.Add(PedestrianStateEnums.Idle, state_Idle);
            _statesLookup.Add(PedestrianStateEnums.Patrol, state_Patrol);
            _statesLookup.Add(PedestrianStateEnums.Flee, state_Flee);

            _currentSpeed = _walkSpeed;

            patrolMover.IdleEnteredEvent += OnIdleEntered;

            if(_dogsDetectorTrigger != null)
                _dogsDetectorTrigger.TriggerEnterEvent += OnDogsDetected;
        }

        private void OnDogsDetected(CollisionMessageRelay sender, Collider detectedCollider)
        {
            if(detectedCollider.tag == "Dog")
            {
                _detectedDogPosition = detectedCollider.transform.position;
                ChangeState(PedestrianStateEnums.Flee);
            }
        }

        private void Start()
        {
            BeginPatrol();
        }

        private void Update()
        {
            if (_currentState != null)
                _currentState.HandleUpdate();
        }

        public void ChangeState(PedestrianStateEnums newStateEnum)
        {
            if (_statesLookup.TryGetValue(newStateEnum, out NPCState newState))
            {
                if (_currentState != null)
                    _currentState.OnExit();

                _currentState = newState;
                _currentStateEnum = newStateEnum;

                _currentState.OnEnter();
            }
        }

        public void BeginPatrol()
        {
            _canMove = true;
            ChangeState(PedestrianStateEnums.Patrol);
        }

        public void BeginIdle()
        {
            _canMove = false;

            ChangeState(PedestrianStateEnums.Idle);
        }

        public void ChangeSpeed(bool toRunSpeed = false)
        {
            _currentSpeed = toRunSpeed ? _runSpeed : _walkSpeed;
        }

        private void OnIdleEntered()
        {
            BeginIdle();
        }
    }
}

