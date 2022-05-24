using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
	[Tooltip("The velocity at which something has to hit this in order to destroy it.")]
	public float fragility;

	private void OnCollisionEnter(Collision collision)
	{
		float velocity = collision.relativeVelocity.magnitude;
		if (velocity > fragility)
		{
			Destroy(gameObject, 0.1f);
		}
	}
}
