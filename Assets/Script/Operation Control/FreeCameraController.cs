using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
	[Range(1f, 10f)]
	public float MovementSpeed = 2f;

	[Range(1, 500f)]
	public float LookSensitivity = 200f;

	[Range(1, 500f)]
	public float MouseSensitivity = 3;

    [SerializeField]
	Transform cameraTransform;

	Vector2 look;

	private bool enterUI;

    private void Start()
    {

	}



	void Update() 
	{
		enterUI = DetectMouse.enterUI;
        if (!enterUI)
        {
			UpdateMovement();
			UpdateLook();
			UpdateUpDown();
		}
		
	}

    private void UpdateMovement()
    {
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");

		var input = new Vector3();
		input += transform.forward * y;
		input += transform.right * x;
		input = Vector3.ClampMagnitude(input, 1f);

		transform.Translate(input * MovementSpeed * Time.deltaTime, Space.World);
    }

    void UpdateLook()
    {
		look.x += Input.GetAxis("Mouse X");
		look.y += Input.GetAxis("Mouse Y");

		look.y = Mathf.Clamp(look.y, -89f, 89f);

		cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
		transform.localRotation = Quaternion.Euler(0, look.x, 0);
	}

	void UpdateUpDown()
	{
		var x = Input.GetAxis("MoveUD");
	
		//Debug.Log(Input.GetAxis("MoveUD"));
		var input = new Vector3();
		input += transform.up * x;
		input = Vector3.ClampMagnitude(input, 1f);
		if (Input.GetKeyDown(KeyCode.LeftShift))
			MovementSpeed *= 4;
		if (Input.GetKeyUp(KeyCode.LeftShift))
			MovementSpeed /= 4;
		transform.Translate(input* MovementSpeed * Time.deltaTime);
	}
}
