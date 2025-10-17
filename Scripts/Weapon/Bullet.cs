using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;

namespace SPACE_TopDownShooter
{
	public class Bullet : MonoBehaviour
	{
		//
		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log( ("OnCollisionEnter()" + this).colorTag("orange"));
			Rigidbody rb = this.gameObject.GC<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
		}
	}
}