using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour,IAction,ISaveable
    {
        [SerializeField] float maxSpeed = 6.0f;
        [SerializeField] float maxNavPathLength = 40f;
        NavMeshAgent navMeshAgent;
        Health health;
        void Awake()
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
        public void StartMoveAction(Vector3 dest, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            MoveTo(dest, speedFraction);
        }
        public bool CanMoveTo(Vector3 dest)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;
            
            return true;
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

        private float GetPathLength(NavMeshPath path)
        {
            float distance = Vector3.Distance(transform.position, path.corners[0]);
            for (int i = 1; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            //print("Path Distance: " + (int)distance);
            return distance; ;
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