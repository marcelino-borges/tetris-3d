using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    public GameObject[] prefabsToSpawn;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
            Spawn();
        #endif
    }

    public void Spawn()
    {
        if(prefabsToSpawn != null && prefabsToSpawn.Length > 0)
        {
            GameObject sortedPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
            Group group = Instantiate(sortedPrefab, transform.position, Quaternion.identity).GetComponent<Group>();
            group.spawner = this;
        }
    }
}
