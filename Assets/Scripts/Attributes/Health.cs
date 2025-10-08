using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine;
using RPG.Utils;
namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [Range(1,100)]
        [SerializeField] float levelUpRestorePercent = 80;
        LazyValue<float> hp;  
        bool isDead = false;

        private void Awake()
        {
            hp = new LazyValue<float>(GetInitialHeath);
        }
        void Start()
        {
            hp.ForceInit();
        }
        private float GetInitialHeath()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += LevelUpRestoreHealth;
        }
        void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= LevelUpRestoreHealth;
        }
        public void LevelUpRestoreHealth()
        {
            if (GetPercentage() < levelUpRestorePercent)
            {
                hp.value = GetComponent<BaseStats>().GetStat(Stat.Health) * (levelUpRestorePercent / 100);
            }
        }
        public bool IsDead()
        {
            return isDead;
        }
        public void TakeDamage(GameObject instigator,float damage)
        {
            print(gameObject.name + " took damage : " + damage);
            hp.value = Mathf.Max(hp.value - damage, 0);
            if (hp.value == 0)
            {
                AwardExperience(instigator);
                Death();
            }
        }

        public float GetPercentage()
        {
            return hp.value / GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
        }

        public float GetHealthPoints()
        {
            return hp.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public void Death()
        {
            if (isDead) return;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            Destroy(GetComponent<CapsuleCollider>());

            isDead = true;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
           
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            
        }

        public object CaptureState()
        {
            return hp;
        }

        public void RestoreState(object state)
        {
            hp.value = (float)state;
            if (hp.value <= 0)
            {
                Death();
            }
        }
    }
}