using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	public GameObject[] bridgePrefabs; // For the bridges

	// will create other types of Prefabs eg. highwayPrefabs, cityPrefabs, forestPrefabs
	// ....
	//

	private Transform playerTransform;
	private float spawnZ = -30.0f; // position of path that will be spawned behind player
	private float pathLength = 30.0f;

	private float safeZone = 40.0f; // To prevent path from being deleted too soon

	private int amnPathsOnScreen = 5; // Number of paths appearing on screen

	private int lastPrefabIndex = 0;

	private List<GameObject> activePaths;

    private float count = 0.0f;

	// Use this for initialization
	void Start ()
	{
		activePaths = new List<GameObject>();

		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

		for(int i = 0; i < amnPathsOnScreen; i++)
		{
			if (i < 3)
			{
				SpawnPath(0);
                count++;
                Debug.Log(count);
			}
			else
			{
				SpawnPath();
                count++;
                Debug.Log(count);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(playerTransform.position.z - safeZone > (spawnZ - amnPathsOnScreen * pathLength))
		{
			SpawnPath();
			DeletePath();
		}	
	}

	private void SpawnPath(int prefabIndex = -1)
	{
		GameObject go;

		if(prefabIndex == -1)
		{
			go = Instantiate(bridgePrefabs[RandomPrefabIndex()]) as GameObject;
		}
		else
		{
			go = Instantiate(bridgePrefabs[prefabIndex]) as GameObject;
		}

		go.transform.SetParent(transform);
		go.transform.position = Vector3.forward * spawnZ;
		spawnZ += pathLength;
		activePaths.Add(go);
	}

	private void DeletePath()
	{
		Destroy(activePaths[0]);
		activePaths.RemoveAt(0);
	}

	private int RandomPrefabIndex()
	{
		if(bridgePrefabs.Length <= 1)
		{
			return 0;
		}

		int randomIndex = lastPrefabIndex;

		while(randomIndex == lastPrefabIndex)
		{
			randomIndex = Random.Range(0, bridgePrefabs.Length);
		}
		lastPrefabIndex = randomIndex;
		return randomIndex;
	}
}
