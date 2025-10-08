using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{

    public class LevelDisplay : MonoBehaviour
    {
        BaseStats level;
        Text hpText;
        void Awake()
        {
            level = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            hpText = GetComponent<Text>();
        }

        void Update()
        {
            if (level == null)
            {
                hpText.text = "N/A";
            }
            else
            {
                hpText.text = (int)level.GetLevel() + "";
            }
        }
    }
}
