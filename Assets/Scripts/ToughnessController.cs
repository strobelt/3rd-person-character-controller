using UnityEngine;
using UnityEngine.Events;

public class ToughnessController : MonoBehaviour, IHittable
{
    public int Toughness = 10;
    public bool DecreaseOnHit = true;
    public UnityEvent OnToughnessZero;

    public void OnHit()
    {
        if (DecreaseOnHit)
            Toughness--;

        if (Toughness == 0)
            OnToughnessZero?.Invoke();
    }
}
