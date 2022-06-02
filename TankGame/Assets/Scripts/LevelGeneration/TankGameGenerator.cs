using System;
using System.Collections.Generic;
using UnityEngine;

enum Dir
{
	North,
	East,
	South,
	West,
}
public class TankGameGenerator : IMapCreator
{
	static char[,] tiles;
	public static new int xField = 49;
	public static new int yField = 31;

	static MazeTile[,] maze;
	static int mazeX = 8;
	static int mazeY = 5;

	static System.Random r;

	public override char[,] GetMap()
	{
		r = new System.Random();
		tiles = new char[yField, xField];
		maze = new MazeTile[mazeY, mazeX];

		Console.ReadKey();
		Console.Clear();
		Console.WriteLine("");

		BlankArray();
		BlankMaze();
		RunMaze();
		DisplayArray();
		return tiles;

	}
	static void RunMaze()
	{
		List<MazeTile> VisitedTiles = new List<MazeTile>();
		int xPos = 0;
		int yPos = 0;
		while (!isDone())
		{
			MazeTile currentTile = maze[yPos, xPos];
			currentTile.setVisited();
			VisitedTiles.Add(currentTile);

			Dir[] options = GetOptions(xPos, yPos);

			if (options.Length == 0)
			{
				for (int i = (VisitedTiles.Count - 1); i >= 0; i--)
				{
					MazeTile m = VisitedTiles[i];
					Dir[] remainOptions = GetOptions(m.xPos, m.yPos);
					if (remainOptions.Length != 0)
					{
						currentTile = m;
						xPos = m.xPos;
						yPos = m.yPos;
						options = GetOptions(xPos, yPos);

						break;
					}
				}
			}
			Dir walkDir = chooseDir(options);
			//Dir walkDir = Dir.North;
			MazeTile NextTile = GetTileInDirection(currentTile.xPos, currentTile.yPos, walkDir);
			(int xSpace1, int ySpace1) = convertToWorldSpace(NextTile.xPos, NextTile.yPos);
			(int xSpace2, int ySpace2) = convertToWorldSpace(currentTile.xPos, currentTile.yPos);
			WritePath(xSpace1, ySpace1, xSpace2, ySpace2, walkDir);

			NextTile.setVisited();
			VisitedTiles.Add(NextTile);
			currentTile = NextTile;
			xPos = currentTile.xPos;
			yPos = currentTile.yPos;

			//for debugging:
			//Console.ReadKey();
			//DisplayArray();
		}


		AddExtraHoles();
	}
	static void AddExtraHoles()
	{
		(int x_Space1, int y_Space1) = convertToWorldSpace(4, 1);
		(int x_Space2, int y_Space2) = convertToWorldSpace(2, 1);
		WritePath(x_Space1, y_Space1, x_Space2, y_Space2, Dir.East);

		(x_Space1, y_Space1) = convertToWorldSpace(4, 3);
		(x_Space2, y_Space2) = convertToWorldSpace(2, 3);
		WritePath(x_Space1, y_Space1, x_Space2, y_Space2, Dir.East);

		WritePathHelper(0, 3, 0, 4, Dir.North);
		WritePathHelper(0, 0, 0, 1, Dir.North);
		WritePathHelper(7, 3, 7, 4, Dir.North);
		WritePathHelper(7, 0, 7, 1, Dir.North);

		WritePathHelper(1, 0, 0, 0, Dir.East);
		WritePathHelper(7, 0, 6, 0, Dir.East);
		WritePathHelper(1, 4, 0, 4, Dir.East);
		WritePathHelper(7, 4, 6, 4, Dir.East);

	}
	static void WritePathHelper(int x1, int y1, int x2, int y2, Dir d) 
	{
		(int x_Space1, int y_Space1) = convertToWorldSpace(x1, y1);
		(int x_Space2, int y_Space2) = convertToWorldSpace(x2, y2);
		WritePath(x_Space1, y_Space1, x_Space2, y_Space2, d);
	}
	static bool isDone()
	{
		foreach (MazeTile m in maze)
		{
			if (!m.visited)
			{
				return false;
			}
		}
		return true;
	}
	static MazeTile GetTileInDirection(int x, int y, Dir walkDir)
	{
		switch (walkDir)
		{
			case Dir.North:
				return maze[y - 1, x];
			case Dir.East:
				return maze[y, x + 1];
			case Dir.South:
				return maze[y + 1, x];
			case Dir.West:
				return maze[y, x - 1];
			default:
				Console.WriteLine("NO TILE IN GETTILEINDIRECTION!!");
				return maze[0, 0];
		}
	}
	/*
	static void WritePath2(int x1, int y1, int x2, int y2, Dir dir)
	{
		//todo
		if (dir == Dir.North) // y1 is smaller
		{
			for (int i = (y1 - 2); i <= (y2 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
		if (dir == Dir.South) // y2 is smaller
		{
			for (int i = (y2 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
		if (dir == Dir.East) // x2 is smaller
		{
			for (int i = (y1 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x2 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '_';
				}
			}
		}
		if (dir == Dir.West) // x1 is smaller
		{
			for (int i = (y1 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x2 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
	}
	*/
	static void WritePath(int x1, int y1, int x2, int y2, Dir dir)
	{
		//todo
		if (dir == Dir.North) // y1 is smaller
		{
			for (int i = (y1 - 2); i <= (y2 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
		if (dir == Dir.South) // y2 is smaller
		{
			for (int i = (y2 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
		if (dir == Dir.East) // x2 is smaller
		{
			for (int i = (y1 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x2 - 2); j <= (x1 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
		if (dir == Dir.West) // x1 is smaller
		{
			for (int i = (y1 - 2); i <= (y1 + 2); i++)
			{
				for (int j = (x1 - 2); j <= (x2 + 2); j++)
				{
					tiles[i, j] = '.';
				}
			}
		}
	}
	static (int x, int y) convertToWorldSpace(int xTile, int yTile)
	{
		int x = (xTile * 6) + 3;
		int y = (yTile * 6) + 3;
		return (x, y);
	}
	static Dir chooseDir(Dir[] directions)
	{
		int index = r.Next(directions.Length);
		return directions[index];
	}
	static Dir[] GetOptions(int xPos, int yPos)
	{
		List<Dir> dirList = new List<Dir>();
		if (xPos > 0)
		{
			if (!maze[yPos, xPos - 1].visited)
			{
				dirList.Add(Dir.West);
			}
		}
		if (xPos < 7)
		{
			if (!maze[yPos, xPos + 1].visited)
			{
				dirList.Add(Dir.East);
			}
		}
		if (yPos > 0)
		{
			if (!maze[yPos - 1, xPos].visited)
			{
				dirList.Add(Dir.North);
			}
		}
		if (yPos < 4)
		{
			if (!maze[yPos + 1, xPos].visited)
			{
				dirList.Add(Dir.South);
			}
		}

		return dirList.ToArray();
	}
	static void BlankArray()
	{
		for (int i = 0; i < yField; i++)
		{
			for (int j = 0; j < xField; j++)
			{
				tiles[i, j] = '#';
			}
		}
	}
	static void BlankMaze()
	{
		for (int i = 0; i < mazeY; i++)
		{
			for (int j = 0; j < mazeX; j++)
			{
				maze[i, j] = new MazeTile(j, i);
			}
		}
	}
	static void DisplayArray()
	{
		string output = "";
		for (int i = 0; i < yField; i++)
		{
			output = "";
			for (int j = 0; j < xField; j++)
			{
				output += tiles[i, j];
			}
			Console.WriteLine(output);
		}
	}
}

