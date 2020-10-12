using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Transform Target;
    public float RotationSpeed = 100;
    public float FollowSpeed = 8;

    [Range(0, 360)] public float MaxUpwardAngle = 65;
    [Range(0, 360)] public float MaxDownwardAngle = 320;

    private Vector3 _targetOffsetToPlayer;
    private float _offsetDistance;
    private Vector2 _movementVector = Vector2.zero;
    private float _initialAngle;
    private float _normalizedMaxDownwardAngle;
    private float _normalizedInitial;

    void Start()
    {
        _targetOffsetToPlayer = Target.position - Player.position;
        _offsetDistance = (Target.position - transform.position).magnitude;
        _initialAngle = transform.eulerAngles.x;
        _normalizedMaxDownwardAngle = NormalizeAngle(MaxDownwardAngle);
        _normalizedInitial = NormalizeAngle(_initialAngle) - _normalizedMaxDownwardAngle;
    }

    void Update()
    {
        MoveFocus();
        MoveCamera();
    }

    public void Look(InputAction.CallbackContext context)
        => _movementVector = context.ReadValue<Vector2>();

    void MoveFocus()
    {
        var focusFuturePosition = Player.position + _targetOffsetToPlayer - Target.position;
        Target.transform.Translate(focusFuturePosition * FollowSpeed * Time.deltaTime, Space.World);
    }

    void MoveCamera()
    {
        var hInput = _movementVector.x;
        var vInput = _movementVector.y;

        RotateAroundTarget(hInput, vInput);
        DistanceFromTarget();
    }

    private void RotateAroundTarget(float hInput, float vInput)
    {
        var rotationDelta = RotationSpeed * Time.deltaTime;

        var futureTargetRotation = Target.rotation.eulerAngles;
        futureTargetRotation += Vector3.up * hInput * rotationDelta;

        var futureAngle = futureTargetRotation.x + vInput;
        if (MaxUpwardAngle >= futureAngle || futureAngle >= MaxDownwardAngle)
            futureTargetRotation += Vector3.right * vInput * rotationDelta;

        futureTargetRotation.z = 0;
        Target.rotation = Quaternion.Euler(futureTargetRotation);
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