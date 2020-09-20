using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public float smoothSpeed = 10f;

    private Vector3 offset; // Probably what I need to change using the mouse or right stick
    private Vector3 velocity = Vector3.one;
    private float heading = 0;

    void Start()
    {
        offset = transform.position;
    }

    void LateUpdate()
    {
        var desiredPosition = target.position + offset;
        var smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity,
            smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        heading += Input.GetAxis("Mouse X") * Time.deltaTime * 10;

        Quaternion.Euler(0, heading, 0).ToAngleAxis(out float angle, out Vector3 axis);
        transform.RotateAround(target.position, axis, angle);
        transform.LookAt(target);

        //var desiredPosition = target.position + offset;
        //var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        //transform.position = smoothedPosition;
    }
}
