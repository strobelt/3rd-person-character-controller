using UnityEngine;
using UnityEngine.Events;

public class DestructibleModifier : MonoBehaviour, IHittable
{
    public int Toughness = 10;
    public bool DecreaseOnHit = true;
    public UnityEvent<GameObject> OnHit;
    public UnityEvent<GameObject> OnToughnessZero;

    public void Hit(GameObject hitter)
    {
        OnHit?.Invoke(gameObject);

        if (DecreaseOnHit)
            Toughness--;

        if (Toughness == 0)
        {
            OnToughnessZero?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
}
