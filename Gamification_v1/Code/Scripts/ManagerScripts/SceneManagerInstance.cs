using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManagerInstance{
    public List<Vector3> objectLocations;   //locations of objects in scene
    public List<Vector3> randomLocationRange;   //range of randomness for randomly placing
    public List<float> spawnChances;    //chances of an object spawning
    public List<GameObject> objects;    //list of objects in scene
    public bool started = false;
    public bool randomLocations = false;    //if true will use randomlocationrange to determine locations, if false will just use objectlocations
    public string name; //name of the scene
    public float globalSpawnRate;

    public SceneManagerInstance(SceneManager original)
    {
        globalSpawnRate = original.globalSpawnRate;
        spawnChances = new List<float>(original.spawnChances);
        randomLocationRange = new List<Vector3>(original.randomLocationRange);
        objectLocations = new List<Vector3>(original.objectLocations);
        objects = new List<GameObject>(original.objects);
        randomLocations = original.RandomLocations;
        //started = original.started;
        //randomLocations = original.randomLocations;
    }

    //finds locations using randomlocationrange if flag is flipped, otherwise just returns original locations
    public List<Vector3> GetObjectLocations()
    {
        if (randomLocations)
        {
            List<Vector3> Locations = new List<Vector3>();
            for (int i = 0; i < objectLocations.Count; i++)
            {
                Locations.Add(new Vector3(Random.Range(objectLocations[i].x - randomLocationRange[i].x, objectLocations[i].x + randomLocationRange[i].x), Random.Range(objectLocations[i].y - randomLocationRange[i].y, objectLocations[i].y + randomLocationRange[i].y), 0));
            }
            objectLocations = Locations;
            return Locations;
        }
        else
        {
            return objectLocations;
        }
    }

    //used to remove an object at objects[index] after being collected or killed
    public void RemoveObject(int index)
    {
        objects.RemoveAt(index);
        objectLocations.RemoveAt(index);
        //spawnChances.RemoveAt(index);
        //randomLocationRange.RemoveAt(index);
    }
}
