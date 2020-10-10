using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Transform Target;
    public float RotationSpeed = 1;

    [Range(0, 360)] public float MaxUpwardAngle = 65;
    [Range(0, 360)] public float MaxDownwardAngle = 320;

    private Vector3 _targetOffsetToPlayer;
    private float _offsetDistance;
    private Vector2 _movementVector = Vector2.zero;
    private float _initialAngle;
    private float _normalizedMaxUpwardAngle;
    private float _normalizedMaxDownwardAngle;
    private float _normalizedInitial;

    void Start()
    {
        _targetOffsetToPlayer = Target.position - Player.position;
        _offsetDistance = (Target.position - transform.position).magnitude;
        _initialAngle = transform.eulerAngles.x;
        _normalizedMaxUpwardAngle = NormalizeAngle(MaxUpwardAngle);
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
        Target.position = Player.position + _targetOffsetToPlayer;
    }

    void MoveCamera()
    {
        var hInput = _movementVector.x;
        var vInput = _movementVector.y;

        RotateAroundTarget(hInput, vInput);
        DistanceFromTarget();
    }

    private bool HorizontalAngleIsOutOfBounds()
    {
        var horizontalAngle = transform.eulerAngles.x;
        return MaxUpwardAngle < horizontalAngle && horizontalAngle < MaxDownwardAngle;
    }

    private void RotateAroundTarget(float hInput, float vInput)
    {
        var rot = Target.rotation.eulerAngles;
        rot += Vector3.up * hInput;

        var futureAngle = rot.x + vInput;
        if (futureAngle <= MaxUpwardAngle || futureAngle >= MaxDownwardAngle)
            rot += Vector3.right * vInput;
        rot.z = 0;
        Target.rotation = Quaternion.Euler(rot);
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