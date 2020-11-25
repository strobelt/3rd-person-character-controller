using UnityEngine;

namespace Tests.Extensions
{
    public static class QuaternionExtensions
    {
        public static QuaternionAssertions Should(this Quaternion instance) => new QuaternionAssertions(instance);

        public static Vector3Assertions Should(this Vector3 instance) => new Vector3Assertions(instance);
    }
}
