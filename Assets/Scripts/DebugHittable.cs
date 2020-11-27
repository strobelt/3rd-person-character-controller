using UnityEngine;

public interface IHittable
{
    void OnHit();
}

public class DebugHittable : MonoBehaviour, IHittable
{
    public int TimesHit = 0;

    public void OnHit()
        => Debug.Log($"{name} was hit {TimesHit++} times");
}
