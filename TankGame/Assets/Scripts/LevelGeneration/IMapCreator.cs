using UnityEngine;
public abstract class IMapCreator : MonoBehaviour
{
	public abstract char[,] GetMap();
	public int yField;
	public int xField;
}

