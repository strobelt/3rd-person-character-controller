using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;

    private Vector3 offset;
    private float offsetDistance;
    private Vector3 velocity = Vector3.one;
    private float heading = 0;

    private float rotationSpeed = 1;

    void Start()
    {
        offset = transform.position;
        offsetDistance = (target.position - transform.position).magnitude;
    }

    void LateUpdate()
    {
        var hInput = Input.GetAxis("Mouse X");
        var vInput = Input.GetAxis("Mouse Y");
        Debug.Log($"X: {hInput} Y: {vInput}");

        transform.RotateAround(target.position, Vector3.up, hInput * rotationSpeed);
        transform.RotateAround(target.position, Vector3.left, vInput * rotationSpeed);
        var angles = transform.localEulerAngles;
        angles.z = 0;
        transform.localEulerAngles = angles;
        transform.LookAt(target);

        var targetDirection = (transform.position - target.position).normalized;
        var x = targetDirection * offsetDistance;
        transform.position = target.position + x;
    }

    void OldLateUpdate()
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