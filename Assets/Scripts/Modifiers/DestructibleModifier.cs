using System;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleModifier : MonoBehaviour, IHittable
{
    public int Toughness = 100;
    public int DamageResistance = 3;
    public bool DecreaseOnHit = true;
    public UnityEvent<GameObject, int> OnHit;
    public UnityEvent<GameObject> OnToughnessZero;

    private const int NoDamage = 0;

    public void Hit(GameObject hitter, int shootingDamage)
    {
        var partDamage = CalculateDamage(shootingDamage);

        OnHit?.Invoke(gameObject, partDamage);

        if (DecreaseOnHit)
            Toughness -= partDamage;

        if (Toughness == 0)
        {
            OnToughnessZero?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    private int CalculateDamage(int shootingDamage)
        => Math.Max(shootingDamage - DamageResistance, NoDamage);
}
