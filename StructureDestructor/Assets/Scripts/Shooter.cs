using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class Shooter : MonoBehaviour
{
	[Header("Shooting")]
	public GameObject projectile;
	public Transform spawnLocation;
	public float power;
	public float maxPower;
	[Header("Rotation")]
	public float rotationIncrement;
	public float rotationSpeed;
	public float horizontalSpeed;
	public float verticalSpeed;
	public float topXAngle = 315;   // 360 - 315 = 45 degrees up
	public float bottomXAngle = 15; // 15 degrees down
	public float minYAngle = 315;   // 360 - 315 = 45 degrees left
	public float maxYAngle = 45;    // 45 degrees right
	public GameObject globalPivot;
	public GameObject localPivot;
	[SerializeField] private Projection _projection;
	[Header("Mouse Aiming")]
	public LayerMask aimTarget;
	public GameObject target;
	[Space]
	public new Camera camera;
	public TMP_Text powerDisplay;

	private bool canMoveOrShoot = true;
	private float rotationBuffer = 10f;
	private float powerChangeSpeed = 3;
	private float powerChangeTime;

	private void Start()
	{
		_projection.CreatePhysicsScene(projectile);
	}

	private void Update()
	{
		GetPlayerInput();
		_projection.SimulateTrajectory(spawnLocation, spawnLocation.forward * power);
	}

	private void GetPlayerInput()
	{
		//print(localPivot.transform.rotation.eulerAngles);

		if (!canMoveOrShoot) return;
		// Cannon rotation around the center
		if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(Rotate(-rotationIncrement));
		if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(Rotate(rotationIncrement));

		// Cannon aiming
		if (Input.GetKey(KeyCode.A) &&
			(localPivot.transform.localRotation.eulerAngles.y > minYAngle || localPivot.transform.localRotation.eulerAngles.y < maxYAngle + rotationBuffer))
		{ localPivot.transform.Rotate(0, -verticalSpeed, 0, Space.World); }
		if (Input.GetKey(KeyCode.D)
			&& (localPivot.transform.localRotation.eulerAngles.y > minYAngle - rotationBuffer || localPivot.transform.localRotation.eulerAngles.y < maxYAngle))
		{ localPivot.transform.Rotate(0, verticalSpeed, 0, Space.World); }
		if (Input.GetKey(KeyCode.W)
			&& (localPivot.transform.localRotation.eulerAngles.x > topXAngle || localPivot.transform.localRotation.eulerAngles.x < bottomXAngle + rotationBuffer))
		{ localPivot.transform.Rotate(-horizontalSpeed, 0, 0, Space.Self); }
		if (Input.GetKey(KeyCode.S)
			&& (localPivot.transform.localRotation.eulerAngles.x < bottomXAngle || localPivot.transform.localRotation.eulerAngles.x > topXAngle - rotationBuffer))
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

		powerChangeTime += Time.deltaTime;
		// Power
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			power = Mathf.Min(power + 1, maxPower);
			powerChangeTime = 1;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			power = Mathf.Min(power + powerChangeTime * powerChangeSpeed * Time.deltaTime, maxPower);
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			power = Mathf.Round(power);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			power = Mathf.Max(power - 1, 1);
			powerChangeTime = 1;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			power = Mathf.Max(power - powerChangeTime * powerChangeSpeed * Time.deltaTime, 1);
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			power = Mathf.Round(power);
		}

		powerDisplay.text = power.ToString("#");

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
			yield return new WaitForFixedUpdate();
		}
		canMoveOrShoot = true;
	}

	public void RotateRight()
	{
		StartCoroutine(Rotate(-rotationIncrement));
	}

	public void RotateLeft()
	{
		StartCoroutine(Rotate(rotationIncrement));
	}
}
