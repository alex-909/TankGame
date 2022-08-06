using DapperDino.Mirror.Tutorials.Lobby;
using UnityEngine;
using System.Linq;

public class PlayerSpawnPointsHolder : MonoBehaviour
{
    [SerializeField] public Transform[] points;
    [SerializeField] public TankColor[] colors;
    private void Awake() 
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].gameObject.GetComponent<MaterialReference>().tankColor = colors[i];
            PlayerSpawnSystem.AddSpawnPoint(points[i]);
        }
    }
    Transform[] ShufflePoints(Transform[] pointArray) 
    {
        for (int i = 0; i < pointArray.Length; i++) 
        {
            int destIndex = Random.Range(0, pointArray.Length);
            Debug.Log(destIndex);
            int sourceIndex = i;

            if (destIndex != sourceIndex)
            {
                Transform temp = pointArray[sourceIndex];
                pointArray[sourceIndex] = pointArray[destIndex];
                pointArray[destIndex] = temp;
            }
        }
        return pointArray;
    }
}
