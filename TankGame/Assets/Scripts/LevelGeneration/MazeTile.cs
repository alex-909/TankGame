using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MazeTile
{
	public int xPos;
	public int yPos;
	public bool visited;

	MazeTile[] optionTiles; //unvisited tiles next to this tile

	public MazeTile(int x, int y)
	{
		optionTiles = new MazeTile[4];
		for (int i = 0; i < 4; i++)
		{
			optionTiles[i] = null;
		}

		visited = false;
		xPos = x;
		yPos = y;
	}
	public void setVisited()
	{
		visited = true;
	}
}

