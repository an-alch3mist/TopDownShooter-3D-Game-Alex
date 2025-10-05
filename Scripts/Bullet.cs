using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

public class Bullet : MonoBehaviour
{
	public float speed = 10f;

	public void Launch()
	{
		Rigidbody rb = this.gameObject.GC<Rigidbody>();
		rb.AddForce(this.transform.forward * this.speed);
	}

	float timer = 0f;
	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 5f)
			GameObject.Destroy(this.gameObject);
	}
}
