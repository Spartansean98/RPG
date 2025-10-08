using UnityEngine;
using UnityEngine.UI;
namespace RPG.Attributes
{

    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text hpText;
        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            hpText = GetComponent<Text>();
        }

        void Update()
        {
            hpText.text = (int)health.GetHealthPoints()+"/"+health.GetMaxHealthPoints();
        }
    }

}