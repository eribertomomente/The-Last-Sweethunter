using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OgresSpawner : NetworkBehaviour
{

    public GameObject ogre1Prefab;
    public GameObject ogre2Prefab;

    public GameObject positionOgre1;
    public GameObject positionOgre2;

    public List<GameObject> patrolpoints;

    void Start()
    {
      
        Instantiate(ogre1Prefab, positionOgre1.transform.position, transform.rotation);
        Instantiate(ogre2Prefab, positionOgre2.transform.position, transform.rotation);

    }

}
