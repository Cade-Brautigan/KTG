using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{

    // FIND A WAY TO TRACK WHEN OBJECTS ARE CONSUMED BY PLAYER AND REFLECT THIS IN COUNT
    // FIX CLASS TO FULLY REFLECT OOP PRINCIPLES

    float radius = 30f;
    // represents all SpawnObject types
    public List<SpawnObject> spawnObjects = new List<SpawnObject>();

    [System.Serializable]
    public class SpawnObject {
        public GameObject prefab;
        public int initCount;
        public int count;
        public int maxCount;
        public float respawnTime; // time it takes for object.count++
        public float despawnTime; // lifetime of each object. despawnTime = 0 means object do not despawn
        public bool isRespawning = false;
    }

    void Start() {
        // Instantiate the initial count of each object type
        foreach (SpawnObject obj in spawnObjects) {
            for (int i = 0; i < obj.initCount; i++) {
                Spawn(obj);
            }
        }
    }

    void FixedUpdate() {
        foreach (SpawnObject obj in spawnObjects) {
            // if count is less than maxCount and respawn is not running, starts respawn
            if (obj.count < obj.maxCount && obj.isRespawning == false) {
                StartCoroutine(Respawn(obj));
            }
        }
    }

    IEnumerator Respawn(SpawnObject obj) {
        if (obj.isRespawning) {yield break;}
        obj.isRespawning = true;
        yield return new WaitForSeconds(obj.respawnTime);
        Spawn(obj);
        obj.isRespawning = false;
    }                                                                                               

    void Spawn(SpawnObject objType) {
        if (objType.count < objType.maxCount) {
            GameObject newObj = Instantiate(objType.prefab, RandomPointOnCircle(), Quaternion.identity);
            //Debug.Log("New obj " + newObj);
            //Debug.Log(objType.prefab.ToString() + "spawned");
            objType.count++;
            // Set a timer to despawn the object after a certain time
            if (objType.despawnTime > 0) {
                StartCoroutine(Despawn(newObj, objType));
            }
        }
    }

    IEnumerator Despawn(GameObject obj, SpawnObject objType) {
        yield return new WaitForSeconds(objType.despawnTime);
        objType.count--;
        Destroy(obj);
        Debug.Log(objType.prefab + " despawned");
    }

    /*
    public SpawnObject FindCorrespondingPrefab(GameObject obj) {
        string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
        string objSceneName = obj.scene.name;
        foreach (SpawnObject spawnObj in spawnObjects) {
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(spawnObj.prefab);
            string prefabSceneName = spawnObj.prefab.scene.name;
            if (assetPath == prefabPath && objSceneName == prefabSceneName) {
                return spawnObj;
            }
        }
        return null;
    }
    */

    private Vector3 RandomPointOnCircle() {
        float angle = Random.Range(0f, Mathf.PI * 2); // Random angle in radians
        float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
        float x = randomRadius * Mathf.Cos(angle);
        float y = randomRadius * Mathf.Sin(angle);
        return new Vector3(x, y, 0f) + transform.position;
    }

}
