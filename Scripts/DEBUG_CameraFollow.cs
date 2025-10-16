using UnityEngine;
using SPACE_UTIL;

public class DEBUG_CameraFollow : MonoBehaviour
{
	[SerializeField] Transform _followTr;
	private Vector3 _offset;

	private void Start()
	{
		// Calculate offset once at start
		_offset = transform.position - _followTr.position;
	}

	private void LateUpdate()
	{
		// Update camera position in LateUpdate - this is the Unity-standard timing for cameras
		transform.position = _followTr.position + _offset;
	}
}