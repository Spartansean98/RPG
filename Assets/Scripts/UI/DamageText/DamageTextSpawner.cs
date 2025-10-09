using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText hitTextPrefab;
        [SerializeField] DamageText criticalHitTextPrefab;
        public void Spawn(float damage)
        {
            DamageText damageText = Instantiate<DamageText>(hitTextPrefab, transform);
            damageText.SetValue(damage);
        }
        public void SpawnCriticalHit(float damage)
        {
            DamageText damageText = Instantiate<DamageText>(criticalHitTextPrefab, transform);
            damageText.SetValue(damage);
        }
    }
}
