using System;
using System.Collections;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
namespace RPG.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A,B,C,D,E,F
        }
        [SerializeField] int scene = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 2f;
        [SerializeField] float fadeWaitTime = 2f;
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }
        private IEnumerator Transition()
        {
            if (scene < 0)
            {
                Debug.LogError("Scene number is not set.");
                yield break;
            }
            DontDestroyOnLoad(gameObject);
            Fader fader = FindAnyObjectByType<Fader>();
            SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();


            
            yield return fader.FadeOut(fadeOutTime);
            savingWrapper.Save();
            yield return SceneManager.LoadSceneAsync(scene);

            savingWrapper.Load();
            Portal OtherPortal = GetOtherPortal();
            UpdatePlayer(OtherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);

        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue; 
                return portal;
            }
            return null;
        }

        private void UpdatePlayer(Portal OtherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(OtherPortal.spawnPoint.position);
            GameObject.FindWithTag("Player").transform.rotation = OtherPortal.spawnPoint.rotation;
        }
    }
}