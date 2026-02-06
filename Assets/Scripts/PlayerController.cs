using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public float xSense = 1f;
	public float ySense = 1f;
	public float moveSpeed = 5f;
	public float sprintMultiplier = 5f;
	public float jumpPower = 5f;
    public GameObject bullet;
    public GameObject rocket;
    public GameObject flameOrb;
	float xRotation = 0f;
	float yRotation = 0f;
	Camera camera;
	CharacterController controller;
	Vector3 fallSpeed = new Vector3(0, 0, 0);
	float sprintMult;

    // Start is called before the first frame update
    void Start()
    {
    	camera = Camera.main;
    	controller = GetComponent<CharacterController>();
    	Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
    	//#### MOVEMENT
    	if (controller.isGrounded && fallSpeed.y < 0){
    		fallSpeed.y = -0.5f;
    	}

    	//if (Input.GetMouseButton(1)){
        	float mouseX = Input.GetAxis("Mouse X") * xSense;
        	float mouseY = Input.GetAxis("Mouse Y") * ySense;

        	yRotation += mouseY;
        	xRotation += mouseX;
        	camera.transform.eulerAngles = new Vector3(-yRotation, xRotation, 0f);
        //}

        if (Input.GetButtonDown("Jump") && controller.isGrounded){
        	fallSpeed.y += jumpPower;
        }

        sprintMult = 1f;
        if (Input.GetKey(KeyCode.LeftShift)){
        	sprintMult = 5f;
        }
        //Debug.Log(controller.isGrounded);

        fallSpeed.y += -10f * Time.deltaTime;
        Vector3 movement = (camera.transform.TransformDirection(Vector3.forward) * Input.GetAxisRaw("Vertical")) + (camera.transform.TransformDirection(Vector3.right) * Input.GetAxisRaw("Horizontal"));
        movement.y = 0;
        controller.Move(movement.normalized * moveSpeed * Time.deltaTime * sprintMult + fallSpeed * Time.deltaTime);


        if(Input.GetKeyDown("e")){ // laser 
            GameObject bulletClone = Instantiate(bullet, camera.transform.TransformPoint(Vector3.forward*1.9f) - new Vector3(0,0.9f,0),camera.transform.rotation * Quaternion.Euler(90,0,0));
            (bulletClone.GetComponent(typeof(Rigidbody)) as Rigidbody).AddForce(camera.transform.TransformDirection(Vector3.forward*100),ForceMode.VelocityChange);
        }

        if(Input.GetKeyDown("n")){ // nuke
            GameObject rocketClone = Instantiate(rocket, camera.transform.TransformPoint(Vector3.forward*3f) - new Vector3(0,0.9f,0),camera.transform.rotation * Quaternion.Euler(0,0,0));
            (rocketClone.GetComponent(typeof(Rigidbody)) as Rigidbody).AddForce(camera.transform.TransformDirection(Vector3.forward*60),ForceMode.VelocityChange);
        }

        if(Input.GetKeyDown("r")){ // flame orb
            GameObject flameOrbClone = Instantiate(flameOrb, camera.transform.TransformPoint(Vector3.forward*1.9f) - new Vector3(0,0.9f,0),camera.transform.rotation * Quaternion.Euler(0,0,0));
            (flameOrbClone.GetComponent(typeof(Rigidbody)) as Rigidbody).AddForce(camera.transform.TransformDirection(Vector3.forward*60),ForceMode.VelocityChange);
        }
    }
}
