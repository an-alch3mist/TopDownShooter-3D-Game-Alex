using UnityEngine;
using System.Collections;

using SPACE_UTIL;

namespace SPACE_TopDownShooter
{
	public class PlayerAim : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;
		[SerializeField] LayerMask _aimLayer;
		[SerializeField] Transform _aimTr;
	}
}