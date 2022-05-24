using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
	public CinemachineVirtualCamera[] cams;
	int camIndex = 0;

	private void Start()
	{
		foreach (var cam in cams) cam.Priority = 0;
		cams[camIndex].Priority = 100;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			cams[camIndex].Priority = 0;
			camIndex = (camIndex + 1) % cams.Length;
			cams[camIndex].Priority = 100;
		}
	}
}
