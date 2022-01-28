using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }
        public static ObjectPooler Instance;
        private void Awake()
        {
            Instance = this;
            CreatePools();
        }
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        private void CreatePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                Transform poolObj = new GameObject(pool.tag + " Pool").transform;
                poolObj.transform.position = Vector3.up * 100;
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, poolObj);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                poolDictionary.Add(pool.tag, objectPool);
            }
        }
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }
            GameObject objectToSpawn;
            objectToSpawn = poolDictionary[tag].Dequeue();
            poolDictionary[tag].Enqueue(objectToSpawn);
            int i = 0;
            int c = poolDictionary[tag].Count;
            while (objectToSpawn.activeSelf && i++ < c)
            {
                objectToSpawn = poolDictionary[tag].Dequeue();
                poolDictionary[tag].Enqueue(objectToSpawn);
            }
            objectToSpawn.LeanCancel();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);
            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            if (pooledObj != null)
                pooledObj.OnObjectSpawn();
            return objectToSpawn;
        }
    }
}