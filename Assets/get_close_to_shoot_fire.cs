using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DigitalRuby.PyroParticles {
public class get_close_to_shoot_fire : MonoBehaviour
{
	private GameObject wayPoint;
	private Vector3 wayPointPos;
	//This will be the AI's speed. Adjust as necessary.
	private float speed = 10.0f;
	private Transform Target;
	private float RotationSpeed = 3.0f;

	//values for internal u
	private Quaternion _lookRotation;
	private Vector3 _direction;
	private float distance;
	private float trail = 8.0f;
	public GameObject[] Prefabs;
	public UnityEngine.UI.Text CurrentItemText;

	private GameObject currentPrefabObject;
	private FireBaseScript currentPrefabScript;
	private int currentPrefabIndex;
	private Quaternion originalRotation;
	
	// Player damage
	private GameObject player;
	private HeliBotController heliBotController;
	
    void Start ()
    {
		//At the start of the game, the zombies will find the gameobject called wayPoint.
		wayPoint = GameObject.Find("wayPoint");
		Target = wayPoint.transform;
		originalRotation = transform.localRotation;
		player = GameObject.FindWithTag("Player");
		heliBotController = player.GetComponent<HeliBotController>();
    }

    void Update ()
    {

    distance = Vector3.Distance(this.transform.position, wayPoint.transform.position);
    //we are too close and do not want to be hit
     wayPointPos = new Vector3(wayPoint.transform.position.x, wayPoint.transform.position.y, wayPoint.transform.position.z);
    if (distance < trail) {
        //Debug.Log("Should be backing up?");
        transform.position = Vector3.MoveTowards(transform.position, wayPointPos, -1 * speed * Time.deltaTime);
    } else {
        transform.position = Vector3.MoveTowards(transform.position, wayPointPos, speed * Time.deltaTime);
    }


      //wayPointRotation = new Quaternion(wayPoint.transform.rotation.x, 0, wayPoint.transform.rotation.z, 0);
      //find the vector pointing from our position to the target
      _direction = (Target.position - transform.position).normalized;

    //create the rotation we need to be in to look at the target
    _lookRotation = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, -90, 0);
    //Debug.Log(_lookRotation);

    //rotate us over time according to speed until we are in the required rotation
    transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    if (distance < trail + 4) {
        //gonna try and shoot some fire!

        if (GameObject.Find("Flamethrower(Clone)") != null) {
            //already exists, lets update location
            GameObject fire = GameObject.Find("Flamethrower(Clone)");
            // Debug.Log("Game object exists");
            fire.transform.position =  transform.position+(transform.right*2)+transform.up*2;
            fire.transform.rotation = _lookRotation * Quaternion.Euler(0, 90, 0);
			if (heliBotController != null) heliBotController.SubtractHealth(.15f);
            //Debug.Log("at pt" + fire.transform.position);
        } else {
            //does not exist so lets make flame
            //Debug.Log("Making flame");
            BeginEffect();
        }
    }
    if (distance > trail + 6) {
        //gonna try and shoot some fire!
        //Debug.Log("I think i wana stop shooting fire?" + Prefabs.Length);
        StopCurrent();
    }
    }


    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }
        if (angle > 360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }

    private void BeginEffect()
    {
        Vector3 pos;
        float yRot = transform.rotation.eulerAngles.y;
        Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Quaternion rotation = Quaternion.identity;
        currentPrefabObject = GameObject.Instantiate(Prefabs[currentPrefabIndex]);
        currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();

        if (currentPrefabScript == null)
        {
            // temporary effect, like a fireball
            currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
            if (currentPrefabScript.IsProjectile)
            {
                // set the start point near the player
                rotation = _lookRotation;
                pos = transform.position + forward + right + up;
            }
            else
            {
                // set the start point in front of the player a ways
                pos = transform.position + (forwardY * 4.0f);
            }
        }
        else
        {
            // set the start point in front of the player a ways, rotated the same way as the player
            pos = transform.position + (forwardY * 2.0f);
            rotation = _lookRotation;
            pos.y = 0.5f;
        }

        FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
        // if (projectileScript != null)
        // {
        //     // make sure we don't collide with other fire layers
        //     projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
        // }

        currentPrefabObject.transform.position = pos;
        currentPrefabObject.transform.rotation = _lookRotation;
    }

    public void StartCurrent()
    {
        StopCurrent();
        BeginEffect();
    }

    private void StopCurrent()
    {
        // if we are running a constant effect like wall of fire, stop it now
        if (currentPrefabScript != null && currentPrefabScript.Duration > 10000)
        {
            currentPrefabScript.Stop();
        }
        currentPrefabObject = null;
        currentPrefabScript = null;
    }

    public void NextPrefab()
    {
        currentPrefabIndex++;
        if (currentPrefabIndex == Prefabs.Length)
        {
            currentPrefabIndex = 0;
        }
    }

    public void PreviousPrefab()
    {
        currentPrefabIndex--;
        if (currentPrefabIndex == -1)
        {
            currentPrefabIndex = Prefabs.Length - 1;
        }
    }

}
}
