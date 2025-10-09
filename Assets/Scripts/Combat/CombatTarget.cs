using System;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!callingController.GetComponent<Fighter>().CanAttack(gameObject)) return false;

                if (Input.GetMouseButton(0))
                {
                    callingController.GetComponent<Fighter>().Attack(gameObject);
                }
            }
            return true;
        }
    }
}