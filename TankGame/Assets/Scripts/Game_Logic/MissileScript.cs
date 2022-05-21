using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MissileScript : NetworkBehaviour
{
    [SerializeField] GameObject explosionSphere;
    public float waitTime;
    public float explosionRadius;
    public float speed;

    private float fTimer = 0;
    private bool explosionTriggered = false;

    void Update()
    {
        if (fTimer < waitTime)
        {
            fTimer += Time.deltaTime;
        }
        else //missile starts falling down
        {
            MoveDown();
        }
    }
    private void MoveDown() 
    {
        if (transform.position.y > 0)
        {
            var pos = transform.position;
            pos.y -= speed * Time.deltaTime;
            transform.position = pos;
        }
        else if (!explosionTriggered) { Explode(); }
    }
    private void Explode() 
    {
        explosionTriggered = true;
        CmdExplode();
    }
    [Command(requiresAuthority = false)]
    private void CmdExplode() 
    {
        //spawn particle system
        //RpcPlayParticles();

        //spawn sphere
        var pos = transform.position;
        pos.y = 0;
        var go = Instantiate(explosionSphere, pos, Quaternion.identity);
        NetworkServer.Spawn(go);
        NetworkServer.Destroy(this.gameObject);
    }
    [ClientRpc]
    private void RpcPlayParticles() 
    {

    }
}
