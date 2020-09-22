using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float RotationSpeed = 1;

    [Range(0, 360)]
    public float MaxUpwardAngle = 65;
    [Range(0, 360)]
    public float MaxDownwardAngle = 320;

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

        RotateAroundTarget(hInput, vInput);
        LookAtTarget();

        if (HorizontalAngleIsOutOfBounds())
        {
            RotateAroundTargetVertical(-vInput);
            LookAtTarget();
        }

        DistanceFromTarget();
    }

    private bool HorizontalAngleIsOutOfBounds()
    {
        var x = transform.eulerAngles.x;
        return MaxUpwardAngle < x && x < MaxDownwardAngle;
    }

    private void RotateAroundTarget(float hInput, float vInput)
    {
        transform.RotateAround(Target.position, Vector3.up, hInput * RotationSpeed);
        RotateAroundTargetVertical(vInput);
    }

    private void RotateAroundTargetVertical(float vInput)
    {
        transform.RotateAround(Target.position, Vector3.left, vInput * RotationSpeed);
    }

    private void LookAtTarget()
    {
        var angles = transform.localEulerAngles;
        angles.z = 0;
        transform.localEulerAngles = angles;
        transform.LookAt(Target);
    }

    private void DistanceFromTarget()
    {
        var targetDirection = (transform.position - Target.position).normalized;
        var distance = targetDirection * _offsetDistance;
        transform.position = Target.position + distance;
    }
}