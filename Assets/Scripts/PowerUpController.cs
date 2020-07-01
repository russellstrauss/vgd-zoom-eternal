using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference https://www.youtube.com/watch?v=CLSiRf_OrBk
// https://answers.unity.com/questions/1638885/how-to-destroy-a-clone-of-an-effect-attached-to-an.html
public class PowerUpController : MonoBehaviour
{
	public GameObject pickupEffect;
	public float duration = 2;
	public float spinForce = 200.0f;
	private GameObject player;
	private HeliBotController heliBotController;

	void Start()
	{
		player = GameObject.FindWithTag("Player");
		heliBotController = player.GetComponent<HeliBotController>();
	}
	
	void Update()
	{
		transform.Rotate(0, spinForce * Time.deltaTime, 0);
	}

    void OnTriggerEnter(Collider other)
    {
    	if (other.gameObject.CompareTag("playerCollider")) {
    		StartCoroutine(Pickup(other));
			FindObjectOfType<AudioManager>().Play("ting");
			heliBotController.setBotSpeed(heliBotController.botMovementSpeed + 500);
    	}
    }
	
    IEnumerator Pickup(Collider other)
    {
		GameObject clone = Instantiate(pickupEffect, transform.position, transform.rotation);
		player.GetComponent<HeliBotController>().AddHealth(100);
		
		// remove the effect from the player
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;

		yield return new WaitForSeconds(duration);
		//wait x amount of seconds

		// remove power up object
		ParticleSystem.MainModule particle = clone.GetComponent<ParticleSystem>().main;
		Destroy(clone, particle.duration);
		Destroy(gameObject);
    }
}
