using System.Collections;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.5f;
        [SerializeField] float fadeWaitTime = 0.5f;
        void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        
        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);

            Fader fader = FindFirstObjectByType<Fader>();
            fader.FadeOutImmediate();

            yield return new WaitForSeconds(fadeWaitTime);

            yield return fader.FadeIn(fadeInTime);
        }
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();

            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                print("Deleted Save File");
                Delete();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }

        private void Reload()
        {
            StartCoroutine(ReloadScene());
        }

        private IEnumerator ReloadScene()
        {
            Fader fader = FindAnyObjectByType<Fader>();
            yield return fader.FadeOut(fadeWaitTime);
            yield return SceneManager.LoadSceneAsync(0);
            fader.FadeIn(fadeInTime);
        }

        public void Load()
        {
            
            GetComponent<SavingSystem>().Load(defaultSaveFile);
           
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
    }
}