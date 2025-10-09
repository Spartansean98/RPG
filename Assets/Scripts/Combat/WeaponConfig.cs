using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat{

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] float weaponDamage = 0;
        [SerializeField] float weaponPercentageBonus = 1f;
        [SerializeField] bool isRightHand = true;
        [SerializeField] Projectile projectile = null;
        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            Weapon weapon = null;
            DestroyOldWeapon(rightHand, leftHand);
            if (weaponPrefab != null)
            {
                Transform hand = GetTransform(rightHand, leftHand);
                weapon = Instantiate(weaponPrefab, hand);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform hand;
            if (isRightHand) hand = rightHand;
            else hand = leftHand;
            return hand;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instagator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, calculatedDamage, instagator);
        }
        public float GetRange()
        {
            return weaponRange;
        }
        public float GetTimeBetweenAttack()
        {
            return timeBetweenAttack;
        }
        public float GetDamage()
        {
            return weaponDamage;
        }
        public float GetPercentageBonusDamage()
        {
            return weaponPercentageBonus;
        }
    }
}