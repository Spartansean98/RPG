using System;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour,IAction,ISaveable
    {
        [SerializeField] float maxSpeed = 6.0f;
        NavMeshAgent navMeshAgent;
        Health health;
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();   
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
        public void StartMoveAction(Vector3 dest,float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
    
            MoveTo(dest,speedFraction);
        }
        public void MoveTo(Vector3 dest, float speedFraction)
        {
            GetComponent<NavMeshAgent>().destination = dest;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }
        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("playerForward", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            GetComponent<NavMeshAgent>().Warp(((SerializableVector3)state).ToVector());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}