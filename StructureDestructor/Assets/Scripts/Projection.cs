using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
	[SerializeField] private LineRenderer _line;
	[SerializeField] private int _maxPhysicsFrameIterations = 100;
	[SerializeField] private Transform _obstaclesParent;

	private Scene _simulationScene;
	private PhysicsScene _physicsScene;
	private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

	private GameObject _ghostObj;

	public void CreatePhysicsScene(GameObject ballPrefab)
	{
		_ghostObj = Instantiate(ballPrefab);
		_ghostObj.GetComponent<Renderer>().enabled = false;
		_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
		_physicsScene = _simulationScene.GetPhysicsScene();

		foreach (Transform obj in _obstaclesParent)
		{
			var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
			ghostObj.GetComponent<Renderer>().enabled = false;
			SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
			if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
			if (ghostObj.TryGetComponent(out Destructable destructable)) Destroy(destructable);
		}
	}

	private void Update()
	{
		foreach (var item in _spawnedObjects)
		{
			if (item.Value == null || item.Key == null) _spawnedObjects.Remove(item.Key);
			item.Value.position = item.Key.position;
			item.Value.rotation = item.Key.rotation;
		}
	}

	public void SimulateTrajectory(Transform ballSpawn, Vector3 velocity)
	{
		_ghostObj.transform.position = ballSpawn.position;
		_ghostObj.transform.rotation = ballSpawn.rotation;
		SceneManager.MoveGameObjectToScene(_ghostObj.gameObject, _simulationScene);

		var rb = _ghostObj.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;
		rb.AddForce(velocity, ForceMode.Impulse);

		_line.positionCount = _maxPhysicsFrameIterations;

		for (var i = 0; i < _maxPhysicsFrameIterations; i++)
		{
			_physicsScene.Simulate(Time.fixedDeltaTime);
			_line.SetPosition(i, _ghostObj.transform.position);
		}
	}
}