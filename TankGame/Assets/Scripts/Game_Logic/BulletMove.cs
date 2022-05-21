using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using DapperDino.Tutorials.Lobby;

public class BulletMove : NetworkBehaviour
{
	public readonly SyncList<Vector3> points = new SyncList<Vector3>();

	[SyncVar] public uint originPlayer;
	
	[SerializeField] private float speed;
	private Rigidbody rb;
	public Vector3 velocity = Vector3.zero;
	private Vector3 target;
	private int index = 0;

	public event Action KilledPlayer;
	public event Action HitPlayer;


	public void RaiseKillEvent() 
	{
		if (this.KilledPlayer == null) { return; }
		Debug.Log("raise kill event");
		this.KilledPlayer();
	}
	public void RaiseHitEvent()
	{
		if (this.HitPlayer == null) { return; }
		Debug.Log("raise hit event");
		this.HitPlayer();
	}
	public override void OnStartClient() 
	{
		rb = this.GetComponent<Rigidbody>();
		target = points[0];

		transform.forward = (target - transform.position).normalized;
		this.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
	}
	
	private void FixedUpdate()
	{
		FollowPath();
		#region old
		/*
		rb.velocity = moveVector.normalized * speed;
		rb.angularVelocity = Vector3.zero;
		transform.forward = rb.velocity.normalized;
		this.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
		moveVector = rb.velocity.normalized;
		*/
		#endregion
	}
	void FollowPath() 
	{
		//check if you are close to a target
		if ((transform.position - target).magnitude < 0.1f)
		{
			//Debug.Log("close");
			index++;
			if (index > (points.Count - 1))
			{
				Destroy(this.gameObject);
			}
			else 
			{
				target = points[index];
			}
		}

		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
		transform.forward = (target - transform.position).normalized;
		this.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<BulletMove>( out BulletMove b)) { return; }

		Debug.Log("Collided!");
		NetworkServer.Destroy(other.gameObject);
		NetworkServer.Destroy(this.gameObject);
	}
}
