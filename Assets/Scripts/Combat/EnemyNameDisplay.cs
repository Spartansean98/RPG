using UnityEngine;
using UnityEngine.UI;
namespace RPG.Combat
{

    public class EnemyNameDisplay : MonoBehaviour
    {
        GameObject enemy;
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
                hpText.text = "Enemy: ";
                return;
            }
            else
            {
                enemy = player.GetTarget();
                hpText.text = enemy.name + ": ";
            }
        }
    }

}