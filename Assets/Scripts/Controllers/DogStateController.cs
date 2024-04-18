using System.Collections.Generic;
using UnityEngine;
using NPC.Core;
using Sirenix.OdinInspector;

namespace NPC.Dogs
{
    public class DogStateController : MonoBehaviour
    {
        //----------------PUBLIC VARIABLES---------------------------
        public WaypointMover patrolMover;

        public DogAnimatorController animatorController => _animatorController;
        public float currentStamina => _currentStamina;
        public float currentSpeed => _currentSpeed;
        public float rotationSpeed => _rotationSpeed;
        public bool canMove => _canMove;
        public bool useStamina => _useStamina;

        //----------------SERIALIZED VARIABLES---------------------------
        [SerializeField] private DogAnimatorController _animatorController;
        [SerializeField] private bool _useStamina = false;
        [SerializeField, ShowIf(nameof(_useStamina))] private float _maxStamina = 50f;
        [SerializeField, ShowIf(nameof(_useStamina))] private float _staminaConsumptionRate = 0.05f;
        [SerializeField, ShowIf(nameof(_useStamina))] private float _staminaRestorationRate = 0.12f;
        [SerializeField, HideIf(nameof(_useStamina))] private float _walkSpeed = 5f;
        [SerializeField, ShowIf(nameof(_useStamina))] private float _runSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 10f;

        //----------------PRIVATE VARIABLES---------------------------
        private float _currentStamina;
        private float _currentSpeed;

        private bool _canMove = true;

        private DogState_Idle dogState_Idle;
        private DogState_PatrolWalk dogState_PatrolWalk;
        private DogState_PatrolRun dogState_PatrolRun;


        private Dictionary<DogStateEnums, NPCState> _statesLookup;

        private DogStateEnums _currentStateEnum;
        private NPCState _currentState;


        private void Awake()
        {
            dogState_Idle = new DogState_Idle(this);
            dogState_PatrolWalk = new DogState_PatrolWalk(this);
            dogState_PatrolRun = new DogState_PatrolRun(this);

            _statesLookup = new Dictionary<DogStateEnums, NPCState>();
            _statesLookup.Add(DogStateEnums.Idle, dogState_Idle);
            _statesLookup.Add(DogStateEnums.PatrolWalk, dogState_PatrolWalk);
            _statesLookup.Add(DogStateEnums.PatrolRun, dogState_PatrolRun);

            _currentStamina = _maxStamina;
            _currentSpeed = _useStamina ? _runSpeed : _walkSpeed;

            patrolMover.IdleEnteredEvent += OnIdleEntered;
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

        public void ChangeState(DogStateEnums newStateEnum)
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
            ChangeState(_useStamina ? DogStateEnums.PatrolRun : DogStateEnums.PatrolWalk);
        }

        public void BeginIdle()
        {
            _canMove = false;

            if (_useStamina)
                _currentStamina = 0;

            ChangeState(DogStateEnums.Idle);
        }

        public void RestoreStamina()
        {
            _currentStamina += _staminaRestorationRate;

            if (_currentStamina >= _maxStamina)
            {
                _currentStamina = _maxStamina;

                BeginPatrol();
            }
        }

        public void ConsumeStamina()
        {
            _currentStamina -= _staminaConsumptionRate;
        }

        private void OnIdleEntered()
        {
            if (_useStamina)
                patrolMover.PickNextWaypoint();
            else
                BeginIdle();
        }
    }
}

