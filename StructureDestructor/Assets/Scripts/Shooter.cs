using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Shooter : MonoBehaviour
{
	[Header("Shooting")]
	public GameObject projectile;
	public Transform spawnLocation;
	public float power;
	[Header("Rotation")]
	public float rotationIncrement;
	public float rotationSpeed;
	public float horizontalSpeed;
	public float verticalSpeed;
	public float topXAngle = 315;   // 360 - 315 = 45 degrees up
	public float bottomXAngle = 15;   // 15 degrees down
	public float minYAngle = 315;	// 360 - 45 = 45 degrees left
	public float maxYAngle = 45;	// 45 degrees right
	public GameObject globalPivot;
	public GameObject localPivot;
	[Header("Mouse Aiming")]
	public LayerMask aimTarget;
	public GameObject target;
	[Header("Cameras")]
	public new Camera camera;

	private bool canMoveOrShoot = true;
	private float rotationBuffer = 10f;

	private void Update()
	{
		GetPlayerInput();
	}

	private void GetPlayerInput()
	{
		print(localPivot.transform.rotation.eulerAngles);

		if (!canMoveOrShoot) return;
		// Cannon rotation around the center
		if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(Rotate(-rotationIncrement));
		if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(Rotate(rotationIncrement));

		// Cannon aiming
		if (Input.GetKey(KeyCode.A) &&
			(localPivot.transform.rotation.eulerAngles.y > minYAngle || localPivot.transform.rotation.eulerAngles.y < maxYAngle + rotationBuffer))
		{ localPivot.transform.Rotate(0, -verticalSpeed, 0, Space.World); }
		if (Input.GetKey(KeyCode.D)
			&& (localPivot.transform.rotation.eulerAngles.y > minYAngle - rotationBuffer || localPivot.transform.rotation.eulerAngles.y < maxYAngle))
		{ localPivot.transform.Rotate(0, verticalSpeed, 0, Space.World); }
		if (Input.GetKey(KeyCode.W)
			&& (localPivot.transform.rotation.eulerAngles.x > topXAngle || localPivot.transform.rotation.eulerAngles.x < bottomXAngle + rotationBuffer))
		{ localPivot.transform.Rotate(-horizontalSpeed, 0, 0, Space.Self); }
		if (Input.GetKey(KeyCode.S)
			&& (localPivot.transform.rotation.eulerAngles.x < bottomXAngle || localPivot.transform.rotation.eulerAngles.x > topXAngle - rotationBuffer))
		{ localPivot.transform.Rotate(horizontalSpeed, 0, 0, Space.Self); }

		// Mouse aiming
		if (Input.GetKey(KeyCode.Mouse0))
		{
			//print(Input.mousePosition);
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, 100, aimTarget))
			{
				target.transform.position = hit.point;
				localPivot.transform.LookAt(hit.point);
			}
		}

		// Shoot
		if (Input.GetKeyDown(KeyCode.Space)) Shoot();
	}

	public void Shoot()
	{
		var bullet = Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);
		bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.rotation * (Vector3.forward * power), ForceMode.Impulse);
	}

	public void SetYAxis(float y)
	{
		localPivot.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);
	}

	public void SetXAxis(float x)
	{
		localPivot.transform.rotation = Quaternion.Euler(x, transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.z);
	}

	public IEnumerator Rotate(float rI)
	{
		canMoveOrShoot = false;
		//globalPivot.transform.Rotate(0, -rotationSpeed, 0);
		var startRotation = globalPivot.transform.rotation;
		var endRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rI, 0);
		float n = 0;
		while (n < 1)
		{
			globalPivot.transform.rotation = Quaternion.Slerp(startRotation, endRotation, n += rotationSpeed);
			yield return new WaitForSeconds(0.02f);
		}
		canMoveOrShoot = true;
	}
}
