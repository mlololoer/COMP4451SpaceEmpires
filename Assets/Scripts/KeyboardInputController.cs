using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputController : MonoBehaviour
{

    public float panSpeed = 3.0f;
    public float zoomSpeed = 0.03f;
    // Update is called once per frame
    void Update()
    {
        //1. Handle keyboard camera panning and zooming
    	if (Input.GetKey("page up")) {
    		if (Camera.main.orthographicSize < 10.0f) {
    			Camera.main.orthographicSize += zoomSpeed;
    		}

    	} else if (Input.GetKey("page down")) {
    		if (Camera.main.orthographicSize > 3.0f) {
    			Camera.main.orthographicSize -= zoomSpeed;
    		}
    	}
        this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * panSpeed * Time.deltaTime);
    }
}
