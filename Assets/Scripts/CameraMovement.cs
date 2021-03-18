using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public Transform cameraTransform;
    float speed = 10.0f;
    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        cameraTransform.position += move * speed * Time.deltaTime;
    }
}
