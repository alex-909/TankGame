using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DapperDino.Mirror.Tutorials.Lobby;

public class CreateMap : NetworkBehaviour
{
	private char[,] mapArray;
	public TankGameGenerator mapCreator;
	public GameObject wallPrefab;
	public GameObject wallParentPrefab;
	public float yHeight;

	public Vector3 offset;
	private GameObject wallParent;
	[Space]
	public Material WallMaterial;

	private NetworkManagerTG room;
	private NetworkManagerTG networkManager
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as NetworkManagerTG;
		}
	}

	public override void OnStartServer()
	{
		wallParent = CreateWallParent();
		wallParent.transform.position = Vector3.zero;

		//set correct position!
		Debug.Log("server generates map");
		GenerateMap();
		wallParent.transform.position = offset;
	}

	public void ResetMap()
	{
		Debug.Log("reset the map");
		DeleteOldMap();

		GenerateMap();
		wallParent.transform.position = offset;
	}
	void GenerateMap()
	{
		mapArray = GetMapArray();
		PlaceAllWalls();
	}
	char[,] GetMapArray()
	{
		return mapCreator.GetMap();
	}
	void PlaceAllWalls()
	{
		for (int i = 0; i < 31; i++)
		{
			for (int j = 0; j < 49; j++)
			{
				TryPlaceSingleWall(j, i, mapArray[i, j]);
			}
		}
	}
	void TryPlaceSingleWall(int x, int y, char c)
	{
		if (!c.Equals('#'))
		{
			return;
		}

		Vector3 pos = new Vector3(x, yHeight, y);
		var go = Instantiate(wallPrefab, pos, Quaternion.identity);
		//go.transform.parent = wallParent.transform;
		NetworkServer.Spawn(go);
	}
	// Update is called once per frame

	void DeleteOldMap()
	{
		WallScript[] walls = FindObjectsOfType<WallScript>();

		foreach (WallScript wall in walls)
		{
			//Debug.Log("destroyed a single block");
			NetworkServer.Destroy(wall.gameObject);
		}
		NetworkServer.Destroy(wallParent);

		wallParent = CreateWallParent();

		wallParent.transform.position = Vector3.zero;
	}
	GameObject CreateWallParent()
	{
		var wallParent = Instantiate(wallParentPrefab, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(wallParent);
		return wallParent;
	}
}
