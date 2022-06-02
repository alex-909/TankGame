using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using DapperDino.Tutorials.Lobby;

public class PlayerShoot : NetworkBehaviour
{

    //idea: have a shoot test point to see if in wall and a spawn point
    [Header("other")]
    [SerializeField] private PlayerScript playerScript;
    [Header("Bullet")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform wallTest;
    [SerializeField] Transform spawn;
    [SerializeField] KeyCode shootKey;
    [Space]
    [SerializeField] LayerMask layerMask;
    [SerializeField] float radius;

    [Header("Missile")]
    [SerializeField] GameObject missile;
    public float missileCooldown;
    public float missileOffset;
    public int maxMissile;

    [Header("Ammo")]
    public LineController lc;
    public AmmoUI ammoUI;
    public GameObject ammoCanvas;
    [Space]
    public float ammoCooldown;
    public int maxAmmo;
    

    private float timer_ammo = 0;
    private float timer_missile = 0;
    private int ammo;
    private int missiles;
    // Update is called once per frame
    int hits = 0;
	private void Awake()
	{
        lc = GameObject.FindGameObjectWithTag("LR").GetComponent<LineController>();
	}
    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }
    private DeathManager DeathManager
    {
        get
        {
            return (DeathManager)FindObjectOfType(typeof(DeathManager));
        }
    }
    public override void OnStartAuthority()
	{
        if (hasAuthority)
        {
            ammoCanvas.SetActive(true);
        }
        ammoUI.CreateUI(maxAmmo);
        lc.SetUpLine(spawn);
        lc.SetLineEnabled(false);

        InputManager.Controls.PlayerMap.Shoot.performed += ctx => SetPressedBullet();
        InputManager.Controls.PlayerMap.Missile.performed += ctx => SetPressedMissile();
    }
    [Client]
    private void SetPressedBullet() 
    {
        if (ammo > 0)
        {
            if (!IsInWall() && playerScript.IsGrounded())
            {
                ammo--;

                CmdSpawnBullet();
                //go.transform.eulerAngles = new Vector3(90, go.transform.rotation.y, go.transform.rotation.z);
            }
        }
        ammoUI.UpdateAmmo(ammo);
    }
    [Client]
    private void SetPressedMissile() 
    {
        if (missiles > 0) 
        {
            missiles--;
            Debug.Log("created missile");
            Vector3 pos = playerScript.MousePosition;
            CmdSpawnMissile(pos);
        }
        ammoUI.UpdateMissile(missiles);
    }
    void Update()
    {
        if (!hasAuthority) { return; }

		ReloadBullets();
        ReloadMissile();

		ammoUI.UpdateAmmo(ammo);
        ammoUI.UpdateMissile(missiles);

        CheckLineRenderer();
    }
    private void CheckLineRenderer() 
    {
        if (Input.GetKey(KeyCode.Mouse1)) { DrawBouncePreview(); }

        else { lc.SetLineEnabled(false); }
    }
    private void ReloadBullets() 
    {
        Debug.Log($"missiles: {missiles}");
        if (ammo < maxAmmo)
        {
            timer_ammo += Time.deltaTime;
            if (timer_ammo > ammoCooldown)
            {
                timer_ammo = 0;
                ammo++;
            }
        }
        else
        {
            timer_ammo = 0;
        }

        if (ammo > maxAmmo) ammo = maxAmmo;
    }
    private void ReloadMissile() 
    {
        if (missiles < maxMissile)
        {
            timer_missile += Time.deltaTime;
            if (timer_missile > missileCooldown)
            {
                timer_missile = 0;
                missiles++;
            }
        }
        else
        {
            timer_missile = 0;
        }

        if (missiles > maxMissile) missiles = maxMissile;
    }

    [Command]
    private void CmdSpawnBullet() 
    {
        var go = Instantiate(bullet, spawn.position, bullet.transform.rotation);
        Vector3[] targetPoints = DrawBouncePreview();

        go.transform.forward = (targetPoints[0] - transform.position).normalized;
        go.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);

        var bulletMove = go.GetComponent<BulletMove>();
        bulletMove.points.AddRange(targetPoints);
        bulletMove.KilledPlayer += GotKill;
        bulletMove.HitPlayer += GotPlayerHit;
        bulletMove.originPlayer = this.GetComponent<NetworkIdentity>().netId;
        NetworkServer.Spawn(go);
    }
    [Command]
    private void CmdSpawnMissile(Vector3 pos)
    {
        pos.y += missileOffset;
        var go = Instantiate(missile, pos, Quaternion.identity);
        NetworkServer.Spawn(go);
    }

    private void GotKill() 
    {
        Debug.Log("registered kill in playershoot");
        DeathManager.PlayerGotKill(this.playerScript.networkGamePlayerIdentity);
    }
    private void GotPlayerHit()
    {
        Debug.Log("registered hit in playershoot");
        hits++;
        Debug.Log($"hits: {hits}");
    }
    private Vector3[] DrawBouncePreview() 
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 startPosition = spawn.position;
        Vector3 direction = spawn.forward;
        //points.Add(startPosition);
        for (int i = 0; i < 3; i++) 
        {
            var hit = LinePreviewEnd(startPosition, direction, out bool success);
            if (!success)
            {
                
                Debug.Log("no success");
                points.Add(playerScript.MousePosition);
                break;
                //Debug.LogError("not hit!");
                
            }
            points.Add(hit.point);
            startPosition = hit.point;
            direction = Vector3.Reflect(direction.normalized, hit.normal);
        }
        return points.ToArray();
    }
    private RaycastHit LinePreviewEnd(Vector3 startPos, Vector3 direction, out bool success) 
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(startPos, direction, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(startPos, direction * hit.distance, Color.yellow);
            //Debug.Log("Did hit");
            success = true;
            return hit;
        }
        else
        {
            //Debug.LogError("Did not hit");
            success = false;
            return hit;
        }
    }
    private bool IsInWall() 
    {
        bool test_in_wall = false;
        bool spawn_in_wall = false;

        Collider[] hitColliders = Physics.OverlapSphere(wallTest.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Wall"))
            {
                Debug.Log("In Wall");
                test_in_wall = true;
            }
        }

        hitColliders = Physics.OverlapSphere(spawn.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Wall"))
            {
                Debug.Log("In Wall");
                spawn_in_wall = true;
            }
        }

        if (spawn_in_wall || test_in_wall) 
        {
			return true;
        }
        return false;
    }
    public int GetAmmo() 
    {
        return ammo;
    }
}
