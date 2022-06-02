using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum TankColor
{
    Blue,
    Red,
    Green,
    Yellow
}
public class ApplyMaterials : NetworkBehaviour
{
    [SyncVar] public TankColor tankColor;
    [SerializeField] private GameObject Cannon;
    [SerializeField] private GameObject TankBase;

	public override void OnStartClient()
	{
        (Material cannonMat, Material tankMat) = GameAssets.i.GetTankMaterials(this.tankColor);
        ApplyCannonMaterial(cannonMat);
        ApplyTankBaseMaterial(tankMat);
	}
	//check for material in onstartclient, then apply the material
	public void ApplyCannonMaterial(Material cannonMaterial) 
    {
        Cannon.GetComponent<Renderer>().material = cannonMaterial;
    }
    public void ApplyTankBaseMaterial(Material tankBaseMaterial)
    {
        TankBase.GetComponent<Renderer>().material = tankBaseMaterial;
    }
}
