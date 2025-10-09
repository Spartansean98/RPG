using System;
using System.Collections;
using Control;
using RPG.Attributes;
using RPG.Control;
using TMPro;
using UnityEngine;
namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour,IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float respawnTime = 5f;
        [SerializeField] float healthToRestore = 0;
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if(healthToRestore>0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
            
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool show)
        {
            GetComponent<Collider>().enabled = show;
            foreach (Transform transform in GetComponentInChildren<Transform>())
            {
                transform.gameObject.SetActive(show);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
            
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}