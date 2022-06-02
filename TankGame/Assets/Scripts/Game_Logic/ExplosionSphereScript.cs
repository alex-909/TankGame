using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DapperDino.Tutorials.Lobby;
using Mirror;

public class ExplosionSphereScript : NetworkBehaviour
{
    private float fTimer = 0;
    void Start()
    {
        var center = transform.position;
        var radius = transform.localScale.x / 2;

        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var c in hitColliders)
        {
            if (c.gameObject.TryGetComponent<PlayerKnockback>(out PlayerKnockback playerKnockback)) 
            {
                playerKnockback.Knockback(center);
            }
        }
    }
	private void Update()
	{
        if (fTimer < 0.1)
        {
            fTimer += Time.deltaTime;
        }
        else 
        {
            NetworkServer.Destroy(this.gameObject);   
        }
	}
}
