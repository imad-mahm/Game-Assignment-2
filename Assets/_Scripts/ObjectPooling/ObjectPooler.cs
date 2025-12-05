using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int PoolSize;
    }

    public static Dictionary<string, Queue<GameObject>> PoolDictionary;
    public List<Pool> pools;
    public static ObjectPooler Instance;

    private void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            var objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.PoolSize; i++)
            {
                var obj = Instantiate(pool.Prefab);
                obj.SetActive(false);

                objectPool.Enqueue(obj);
            }

            PoolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public static GameObject GetFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist.");
            return null;
        }

        var objectToSpawn = PoolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rot;

        var pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        PoolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public static void ReleaseFromPool(GameObject obj, float delay = 0f)
    {
        if (delay <= 0)
        {
            obj.SetActive(false);
        }
        else
        {
            Instance.StartCoroutine(Instance.ReleaseAfterDelay(obj, delay));
        }
    }
    
    private IEnumerator ReleaseAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
