using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC.Pedestrian
{
    public class PedestrianAnimatorController : MonoBehaviour
    {
        //--------------------SERIALIZED VARIABLES-------------------
        [SerializeField] private Animator _animator;

        //--------------------PRIVATE VARIABLES-------------------
        private readonly static int AnimParam_Idle = Animator.StringToHash("idle");
        private readonly static int AnimParam_Walk = Animator.StringToHash("walk");
        private readonly static int AnimParam_Run = Animator.StringToHash("run");


        public void SetToIdle()
        {
            _animator.ResetTrigger(AnimParam_Idle);
            _animator.SetTrigger(AnimParam_Idle);
        }

        public void SetToWalk()
        {
            _animator.ResetTrigger(AnimParam_Walk);
            _animator.SetTrigger(AnimParam_Walk);
        }

        public void SetToRun()
        {
            _animator.ResetTrigger(AnimParam_Run);
            _animator.SetTrigger(AnimParam_Run);
        }
    }
}

