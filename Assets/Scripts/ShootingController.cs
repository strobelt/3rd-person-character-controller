using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    public GameObject Bullet;
    public Cinemachine3rdPersonAim Aim;
    public RectTransform TargetingReticle;

    private bool _isShooting;
    private int _hittableLayerMask;
    private Camera _mainCamera;
    private int _displayWidth, _displayHeight;

    private readonly Color[] _colors =
    {
        Color.red, Color.blue, Color.yellow, Color.black, Color.green
    };

    public void Shoot(InputAction.CallbackContext context) => _isShooting = !context.canceled;

    void Start()
    {
        _hittableLayerMask = 1 << LayerMask.NameToLayer("Player");
        _hittableLayerMask = ~_hittableLayerMask;

        _mainCamera = Camera.main;
        var canvas = TargetingReticle.GetComponentInParent<Canvas>();
        _displayWidth = canvas.worldCamera.pixelWidth;
        _displayHeight = canvas.worldCamera.pixelHeight;
    }

    void FixedUpdate()
    {
        if (_isShooting)
        {
            var ray = GetRayFromTargetingReticle();

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hittableLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
                Debug.Log("Did Hit");
                HandleHit(hit);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    Ray GetRayFromTargetingReticle()
    {
        var posX = TargetingReticle.transform.position.x + _displayWidth / 2f;
        var posY = TargetingReticle.transform.position.y + _displayHeight / 2f;
        return Camera.main.ScreenPointToRay(new Vector3(posX, posY, 0));
    }

    void HandleHit(RaycastHit hit)
    {
        var random = new System.Random();
        var randomColor = _colors[random.Next(0, _colors.Length - 1)];
        hit.transform.gameObject.GetComponent<Renderer>().material.color = randomColor;
    }
}