using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WallScript : NetworkBehaviour
{
	public override void OnStartServer()
	{
		hideFlags = HideFlags.HideInHierarchy;

		float xOffset = -24;
		float zOffset = -15;

		transform.position = new Vector3(
			transform.position.x + xOffset, //wallparent x
			transform.position.y,
			transform.position.z + zOffset //wallparent  z
			);
	}

}
