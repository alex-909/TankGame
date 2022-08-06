using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;
using Assets.Scripts.Game_Mirror;
using DapperDino.Tutorials.Lobby;

public class PlayerScript : NetworkBehaviour
{
	[SyncVar] public string playerName;
	public NetworkIdentity networkGamePlayerIdentity;

	[SerializeField] float movementSpeed;
	[SerializeField] float extraSpeed;
	private float speed;
	[SerializeField] float rotationSpeed;
	[Space]
	[SerializeField] Transform cannon;
	[SerializeField] Transform body;
	//[SerializeField] Transform test;
	[Space]
	[SerializeField] LayerMask groundMask;

	public HealthManager healthManager;
	[HideInInspector] public bool alive = true;


	[SerializeField] private Rigidbody rb;
	[SerializeField] private PowerUpManagerPlayer powerUpManager;

	[Header("particles")]
	[SerializeField] ParticleSystem particleDamage;
	[SerializeField] GameObject particleExplosion;

	[Header("Ground Check & Explosion")]
	[SerializeField] Transform groundCheck;
	public float groundDistance;
	public float gravity;
	public float yBoost;
	public float strength;
	public LayerMask groundCheckMask;

	private DeathManager DeathManager
	{
		get
		{
			return (DeathManager)FindObjectOfType(typeof(DeathManager));
		}
	}
	public Vector3 MousePosition
	{
		get
		{
			(bool success, Vector3 point) = GetMousePosition();
			return point;
		}
	}

	private Vector3 mousePos;
	private Vector3 moveVector;
	private Vector3 moveDir = new Vector3(0, 0, 1);
	private Vector3 lastMoveDir = new Vector3(0, 0, 1);

	private Vector2 previousInput;
	private Vector2 mouseScreenPos;

	private Controls controls;
	private Controls Controls
	{
		get
		{
			if (controls != null) { return controls; }
			return controls = new Controls();
		}
	}

	private Camera mainCamera;
	private Camera MainCamera
	{
		get
		{
			if (mainCamera != null) { return mainCamera; }
			Debug.Log("searched new camera");
			mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			return mainCamera;
		}
	}

	[Client]
	private void SetMovement(Vector2 movement) => previousInput = movement;
	[Client]
	private void ResetMovement() => previousInput = Vector2.zero;
	[Client]
	private void SetMousePos(Vector2 pos) => mouseScreenPos = pos;


	public override void OnStartAuthority()
	{
		enabled = true;
		if (hasAuthority)
		{
			healthManager.Initialize(3);
			//RpcRegister();
		}
		CmdRegister();
		//mainCamera = Camera.main;
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		moveVector = Vector3.zero;

		InputManager.Controls.PlayerMap.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
		InputManager.Controls.PlayerMap.Move.canceled += ctx => ResetMovement();
		InputManager.Controls.PlayerMap.Aim.performed += ctx => SetMousePos(ctx.ReadValue<Vector2>());
	}

	[Command]
	private void CmdRegister()
	{
		DeathManager.Register(this);
	}

	private void Update()
	{
		if (!hasAuthority) { return; }

		if (!alive) { return; }

		var (success, position) = GetMousePosition();
		if (success)
		{
			mousePos = position;
		}

		moveVector = Vector3.zero;
		moveVector.x = previousInput.x;
		moveVector.z = previousInput.y;

		moveVector = moveVector.normalized;
	}
	private (bool success, Vector3 position) GetMousePosition()
	{
		var ray = MainCamera.ScreenPointToRay(mouseScreenPos);
		if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
		{
			return (success: true, position: hitInfo.point);
		}
		else
		{
			return (success: false, position: Vector3.zero);
		}
	}
	void FixedUpdate()
	{
		if (!hasAuthority) { return; }

		if (!alive) { return; }

		lastMoveDir = moveDir;

		HandleCannonRotation();
		HandleTankMovement();
		HandleTankRotation();
	}

	#region Handle Movement and Rotations
	private void HandleCannonRotation()
	{
		//test.position = mousePos;
		var direction = mousePos;
		direction.y = cannon.position.y;
		cannon.LookAt(direction);
	}

	private void HandleTankMovement()
	{
		//transform.Translate(moveVector * speed * Time.fixedDeltaTime);
		//Debug.Log($"grounded: {isGrounded()}");
		bool grounded = IsGrounded();

		if (!IsGrounded())
		{
			var velocity = rb.velocity;
			velocity.y -= gravity;
			rb.velocity = velocity;
			rb.AddForce(moveVector);
			return;
		}
		if (powerUpManager.HasPowerUp(PowerUpType.ExtraSpeed))
		{
			speed = movementSpeed + extraSpeed;
		}
		else 
		{
			speed = movementSpeed;
		}


		rb.velocity = moveVector.normalized * speed;
		if (rb.velocity.magnitude != 0)
		{
			moveDir = rb.velocity.normalized;
		}
	}
	private void HandleTankRotation()
	{
		var angle1 = Vector3.Angle(body.forward, moveDir);
		var angle2 = Vector3.Angle(body.forward * -1, moveDir);
		//Debug.Log($"{angle1} : {angle2}");

		if (Math.Abs(angle1 - angle2) < 100f)
		{
			//Debug.Log("same angle");
			if ((lastMoveDir - body.transform.forward).magnitude < 0.1f) //wenn panzer vorwärts gleich bewegungsrichtung ist:
			{
				var rotation1 = Quaternion.LookRotation(moveDir, Vector3.up);
				body.rotation = Quaternion.Lerp(body.rotation, rotation1, rotationSpeed * Time.fixedDeltaTime);
			}
			else
			{
				var rotation2 = Quaternion.LookRotation(moveDir * -1, Vector3.up); //andernfalls:
				body.rotation = Quaternion.Lerp(body.rotation, rotation2, rotationSpeed * Time.fixedDeltaTime);
			}
		}
		else //were e.g moving left right -> base doesnt rotate
		{
			if (angle1 < angle2)
			{
				var rotation1 = Quaternion.LookRotation(moveDir, Vector3.up);
				body.rotation = Quaternion.Lerp(body.rotation, rotation1, rotationSpeed * Time.fixedDeltaTime);
			}
			else
			{
				var rotation2 = Quaternion.LookRotation(moveDir * -1, Vector3.up);
				body.rotation = Quaternion.Lerp(body.rotation, rotation2, rotationSpeed * Time.fixedDeltaTime);
			}

		}
	}
	public bool IsGrounded()
	{
		RaycastHit hit;
		// Does the ray intersect any objects excluding the player layer

		if (Physics.SphereCast(groundCheck.position, 1, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, groundCheckMask))
		{
			float checkY = groundCheck.position.y;
			float hitY = hit.point.y;
			float delta = checkY - hitY;

			//Debug.Log($"hit y delta: {delta} grounded: {delta < groundDistance}"); //-> 1.378

			if (delta > (groundDistance + 0.001f)) //were in the air
			{
				//Debug.Log("return on floating");
				return false;
			}

			if (hit.collider.CompareTag("Wall"))
			{
				//were on a wall
				var dir = transform.position - hit.collider.gameObject.transform.position;

				var temp = dir;
				temp.y = 0;
				temp = temp.normalized;

				dir.x = temp.x;
				dir.z = temp.z;
				dir.y = 1;

				if (dir.x == 0 && dir.z == 0)
				{
					dir.x = 1;
					dir.z = 1;
					Debug.Log("in middle");
				}

				rb.velocity = dir * 4;
				//Debug.Log("return on wall, false");
				return false;
			}

			//Debug.Log("set new ground position");
			var pos = transform.position;
			pos.y += (groundDistance - delta);

			transform.position = pos;

			return true;
		}
		else
		{
			Debug.LogError("Did not Hit");
			var pos = transform.position;
			pos.y = 0.5f + groundDistance;

			transform.position = pos;
			return true;
		}
	}
	public void HandleKnockback(Vector3 origin)
	{
		if (!hasAuthority) { return; }

		//Debug.Log("knockback applied");
		var pos = transform.position;
		pos.y += 0.1f;
		transform.position = pos;

		var dir = (transform.position - origin).normalized;
		dir.y += yBoost;
		rb.AddForce(100 * strength * dir);
	}
	#endregion

	private void HitByBullet(BulletMove bullet)
	{
		CmdPlayParticle();
		healthManager.TakeDamage();
		if (healthManager.GetHealth() <= 0)
		{
			CmdPlayExplosionParticle();
			CmdRaiseKillEvent(bullet);
			Die();
		}
		else
		{
			CmdRaiseHitEvent(bullet);
		}
		Debug.Log("hit by bullet");
	}

	[Command]
	void CmdPlayExplosionParticle()
	{
		RpcPlayExplosionParticle();
	}

	[ClientRpc]
	void RpcPlayExplosionParticle()
	{
		Vector3 pos = this.transform.position;
		pos.y += 0.3f;
		Instantiate(particleExplosion, pos, Quaternion.identity);
	}
	[Command]
	void CmdPlayParticle()
	{
		RpcPlayParticle();
	}

	[ClientRpc]
	private void RpcPlayParticle()
	{
		particleDamage.Clear();
		particleDamage.Play();
	}

	[Command]
	private void CmdRaiseKillEvent(BulletMove bullet)
	{
		bullet.RaiseKillEvent();
	}

	[Command]
	private void CmdRaiseHitEvent(BulletMove bullet)
	{
		bullet.RaiseHitEvent();
	}

	private void Die()
	{
		CmdDie();
	}
	[Command]
	private void CmdDie()
	{
		DeathManager.Died(this);
	}

	[TargetRpc]
	public void BlockInput()
	{
		Debug.Log("blocking input in player");
		InputManager.Add(ActionMapNames.Player);
		InputManager.Controls.PlayerMap.Aim.Enable();
	}

	[TargetRpc]
	public void SetPosition(Vector3 position, Quaternion rotation)
	{
		transform.SetPositionAndRotation(position, rotation);
	}

	[TargetRpc]
	public void Respawn()
	{
		if (!hasAuthority) { return; }

		Debug.Log("player respawn method");
		healthManager.ResetHealth();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!hasAuthority) { return; }

		if (other.gameObject.TryGetComponent(out BulletMove b))
		{
			if (b.originPlayer == this.GetComponent<NetworkIdentity>().netId)
			{
				return;
			}
			HitByBullet(b);
			CmdDestroyObject(other.gameObject);
		}

		if (other.gameObject.TryGetComponent(out PowerUpScript p)) 
		{
			powerUpManager.AddPowerUp(p.GetInfo());
			CmdDestroyObject(p.gameObject);
		}
	}
	[Command]
	private void CmdDestroyObject(GameObject obj)
	{
		if (!obj) { return; }
		NetworkServer.Destroy(obj);
	}
}
