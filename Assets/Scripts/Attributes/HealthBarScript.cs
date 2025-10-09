using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBarScript : MonoBehaviour
    {
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Health health = null;
        [SerializeField] Canvas canvas = null;
        void Update()
        {
            float fraction = health.GetFraction();
            if (Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
            {
                canvas.enabled = false;
                return;
            }
            else
            {
                canvas.enabled = true;
            }
            foreground.localScale =new Vector3(fraction, 1 );
        }
    }
}
