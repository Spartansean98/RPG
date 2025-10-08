using System;
using RPG.Core;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

namespace RPG.Combat{

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] float weaponDamage = 1f;
        [SerializeField] bool isRightHand = true;
        [SerializeField] Projectile projectile = null;
        const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if (weaponPrefab != null)
            {
                Transform hand = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(weaponPrefab, hand);
                weapon.name = weaponName;
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
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target,Health archer)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target,weaponDamage,archer);
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
    }
}