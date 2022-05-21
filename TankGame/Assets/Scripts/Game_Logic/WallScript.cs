using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WallScript : NetworkBehaviour
{
	public override void OnStartServer()
	{
		//Debug.Log("start wallscript");

		//warum darf ich das object nicht parenten? niemand weiﬂ es
		//jetzt sind leider alle in der hierarchie verstreut

		//if (this.transform.parent == null)

		//Debug.Log("set parents!");

		// GameObject parentObject = GameObject.FindGameObjectWithTag("WALLPARENT");

		//float xOffset = parentObject.transform.position.x;
		//float zOffset = parentObject.transform.position.z;

		hideFlags = HideFlags.HideInHierarchy;

		float xOffset = -24;
		float zOffset = -15;

		transform.position = new Vector3(
			transform.position.x + xOffset, //wallparent x
			transform.position.y,
			transform.position.z + zOffset //wallparent  z
			);

		//transform.SetParent(parentObject.transform);

		//Debug.Log("i still exist!");

	}

}
