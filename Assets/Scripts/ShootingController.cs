using Cinemachine;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    public Cinemachine3rdPersonAim Aim;
    public Canvas Canvas;

    private bool _isShooting;
    private int _hittableLayerMask;

    public void Shoot(InputAction.CallbackContext context) => _isShooting = !context.canceled;

    void Start()
    {
        _hittableLayerMask = 1 << LayerMask.NameToLayer("Player");
        _hittableLayerMask = ~_hittableLayerMask;
    }

    void FixedUpdate()
    {
        if (_isShooting)
        {
            var ray = GetRayFromTargetingReticle();

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hittableLayerMask))
            {
                HandleHit(hit);
            }
        }
    }

    Ray GetRayFromTargetingReticle()
    {
        var canvasCenter = Canvas.pixelRect.center;
        var aimCenter = Aim.AimTargetReticle.rect.center;
        return Camera.main.ScreenPointToRay(canvasCenter - aimCenter);
    }

    void HandleHit(RaycastHit hit)
    {
        var hittables = hit.collider.gameObject.GetComponents<IHittable>().ToList();
        hittables.ForEach(hittable => hittable.Hit(gameObject));
    }
}