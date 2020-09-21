using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float RotationSpeed = 1;

    private float _offsetDistance;
    private Vector2 _movementVector = Vector2.zero;

    void Start() => _offsetDistance = (Target.position - transform.position).magnitude;

    void Update() => MoveCamera();

    public void Look(InputAction.CallbackContext context)
        => _movementVector = context.ReadValue<Vector2>();

    void MoveCamera()
    {
        var hInput = _movementVector.x;
        var vInput = _movementVector.y;

        transform.RotateAround(Target.position, Vector3.up, hInput * RotationSpeed);
        transform.RotateAround(Target.position, Vector3.left, vInput * -RotationSpeed);
        var angles = transform.localEulerAngles;
        angles.z = 0;
        transform.localEulerAngles = angles;
        transform.LookAt(Target);

        var targetDirection = (transform.position - Target.position).normalized;
        var distance = targetDirection * _offsetDistance;
        transform.position = Target.position + distance;
    }
}