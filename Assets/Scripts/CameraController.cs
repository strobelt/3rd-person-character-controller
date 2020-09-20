using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float RotationSpeed = 1;

    private float _offsetDistance;

    void Start() => _offsetDistance = (Target.position - transform.position).magnitude;

    public void Look(InputAction.CallbackContext context)
    {
        var movementVector = context.ReadValue<Vector2>();

        MoveCamera(movementVector.x, movementVector.y);
    }

    void MoveCamera(float hInput, float vInput)
    {
        Debug.Log($"X: {hInput} Y: {vInput}");

        transform.RotateAround(Target.position, Vector3.up, hInput * RotationSpeed);
        transform.RotateAround(Target.position, Vector3.left, vInput * RotationSpeed);
        var angles = transform.localEulerAngles;
        angles.z = 0;
        transform.localEulerAngles = angles;
        transform.LookAt(Target);

        var targetDirection = (transform.position - Target.position).normalized;
        var distance = targetDirection * _offsetDistance;
        transform.position = Target.position + distance;
    }
}