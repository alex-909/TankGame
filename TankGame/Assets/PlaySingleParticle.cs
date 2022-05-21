using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlaySingleParticle : MonoBehaviour
{
	[SerializeField] private ParticleSystem particle;
	private void Awake()
	{
		particle.Play();
	}
	void Update()
    {
		if (!particle.isPlaying) 
		{
			Destroy(this.gameObject);
		}
    }
}
