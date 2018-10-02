using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    /*private Transform lookAt;
    private Vector3 startOffset;
    private Vector3 moveVector;*/

    public Transform lookAt; // object we are looking at
    public Vector3 offset = new Vector3(0, 2.5f, -3.5f);

    // Use this for initialization
    void Start ()
    {
        /*lookAt = GameObject.FindGameObjectWithTag("Player").transform;
        startOffset = transform.position - lookAt.position;*/

        transform.position = lookAt.position + offset;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = lookAt.position + offset;
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);


        /*// Modified to allow camera to follow player's left/right movement without rotating
        moveVector.x = lookAt.position.x + startOffset.x;
        moveVector.z = lookAt.position.z + startOffset.z;
        // keep y to be the same; prevent rotation
        transform.position = new Vector3(moveVector.x, transform.position.y, moveVector.z);
        */


        /*// To keep camera at center of screen; does not follow player's left/right movement
        moveVector = lookAt.position + startOffset;
        // X
        moveVector.x = 0;
        // Y
        moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);
        transform.position = moveVector;*/
    }
}
