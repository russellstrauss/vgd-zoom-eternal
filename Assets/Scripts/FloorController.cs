using UnityEngine;
using System.Collections;

public class FloorController : MonoBehaviour {

	public float rotationSpeed = 100.0f;

	private Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void Update()
	{
		float hRotation = Input.GetAxis("Horizontal") * rotationSpeed;
		float vRotation = Input.GetAxis("Vertical") * rotationSpeed;
		hRotation *= Time.deltaTime;
		vRotation *= Time.deltaTime;
		transform.Rotate(0, 0, hRotation);
		transform.Rotate(vRotation, 0, 0);
	}
}