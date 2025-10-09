using RPG.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float speed = 1f;
        [SerializeField] bool heatSeeking = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 1f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] UnityEvent projectileHit = null;
        [SerializeField] GameObject spellHead = null;
        float damage = 0;

        Health target = null;

        GameObject instigator = null;

        bool hasTarget = false;

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if (!hasTarget || heatSeeking)
            {
                if (!target.IsDead())
                {
                    transform.LookAt(GetAimLocation());
                    hasTarget = true;
                }
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        public void SetTarget(Health target, float damage,GameObject instigator)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        void OnTriggerEnter(Collider other)
        {
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
            {
                if (targetHealth.IsDead()) return;
                
                if (targetHealth != instigator.GetComponent<Health>())
                {
                    projectileHit.Invoke();
                    DisbleMeshAndCollider();

                    speed = 0;
                    targetHealth.TakeDamage(instigator, damage);

                    if (hitEffect != null)
                    {
                        Instantiate(hitEffect, transform.position, transform.rotation);

                    }
                    foreach (GameObject toDestroy in destroyOnHit)
                    {
                        Destroy(toDestroy);
                    }
                    Destroy(gameObject, 2);
                }
            }
            if (other.tag.Equals("Environment"))
            {
                projectileHit.Invoke();
                DisbleMeshAndCollider();
                speed = 0;
                foreach (GameObject toDestroy in destroyOnHit)
                {
                    Destroy(toDestroy);
                }
                
                Destroy(gameObject, 2);
            }
        }

        private void DisbleMeshAndCollider()
        {
            if (GetComponentInChildren<MeshRenderer>() != null)
            {
                GetComponent<Collider>().enabled = false;
                GetComponentInChildren<MeshRenderer>().enabled = false;
                //print("Disable mesh");
            }
            else
            {
                spellHead.SetActive(false);
            }
        }
    }
}