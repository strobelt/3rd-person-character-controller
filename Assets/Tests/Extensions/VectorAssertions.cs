using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using UnityEngine;

namespace Tests.Extensions
{
    public class Vector3Assertions : ReferenceTypeAssertions<Vector3, Vector3Assertions>
    {
        public const float DefaultPrecision = 0.0001f;

        protected override string Identifier => "vector3";

        public Vector3Assertions(Vector3 instance)
            => Subject = instance;

        public AndConstraint<Vector3Assertions> BeEquivalentTo(Vector3 other,
            float precision = DefaultPrecision, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject == other)
                .FailWith(
                    $"Vector3 {Subject:F8} and {other:F8} differ by {Subject - other}, more than {precision}");

            return new AndConstraint<Vector3Assertions>(this);
        }
    }
}