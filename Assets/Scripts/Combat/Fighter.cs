using UnityEngine;
using RPG.Movement;
using RPG.Core;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {

        [Range(0,1)]
        [SerializeField] float chaseSpeedFraction = 1f;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransfrom;
        [SerializeField] Weapon defaultweapon = null;
        Health target;

        Weapon currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;
        void Start()
        {
            EquipWeapon(defaultweapon);
        }
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null || target.IsDead())
            {
                StopAttackAnimation();
                return;
            }
            if (!InRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position,chaseSpeedFraction);

            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            currentWeapon = weapon;
            weapon.Spawn(rightHandTransform,leftHandTransfrom, GetComponent<Animator>());
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= currentWeapon.GetTimeBetweenAttack())
            {
                TriggerAttack();

                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        void Hit()//animation event
        {
            if (target == null) return;
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(leftHandTransfrom, rightHandTransform, target ,GetComponent<Health>());
            }
            else
            {
            target.TakeDamage(currentWeapon.GetDamage());
            }

        }
        void Shoot()
        {
            Hit();
        }
        private bool InRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            
        }
        public bool CanAttack(GameObject targeted)
        {
            if (targeted == null){ return false; }
            Health targetToTest = targeted.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        public void Cancel()
        {
            target = null;
            StopAttackAnimation();
            GetComponent<Mover>().Cancel();

        }

        private void StopAttackAnimation()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

    }
}