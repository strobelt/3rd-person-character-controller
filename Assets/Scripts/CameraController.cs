using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float RotationSpeed = 1;

    [Range(0, 360)] public float MaxUpwardAngle = 65;
    [Range(0, 360)] public float MaxDownwardAngle = 320;

    private float _offsetDistance;
    private Vector2 _movementVector = Vector2.zero;
    private float _initialAngle;
    private float _normalizedMaxDownwardAngle;
    private float _normalizedInitial;

    void Start()
    {
        _offsetDistance = (Target.position - transform.position).magnitude;
        _initialAngle = transform.eulerAngles.x;
        _normalizedMaxDownwardAngle = NormalizeAngle(MaxDownwardAngle);
        _normalizedInitial = NormalizeAngle(_initialAngle) - _normalizedMaxDownwardAngle;
    }

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
        var horizontalAngle = transform.eulerAngles.x;
        return MaxUpwardAngle < horizontalAngle && horizontalAngle < MaxDownwardAngle;
    }

    private void RotateAroundTarget(float hInput, float vInput)
    {
        transform.RotateAround(Target.position, Vector3.up, hInput * RotationSpeed);
        RotateAroundTargetVertical(vInput);
    }

    private void RotateAroundTargetVertical(float vInput)
    {
        transform.RotateAround(Target.position, -transform.right, vInput * RotationSpeed);
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
        var angleOffset = GetAngleOffset();
        var distance = targetDirection * (angleOffset + _offsetDistance);
        transform.position = Target.position + distance;
    }

    private float GetAngleOffset()
    {
        var normalizedCurrent = NormalizeAngle(transform.eulerAngles.x) - _normalizedMaxDownwardAngle;
        return normalizedCurrent / _normalizedInitial;
    }

    private float NormalizeAngle(float angle)
        => angle > 180 ? angle - 360 : angle;
}