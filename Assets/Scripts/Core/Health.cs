using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
namespace RPG.Core
{
    public class Health : MonoBehaviour,ISaveable
    {
        //[SerializeField] float maximumHealth = 20f;
        [SerializeField] float hp = 20f;
        bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }
        public void TakeDamage(float damage)
        {
            hp = Mathf.Max(hp - damage, 0);
            if (hp == 0)
            {
                Death();
            }
        }

        public void Death()
        {
            if (isDead) return;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            Destroy(GetComponent<CapsuleCollider>());

            isDead = true;
        }

        public object CaptureState()
        {
            return hp;
        }

        public void RestoreState(object state)
        {
            hp = (float)state;
            if (hp <= 0)
            {
                Death();
            }
        }
    }
}