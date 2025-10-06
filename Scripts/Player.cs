using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_DrawSystem;

namespace SPACE_CrashCourse
{
	public class Player : MonoBehaviour
	{
		[SerializeField] Rigidbody rb;
		[SerializeField] float moveSpeed = 1f;
		[SerializeField] float rotationSpeed = 45f;

		[SerializeField] Transform tankTowerTr;
		[SerializeField] Transform bulletSpawnPosTr;
		[SerializeField] GameObject pfBullet;
		[SerializeField] Transform targetTr;
		[SerializeField] LayerMask layerMask;

		private void Awake()
		{
			Debug.Log("Awake(): " + this);
			line = new Line(name: "position-line");
			rayLine = new Line(name: "ray-line");
		}

		Line line;
		Line rayLine;

		private void Update()
		{
			Vector3 vel = new Vector3()
			{
				x = 0f,
				y = 0f,
				z = Input.GetAxisRaw("Vertical"),
			} * this.moveSpeed;

			float turn_vel = rotationSpeed * Input.GetAxisRaw("Horizontal");
			if (vel.z < 0f)
				turn_vel *= -1;

			if (vel.z.zero() == false)
				transform.localEulerAngles += Vector3.up * turn_vel * Time.deltaTime;
			rb.velocity = transform.forward * vel.z;

			Vector3 targetVec3 = this.targetTr.position - this.transform.position; targetVec3.y = 0f;
			Quaternion targetRotation = Quaternion.LookRotation(targetVec3);
			this.tankTowerTr.rotation = targetRotation;

			//
			line.a = Vector3.zero;
			line.b = this.transform.position;
			line.e = 1f / 50;

			#region RayCast
			Ray ray = new Ray(this.bulletSpawnPosTr.position, this.bulletSpawnPosTr.forward);
			RaycastHit hit;
			//
			if (Physics.Raycast(ray, out hit, 100f, this.layerMask))
			{
				rayLine.a = ray.origin;
				rayLine.b = hit.point;
				rayLine.e = 1f / 50;

				// fire >>
				if (INPUT.M.InstantDown(0))
				{
					GameObject bullet = GameObject.Instantiate(this.pfBullet, C.PrefabHolder);
					bullet.name = "bullet";
					bullet.transform.position = this.bulletSpawnPosTr.position;
					bullet.transform.rotation = this.bulletSpawnPosTr.rotation;
					Launch(bullet, this.bulletSpeed);
				}
				// << fire
			}
			else
			{
				rayLine.Clear();
			}
			#endregion
		}


		float bulletSpeed = 1000f;
		static void Launch(GameObject objBullet, float speed)
		{
			Rigidbody rb = objBullet.GC<Rigidbody>();
			rb.AddForce(objBullet.transform.forward * speed);
		}

	}
}