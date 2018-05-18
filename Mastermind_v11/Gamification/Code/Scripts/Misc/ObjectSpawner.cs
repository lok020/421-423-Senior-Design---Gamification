using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    //allows spawning objects and their rate of spawning
    public List<GameObject> spawnPoints;
    public List<GameObject> spawnedObjects;
    public List<Vector3> spawnedLocations;
    public List<float> spawnedRates;
    public int[] rates;
    public new Camera camera;
    public float heightdivisor = 3.0f, widthdivisor = 1.0f;//Allows to limit possible area for spawning of objects (camera /widthdivisor=2 means objects can only spawn on the left half of thescreen)
    // Use this for initialization
    void Start()
    {
        //spawnObjects();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnRandomObjects(List<GameObject> objects, List<Vector3> objectLocations, List<float> spawnRates, SceneManagerInstance manager)
    {
        //foreach(Vector3 location in objectLocations)
        for(int k = 0; k < objectLocations.Count; k++)
        {
            List<int> attemptedSpawns = new List<int>();
            for(int i = 0; i < objects.Count; i++)
            {
                int index;
                while (attemptedSpawns.Contains(index = Random.Range(0, objects.Count))){ }

                if(spawnRates[index] < Random.Range(0f, 1f))
                {
                    objects[index].transform.position = objectLocations[index];
                    spawnedObjects.Add(Instantiate(objects[index]));
                    spawnedLocations.Add(objectLocations[index]);
                    spawnedRates.Add(1);
                    break;
                }
                attemptedSpawns.Add(index);
                if(attemptedSpawns.Count == objects.Count)
                {
                    break;
                }
            }
        }
        manager.objects = spawnedObjects;
        manager.objectLocations = spawnedLocations;
        foreach(GameObject o in spawnedObjects) DontDestroyOnLoad(o);
    }

    public void SpawnRandomObjectsFromSpawnPoints(List<GameObject> objects, List<float> spawnRates, SceneManagerInstance manager)
    {
        foreach (GameObject location in spawnPoints)
        {
            if (manager.globalSpawnRate < Random.Range(0f, 1f)) continue;
            List<int> attemptedSpawns = new List<int>();
            for (int i = 0; i < objects.Count; i++)
            {
                int index;
                while (attemptedSpawns.Contains(index = Random.Range(0, objects.Count))) { }

                if (spawnRates[index] >= Random.Range(0f, 1f))
                {
                    //Debug.Log(location.transform.position);
                    GameObject obj = Instantiate(objects[index]);
                    obj.transform.position = location.transform.position;
                    spawnedObjects.Add(obj);
                    spawnedLocations.Add(location.transform.position);
                    spawnedRates.Add(1);
                    break;
                }
                attemptedSpawns.Add(index);
                if (attemptedSpawns.Count == objects.Count)
                {
                    break;
                }
            }
            attemptedSpawns.Clear();
        }
        manager.objects = spawnedObjects;
        manager.objectLocations = spawnedLocations;
        foreach (GameObject o in spawnedObjects) DontDestroyOnLoad(o);
    }

    public void RespawnObjects(List<GameObject> objects, List<Vector3> objectLocations)
    {
        for(int i = 0; i < objectLocations.Count - 1; i++)
        {
            Instantiate(objects[i]).SetActive(true);
        }
    }

    public void RespawnObjects()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Instantiate(spawnedObjects[i]).SetActive(true);
        }
    }

    public void CleanObjects()
    {
       for(int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
        }
    }

    public void HideObjects()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].SetActive(false);
        }
    }

    public void ShowObjects()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].SetActive(true);
        }
    }

    public void SpawnObjectsFromSceneManager(List<GameObject> objects,List<Vector3> objectLocations, List<float> spawnRates)
    {
        /*for (int i = 0; i < objectIds.Count;i++)
        {
            GameObject go = Resources.Load(prefabPath[i]) as GameObject;
            go.transform.position = objectLocations[i];
            Instantiate(go);
        }*/
        for (int i = 0; i < objects.Count; i++)
        {
            Debug.Log("spawning" + objects[i].name);
            //add code to randomly decide location if a flag is set
            if (Random.Range(0f, 1f) < spawnRates[i])
            {
                objects[i].transform.position = objectLocations[i];
                spawnedObjects.Add(Instantiate(objects[i]));
            }
        }
    }

    /*void spawnObjects()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            //Debug.Log("Spawning Objects" + i);
            for (int j = 0; j < rates[i]; j++)
            {
                //Debug.Log("Spawning Objects");
                Vector3 randompos = new Vector3(Random.Range(0, camera.pixelWidth / widthdivisor), Random.Range(0, camera.pixelHeight / heightdivisor), 0);
                GameObject newobj = GameObject.Instantiate(objects[i]);
                randompos = camera.ScreenToWorldPoint(randompos);
                randompos.z = 0;
                newobj.transform.position = randompos;

            }
        }
    }*/
}