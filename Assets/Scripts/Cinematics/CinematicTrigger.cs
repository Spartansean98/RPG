using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool triggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if (!triggered && other.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                triggered = true;
            }
        }
    }
}