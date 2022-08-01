using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private Sprite powerUpImage;
    [SerializeField] private bool isPermanent;
    [SerializeField] private float duration;

	[Header("Container")]
	[SerializeField] GameObject pu_container;
	[SerializeField] float waittime = 5;

	private float fTimer = 0;
	private bool obtainable = false;
	private GameObject container;

    public PowerUpInfo GetInfo() 
    {
        return new PowerUpInfo(powerUpType, isPermanent, duration, 0, powerUpImage);
    }
	private void Start()
	{
		this.GetComponent<BoxCollider>().enabled = false;
		container = Instantiate(pu_container, this.transform.position, Quaternion.identity);
	}
	private void Update()
	{
		if (obtainable) { return; }

		fTimer += Time.deltaTime;
		
		if (fTimer > waittime) 
		{
			DisableContainer();
			EnableCollider();
			obtainable = true;
		}
	}
	private void DisableContainer() 
	{
		Destroy(container);
	}
	private void EnableCollider()
	{
		this.GetComponent<BoxCollider>().enabled = true;
	}
}
