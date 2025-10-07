using UnityEngine;
namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawned = false;
        private void Awake()
        {
            if (hasSpawned) return;
            SpawnPersitentObjects();
            hasSpawned = true;
        }

        private void SpawnPersitentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
            
        }
    }
}