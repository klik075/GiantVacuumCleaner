using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnContainer : MonoBehaviour
{
    public List<Transform> spawnSpace = new List<Transform>();
    public void FindSpawnSpace()
    {
        Transform[] spawnSpaceGameObject = GetComponentsInChildren<Transform>();

        if (spawnSpaceGameObject != null)
        {
            spawnSpace = spawnSpaceGameObject.ToList();
            spawnSpace.RemoveAt(0);
        }
    }
}
