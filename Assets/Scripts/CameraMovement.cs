using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public float panSpeed = 3.0f;
    public float zoomSpeed = 0.03f;

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
        
        //2. Handle clicking map
        if (Input.GetMouseButtonUp(0)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -this.transform.position.z;
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll (Camera.main.ScreenPointToRay(mousePos), Mathf.Infinity);
            List<GameObject> hitObjects = new List<GameObject>();
            foreach (var hit in hits) {
                if (hit.collider != null) {
                    hitObjects.Add(hit.collider.gameObject.transform.parent.gameObject);
                }
            }
            SelectionManager.SM.Select(hitObjects);
        }
    }
}
