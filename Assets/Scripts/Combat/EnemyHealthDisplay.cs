using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.Combat
{

    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health enemyHealth;
        Fighter player;
        Text hpText;
        void Awake()
        {

            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            hpText = GetComponent<Text>();
        }

        void Update()
        {
            if (player.GetTarget() == null)
            {
                hpText.text = "N/A";
                return;
            }
            else
            {
                enemyHealth = player.GetTarget().GetComponent<Health>();
                hpText.text = (int)enemyHealth.GetHealthPoints() + "/"+ enemyHealth.GetMaxHealthPoints();
            }
        }
    }

}