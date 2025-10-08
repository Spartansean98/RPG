using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        const string sceneIndexString = "lastSceneBuildIndex";
        public IEnumerator LoadLastScene(string saveFile)
        {
            int sceneIndex = (int)SceneManager.GetActiveScene().buildIndex;
            Dictionary<string, object> state = LoadFile(saveFile);
            if (state.ContainsKey(sceneIndexString))
            {
                sceneIndex = (int)state[sceneIndexString];
            }
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            RestoreState(state);
        }
        public void Save(string saveFile)
        {

            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }
        public void Delete(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            File.Delete(path);
        }
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
            
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            state[sceneIndexString] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (SaveableEntity saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(stateDict[id]);
                }
            }
            }
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}