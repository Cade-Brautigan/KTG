using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

public class SpawnArea : NetworkBehaviour
{

    // FIND A WAY TO TRACK WHEN OBJECTS ARE CONSUMED BY PLAYER AND REFLECT THIS IN COUNT
    // FIX CLASS TO FULLY REFLECT OOP PRINCIPLES

    float radius = 30f;

    [SerializeField] List<SpawnObject> spawnObjects = new List<SpawnObject>(); // represents all SpawnObject types

    [System.Serializable]
    public class SpawnObject {
        public GameObject prefab;
        public int initCount; // The count that should be spawned on game load
        public int count; // Current number of its type spawned
        public int maxCount; // Maximum number of its type spawned
        public float respawnTime; // Time it takes for object.count++
        public float despawnTime; // Lifetime of each object. despawnTime = 0 means object does not despawn
        public bool isRespawning = false;
    }

    [Server]
    private void Start()
    {
        // Spawn the initial count of each item
        foreach (SpawnObject obj in spawnObjects)
        {
            for (int i = 0; i < obj.initCount; i++)
            {
                Spawn(obj);
            }
        }
    }

    [Server]
    private void FixedUpdate()
    {
        // Begin respawn if necessary
        foreach (SpawnObject obj in spawnObjects)
        {
            if (obj.count < obj.maxCount && obj.isRespawning == false)
            {
                StartCoroutine(Respawn(obj));
            }
        }
    }

    [Server]
    private IEnumerator Respawn(SpawnObject obj)
    {
        if (obj.isRespawning) {yield break;}

        obj.isRespawning = true;
        yield return new WaitForSeconds(obj.respawnTime);
        Spawn(obj);
        obj.isRespawning = false;
    }

    [Server]
    private void Spawn(SpawnObject objType)
    {
        if (objType.count < objType.maxCount)
        {
            GameObject newObj = Instantiate(objType.prefab, RandomPointOnCircle(), Quaternion.identity);
            NetworkServer.Spawn(newObj);
            objType.count++;
            // If item despawns, start despawn timer
            if (objType.despawnTime > 0)
            {
                StartCoroutine(Despawn(newObj, objType));
            }
        }
    }

    [Server]
    private IEnumerator Despawn(GameObject obj, SpawnObject objType) {
        yield return new WaitForSeconds(objType.despawnTime);
        objType.count--;
        NetworkServer.Destroy(obj);
    }

    private Vector3 RandomPointOnCircle() {
        float angle = Random.Range(0f, Mathf.PI * 2); // Random angle in radians
        float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
        float x = randomRadius * Mathf.Cos(angle);
        float y = randomRadius * Mathf.Sin(angle);
        return new Vector3(x, y, 0f) + transform.position;
    }

}
