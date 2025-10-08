using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable,IModifierProvider
    {

        [Range(0,1)]
        [SerializeField] float chaseSpeedFraction = 1f;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransfrom;
        [SerializeField] Weapon defaultweapon = null;
        Health target;

        LazyValue<Weapon> currentWeapon;
        float timeSinceLastAttack = Mathf.Infinity;
        void Awake()
        {
            currentWeapon= new LazyValue<Weapon>(SetUpDefaultWeapon);
        }
        Weapon SetUpDefaultWeapon()
        {
            AttachWeapon(defaultweapon);
            return defaultweapon;
        }
        void Start()
        {
            currentWeapon.ForceInit();
        }

        private float GetInitialWeapon()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
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
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            weapon.Spawn(rightHandTransform, leftHandTransfrom, GetComponent<Animator>());
        }

        public GameObject GetTarget()
        {
            if (target == null) return null;
            return target.gameObject;
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= currentWeapon.value.GetTimeBetweenAttack())
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
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(leftHandTransfrom, rightHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }

        }
        void Shoot()
        {
            Hit();
        }
        private bool InRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.value.GetRange();
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

        public object CaptureState()
        {
            if (currentWeapon == null) return "Unarmed";
            return currentWeapon.value.name;

        }

        public void RestoreState(object weaponState)
        {
            Weapon weapon = Resources.Load<Weapon>((string)weaponState);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonusDamage();
            }
        }
    }
}