using System;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Events;
namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        public event Action onLevelUp;
        [SerializeField] UnityEvent levelUp;
        LazyValue<int> currentLevel;

        Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        void Start()
        {
            currentLevel.ForceInit();
        }
        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }
        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }
        void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                levelUp.Invoke();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return GetBaseStat(stat) + GetAdditiveModifier(stat) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {

                    total += modifier;                    
                }

            }
            return total;
        }
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperience();
            int penultimateLevel = progression.GetLevels(Stat.LevelUpExperience, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float levelUpXP = progression.GetStat(Stat.LevelUpExperience, characterClass, level);
                if (levelUpXP > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}