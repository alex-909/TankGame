using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
	private Transform start;

	private void Awake()
	{
		lr = this.GetComponent<LineRenderer>();
		lr.enabled = false;
	}
	public void SetUpLine(Transform startPos) 
	{
		start = startPos;
	}
	public void DisplayLine(Vector3 end) 
	{
		lr.SetPosition(0, start.position);
		lr.SetPosition(1, end);
	}
	public void SetLineEnabled(bool state) 
	{
		lr.enabled = state;
	}
}
