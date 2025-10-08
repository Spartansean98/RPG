using UnityEngine;
using UnityEngine.UI;
namespace RPG.Stats
{

    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        Text hpText;
        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            hpText = GetComponent<Text>();
        }

        void Update()
        {
            if (experience == null)
            {
                hpText.text = "N/A";
            }
            else
            {
                hpText.text = (int)experience.GetExperience() + " xp";
            }
        }
    }
}
