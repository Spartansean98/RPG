using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;
using UnityEngine.Events;
using Unity.VisualScripting;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable,IModifierProvider
    {

        [Range(0,1)]
        [SerializeField] float chaseSpeedFraction = 1f;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransfrom;
        [SerializeField] WeaponConfig defaultWeapon = null;
        Health target;

        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        float timeSinceLastAttack = Mathf.Infinity;
        void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }
        Weapon SetUpDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
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
            if (!InRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, chaseSpeedFraction);

            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransfrom, GetComponent<Animator>());
        }

        public GameObject GetTarget()
        {
            if (target == null) return null;
            return target.gameObject;
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= currentWeaponConfig.GetTimeBetweenAttack())
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
            if(currentWeapon.value!=null)
            {
                currentWeapon.value.OnHit();
            }
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(leftHandTransfrom, rightHandTransform, target, gameObject, damage);
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
        private bool InRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.transform.position) <= currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            
        }
        public bool CanAttack(GameObject targeted)
        {
            if (targeted == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(targeted.transform.position)&& InRange(targeted.transform)) return false;
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
            if (currentWeaponConfig == null) return "Unarmed";
            return currentWeaponConfig.name;

        }

        public void RestoreState(object weaponState)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string)weaponState);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonusDamage();
            }
        }
    }
}