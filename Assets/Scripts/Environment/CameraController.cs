using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Camera _cam;

	private void Start()
	{
		_cam = GetComponent<Camera>();
	}
}
