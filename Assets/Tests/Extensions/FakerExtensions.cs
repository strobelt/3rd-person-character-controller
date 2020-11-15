using Bogus;
using UnityEngine;

namespace Tests.Extensions
{
    public static class FakerExtensions
    {
        public static Vector3 Vector3(this Randomizer randomizer, float min = 0.0f, float max = 1.0f)
        {
            return new Vector3(
                randomizer.Float(min, max),
                randomizer.Float(min, max),
                randomizer.Float(min, max)
            );
        }

        public static Quaternion Quaternion(this Randomizer randomizer, float minEulerAngle = 0.0f,
            float maxEulerAngle = 1.0f)
        {
            return UnityEngine.Quaternion.Euler(
                randomizer.Float(minEulerAngle, maxEulerAngle),
                randomizer.Float(minEulerAngle, maxEulerAngle),
                randomizer.Float(minEulerAngle, maxEulerAngle)
            );
        }
    }
}