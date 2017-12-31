using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour {

    public GameObject prefab;
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    public GameObject GetObject()
    {
        GameObject spawnedGameObject;
        if (inactiveInstances.Count>0)
        {
            spawnedGameObject = inactiveInstances.Pop();
        }
        else
        {
            spawnedGameObject = Instantiate(prefab) as GameObject;

            //add the PooledObject component to the prefab so we know it came from this pool
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
            pooledObject.pool = this;
        }

        //put the instance in the root of the scene and enable it
        spawnedGameObject.transform.SetParent(null);
        spawnedGameObject.SetActive(true);

        return spawnedGameObject;
    }

    //return an instance of the prefab to the pool
    public void ReturnObject(GameObject toReturn)
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        //if the instance came from this pool, return it to the pool
        if (pooledObject != null && pooledObject.pool == this)
        {
            toReturn.transform.SetParent(transform);
            toReturn.SetActive(false);

            // add the instance to the collection of inactive Instance
            inactiveInstances.Push(toReturn);
        }
        else
        {
            Debug.LogWarning(toReturn.name + "was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }
}

// a component that simply identifies the pool that a GameObject came from
public class PooledObject : MonoBehaviour
{
    public SimpleObjectPool pool;
}
