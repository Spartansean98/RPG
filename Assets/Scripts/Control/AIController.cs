using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using RPG.Utils;
namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTimer = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] float waypointDwellTime = 1f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        Fighter fighter;
        Mover mover;
        Health health;
        GameObject player ;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArriveWaypoint = Mathf.Infinity;

        LazyValue<Vector3> guardPosition;
        int waypointNo = 0;
        void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            player = GameObject.FindGameObjectWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }
        Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }
        void Start()
        {
            guardPosition.ForceInit();
        }

        void Update()
        {

            if (health.IsDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();

            }
            else if (timeSinceLastSawPlayer < suspicionTimer)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
               
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArriveWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArriveWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArriveWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
        }

        private void CycleWaypoint()
        {
            waypointNo = patrolPath.GetNextIndex(waypointNo);
        }

        private bool AtWayPoint()
        {
            
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(waypointNo);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        //called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.aliceBlue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
