using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Serializable]
    public class PoolObject
    {
        public string tag;
        public Transform prefab;
        public int size;
    }

    [Header("Pool Settings")]
    [SerializeField] private List<PoolObject> object_list = new List<PoolObject>();

    private Dictionary<string, List<Transform>> pool_dictionary = new();
    public List<Transform> in_pool_list { get; private set; } = new();
    public List<Transform> spawned_list { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var pool in object_list)
        {
            var objectPool = new List<Transform>();
            for (int i = 0; i < pool.size; i++)
            {
                Transform obj = Instantiate(pool.prefab, transform);
                obj.gameObject.SetActive(false);
                objectPool.Add(obj);
                in_pool_list.Add(obj);
            }
            pool_dictionary[pool.tag] = objectPool;
        }
    }

    public Transform SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!pool_dictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Transform objectToSpawn = pool_dictionary[tag].Find(t => !t.gameObject.activeInHierarchy);

        // If all objects are active, create a new one dynamically
        if (objectToSpawn == null)
        {
            var pool = object_list.Find(p => p.tag == tag);
            if (pool == null)
            {
                Debug.LogError($"No prefab found for tag {tag}");
                return null;
            }

            objectToSpawn = Instantiate(pool.prefab, transform);
            pool_dictionary[tag].Add(objectToSpawn);
            in_pool_list.Add(objectToSpawn);
        }

        objectToSpawn.gameObject.SetActive(true);
        objectToSpawn.position = position;
        objectToSpawn.rotation = rotation;
        if (parent != null) objectToSpawn.SetParent(parent);

        spawned_list.Add(objectToSpawn);
        in_pool_list.Remove(objectToSpawn);

        return objectToSpawn;
    }

    public void Despawn(Transform obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        obj.SetParent(transform);

        if (!in_pool_list.Contains(obj))
            in_pool_list.Add(obj);

        if (spawned_list.Contains(obj))
            spawned_list.Remove(obj);
    }

    public void ReturnAllObjectsToPool()
    {
        List<Transform> toReturn = new List<Transform>(spawned_list);
        foreach (var obj in toReturn)
        {
            if (obj != null)
                Despawn(obj);
        }
    }
}