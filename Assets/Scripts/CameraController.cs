﻿using UnityEngine;
using UnityEngine.InputSystem;

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
        Target.position = Player.position + _targetOffsetToPlayer;
    }

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
        Target.Rotate(Vector3.up, hInput * RotationSpeed);
        RotateAroundTargetVertical(vInput);
    }

    private void RotateAroundTargetVertical(float vInput)
    {
        var rot = Target.rotation;
        var angleDelta = vInput * RotationSpeed;
        var hAngle = rot.eulerAngles.x + angleDelta;
        Target.rotation = Quaternion.Euler(hAngle, rot.eulerAngles.y, rot.eulerAngles.z);
        //transform.RotateAround(Target.position, -transform.right, vInput * RotationSpeed);
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