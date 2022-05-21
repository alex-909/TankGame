using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DapperDino.Mirror.Tutorials.Lobby;

public class CreateMapBackUp : NetworkBehaviour
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
	private NetworkManagerTG Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as NetworkManagerTG;
		}
	}
	private void Awake()
	{
		wallParent = CreateWallParent();
		wallParent.transform.position = Vector3.zero;
		//set correct position!
		GenerateMap();
		wallParent.transform.position = offset;
	}
	/*
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			DeleteOldMap();
			GenerateMap();
			wallParent.transform.position = offset;
		}
	}
	*/
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
		go.transform.parent = wallParent.transform;
	}
	// Update is called once per frame

	void DeleteOldMap()
	{
		Destroy(wallParent);
		wallParent = CreateWallParent();
		wallParent.transform.position = Vector3.zero;
	}
	GameObject CreateWallParent()
	{
		return Instantiate(wallParentPrefab, Vector3.zero, Quaternion.identity);
	}


}
