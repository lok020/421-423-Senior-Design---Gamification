using UnityEngine;
using System.Collections.Generic;


public class SceneManager : MonoBehaviour {

    public List<Vector3> objectLocations;
    public List<Vector3> randomLocationRange;
    public List<float> spawnChances;
    public List<GameObject> objects;
    public bool Started = false;
    public bool RandomLocations = false;
    public string Name;
    public float globalSpawnRate;

    public List<Vector3> GetObjectLocations()
    {
        List<Vector3> Locations = new List<Vector3>();
        for(int i=0; i < objectLocations.Count; i++)
        {
            Locations.Add(new Vector3(Random.Range(objectLocations[i].x - randomLocationRange[i].x, objectLocations[i].x + randomLocationRange[i].x), Random.Range(objectLocations[i].y - randomLocationRange[i].y, objectLocations[i].y + randomLocationRange[i].y), 0));
        }
        return Locations;
    }

    public void SpawnObjects()
    {
        List<Vector3> Locations = new List<Vector3>();
        if (RandomLocations)
        {
            Locations = GetObjectLocations();
        }
        else
        {
            Locations = objectLocations;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            Debug.Log("attempting to spawn object");
            //add code to randomly decide location if a flag is set
            if (spawnChances[i] < Random.Range(0, 1))
                Instantiate(objects[i]).transform.position = Locations[i];
        }
    }
}
