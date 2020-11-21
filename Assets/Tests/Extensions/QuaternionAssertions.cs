using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using UnityEngine;

namespace Tests.Extensions
{
    public class QuaternionAssertions : ReferenceTypeAssertions<Quaternion, QuaternionAssertions>
    {
        public const float DefaultPrecision = 0.0001f;

        protected override string Identifier => "quaternion";

        public QuaternionAssertions(Quaternion instance)
            => Subject = instance;

        public AndConstraint<QuaternionAssertions> BeEquivalentTo(Quaternion other, float precision = DefaultPrecision, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(AreQuaternionsApproximatelyEquivalent(Subject, other, precision))
                .FailWith($"Quaternions {Subject.eulerAngles:F8} and {other.eulerAngles:F8} differ by {AbsoluteDotProduct(Subject, other)}, more than {precision}");

            return new AndConstraint<QuaternionAssertions>(this);
        }

        public AndConstraint<QuaternionAssertions> NotBeEquivalentTo(Quaternion other, float precision = DefaultPrecision, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!AreQuaternionsApproximatelyEquivalent(Subject, other, precision))
                .FailWith($"Quaternions {Subject.eulerAngles:F8} and {other.eulerAngles:F8} differ by {AbsoluteDotProduct(Subject, other)}, less than {precision}");

            return new AndConstraint<QuaternionAssertions>(this);
        }

        private bool AreQuaternionsApproximatelyEquivalent(Quaternion first, Quaternion second, float precision)
            => AbsoluteDotProduct(first, second) < precision;

        private float AbsoluteDotProduct(Quaternion first, Quaternion second)
            => 1 - Mathf.Abs(Quaternion.Dot(first, second));
    }
}