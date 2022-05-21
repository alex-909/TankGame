using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;
    public static GameAssets i 
    {
        get 
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    public TankMaterial[] tankMaterials;

    [System.Serializable]
    public class TankMaterial
    {
        public TankColor color;
        public Material cannonMaterial;
        public Material tankBaseMaterial;
    }

    public (Material cannonMat, Material tankBaseMat) GetTankMaterials(TankColor tankColor) 
    {
        for (int i = 0; i < tankMaterials.Length; i++) 
        {
            if (tankMaterials[i].color == tankColor) 
            {
                return (tankMaterials[i].cannonMaterial, tankMaterials[i].tankBaseMaterial);
            }
        }

        Debug.LogError("Tank Color not found!");
        return (tankMaterials[0].cannonMaterial, tankMaterials[0].cannonMaterial);
    }
}
