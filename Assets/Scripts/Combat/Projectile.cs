using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{

    [SerializeField] float speed = 1f;
    [SerializeField] bool heatSeeking = false;
    [SerializeField] GameObject hitEffect = null;
    float damage = 0;

    Health target = null;
    Health archer = null;
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
    public void SetTarget(Health target, float damage, Health archer)
    {
        this.target = target;
        this.damage = damage;
        this.archer = archer;
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
        if (targetHealth == null) return;
        if (targetHealth.IsDead()) return;
        if (targetHealth != archer)
        {
            targetHealth.TakeDamage(damage);
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }

    }
}
