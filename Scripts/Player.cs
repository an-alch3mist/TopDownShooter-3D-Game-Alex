using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

public class Player : MonoBehaviour
{
	[SerializeField] Rigidbody rb;
	[SerializeField] float moveSpeed  =1f;
	[SerializeField] float rotationSpeed = 45f;

	private void Awake()
	{
		Debug.Log("Awake(): " + this);
		Application.targetFrameRate = 60;
	}

	private void Update()
	{
		Vector3 vel = new Vector3()
		{
			x = 0f,
			y = 0f,
			z = Input.GetAxisRaw("Vertical"),
		} * this.moveSpeed;

		transform.localEulerAngles += Vector3.up * rotationSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
		rb.velocity = transform.forward * vel.z;
	}

}
