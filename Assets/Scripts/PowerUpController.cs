using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference https://www.youtube.com/watch?v=CLSiRf_OrBk
// https://answers.unity.com/questions/1638885/how-to-destroy-a-clone-of-an-effect-attached-to-an.html
public class PowerUpController : MonoBehaviour
{
	public GameObject pickupEffect;
	public float duration = 0.0f;
	public float spinForce = 200.0f;
	private GameObject player;
	private HeliBotController heliBotController;
	public bool moveToLeft = true;
	public float cur_x;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
		cur_x = transform.position.x;
		if (player != null) heliBotController = player.GetComponent<HeliBotController>();
	}
	
	void Update()
	{
		// transform.Rotate(0, spinForce * Time.deltaTime, 0);
		// transform.position += transform.forward * Time.deltaTime;
		// transform.Translate(Vector3.right * Time.deltaTime, Space.Self);
		Move();
	}

    void OnTriggerEnter(Collider other)
    {
    	if (other.gameObject.CompareTag("playerCollider")) {
    		StartCoroutine(Pickup(other));
			FindObjectOfType<AudioManager>().Play("robust-beep");
			IncreaseSpeed(100);
    	}
		Debug.Log(gameObject.name == "PowerUpCube");
		if (gameObject.name == "PowerUpCube" && other.gameObject.CompareTag("playerCollider")) {
			FindObjectOfType<PlayerController>().AddHealth(250);
		}
    }
	
    IEnumerator Pickup(Collider other)
    {
		GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
		// player.GetComponent<HeliBotController>().AddHealth(50);
		
		// remove the effect from the player
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;

		yield return new WaitForSeconds(duration);
		//wait x amount of seconds

		// remove power up object
		ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
		Destroy(clone, duration);
		Destroy(gameObject);
    }
	
	void IncreaseSpeed(int speed) {
		if (heliBotController != null) heliBotController.SetBotSpeed(heliBotController.botMovementSpeed + speed);
	}

	private void Move()
    {
		if (transform.position.x <= cur_x -3 && moveToLeft)
        {
            moveToLeft = false;
        }

        else if (transform.position.x >= cur_x+3 && !moveToLeft)
		{
			moveToLeft = true;
		}
        transform.position += (moveToLeft ? Vector3.left : Vector3.right) * Time.deltaTime ;
    }
}
