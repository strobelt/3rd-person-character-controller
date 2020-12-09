using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DamageableModifier : MonoBehaviour, IHittable
{
    public int Toughness = 100;
    public int DamageResistance = 3;
    public bool DecreaseOnHit = true;
    public bool Destroyable = true;
    public float TimeToHideHealthBar = 2;

    public UnityEvent<GameObject, int> OnHit;
    public UnityEvent<GameObject> OnToughnessZero;

    private const int NoDamage = 0;
    private Slider _healthBar;

    private int _currentToughness;
    private Canvas _healthCanvas;
    private Coroutine _hidingHealthBar;

    private int CurrentToughness
    {
        get => _currentToughness;
        set
        {
            _currentToughness = value;
            if (_healthBar != null)
                _healthBar.value = _currentToughness;
        }
    }

    void Start()
    {
        CreateHealthBar();
        if (_healthCanvas)
        {
            _healthCanvas.GetComponent<CanvasGroup>().alpha = 0;
            _healthBar = _healthCanvas.GetComponentInChildren<Slider>();
            _healthBar.minValue = 0;
            _healthBar.maxValue = Toughness;
        }

        CurrentToughness = Toughness;
    }

    private void CreateHealthBar()
    {
        var healthCanvasPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/HealthCanvas"));
        healthCanvasPrefab.transform.SetParent(transform, true);
        var offset = new Vector3(0, .7f, 0);
        healthCanvasPrefab.transform.SetPositionAndRotation(transform.position + offset, Quaternion.identity);

        _healthCanvas = healthCanvasPrefab.GetComponent<Canvas>();
    }

    public void Hit(GameObject hitter, int shootingDamage)
    {
        var partDamage = CalculateDamage(shootingDamage);

        _healthCanvas.GetComponent<CanvasGroup>().alpha = 1;
        if (_hidingHealthBar != null) StopCoroutine(_hidingHealthBar);
        _hidingHealthBar = StartCoroutine(HideHealthBar());

        OnHit?.Invoke(gameObject, partDamage);

        if (DecreaseOnHit)
            CurrentToughness -= partDamage;

        if (CurrentToughness <= 0)
        {
            OnToughnessZero?.Invoke(gameObject);
            if (Destroyable) Destroy(gameObject);
        }
    }

    private IEnumerator HideHealthBar()
    {
        yield return new WaitForSeconds(TimeToHideHealthBar);
        _hidingHealthBar = StartCoroutine(FadeHealthBar());
    }

    private IEnumerator FadeHealthBar()
    {
        const float timeToFade = 2;
        var elapsedTime = 0f;
        var canvasGroup = _healthCanvas.GetComponent<CanvasGroup>();
        while (elapsedTime < timeToFade)
        {
            elapsedTime += Time.deltaTime;

            yield return canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0.0f, elapsedTime / timeToFade);
        }
    }

    private int CalculateDamage(int shootingDamage)
            => Mathf.Max(shootingDamage - DamageResistance, NoDamage);
}