using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensions;
using UnityEngine;

namespace Tests
{
    public class BasePlayerMovementTests
    {
        protected Faker Faker;
        protected PlayerMovement PlayerMovement;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Faker = new Faker();

            var player = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
            PlayerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public class PlayerMovementTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldCreate() => PlayerMovement.Should().NotBeNull();
    }

    public class HandleMovementInputTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldSetPlayerMovementVectorToInputVector()
        {
            var inputVector = new Vector2();
            PlayerMovement.HandleMovementInput(inputVector, true);
            PlayerMovement.MovementVector.Should().Be(inputVector);
        }
    }

    [TestFixture]
    public class WhenStartedMoving : BasePlayerMovementTests
    {
        readonly Vector2 _inputVector = new Vector2();
        private float _precision = 0.0001f;

        [OneTimeSetUp]
        public void Configuration()
        {
            PlayerMovement.CameraTarget = new GameObject();
            PlayerMovement.CameraTarget.transform.forward = Faker.Random.Vector3(0, 10);
            PlayerMovement.CameraTarget.transform.localRotation = Faker.Random.Quaternion(0, 10);
        }

        [Test]
        public void ShouldSetTransformForwardToNormalizedCameraHorizontalForward()
        {
            var normalizedCameraHorizontalForward = PlayerMovement.CameraTarget.transform.forward;
            normalizedCameraHorizontalForward.y = 0;
            normalizedCameraHorizontalForward.Normalize();


            PlayerMovement.HandleMovementInput(_inputVector, true);


            PlayerMovement.transform.forward.x.Should()
                .BeApproximately(normalizedCameraHorizontalForward.x, _precision);

            PlayerMovement.transform.forward.y.Should().Be(0);

            PlayerMovement.transform.forward.z.Should()
                .BeApproximately(normalizedCameraHorizontalForward.z, _precision);

            PlayerMovement.transform.forward.Should().BeEquivalentTo(normalizedCameraHorizontalForward);
        }

        [Test]
        public void ShouldResetCameraTargetRotationButKeepHorizontalIncline()
        {
            var targetHorizontalRotation = PlayerMovement.CameraTarget.transform.localRotation.eulerAngles.x;
            var expectedRotation = Quaternion.Euler(targetHorizontalRotation, 0, 0);

            PlayerMovement.HandleMovementInput(_inputVector, true);

            var targetLocalRotation = PlayerMovement.CameraTarget.transform.localRotation;

            targetLocalRotation.Should().BeEquivalentTo(expectedRotation);
        }

        [Test]
        public void ShouldNotHaveRotationAroundZAxis()
        {
            PlayerMovement.HandleMovementInput(_inputVector, true);

            var targetLocalRotation = PlayerMovement.CameraTarget.transform.localRotation;

            targetLocalRotation.eulerAngles.z.Should().Be(0);
        }
    }

    [TestFixture]
    public class HandleLookInputTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldSetLookVectorToInputVector()
        {
            var inputVector = Faker.Random.Vector2(0, 10);
            PlayerMovement.HandleLookInput(inputVector);
            PlayerMovement.LookVector.Should().Be(inputVector);
        }
    }

    [TestFixture]
    public class HandleJumpInputTests : BasePlayerMovementTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSetBaseIsJumpingToJumpInput(bool jumpInput)
        {
            PlayerMovement.HandleJumpInput(jumpInput);
            PlayerMovement.IsJumping.Should().Be(jumpInput);
        }

        [Test]
        public void ShouldZeroJumpTimerWhenJumpInputIsFalse()
        {
            PlayerMovement.JumpTimer = Faker.Random.Float();
            PlayerMovement.HandleJumpInput(false);
            PlayerMovement.JumpTimer.Should().Be(0);
        }

        [Test]
        public void ShouldNotChangeJumpTimerWhenJumpInputIsTrue()
        {
            var jumpTimer = Faker.Random.Float();
            PlayerMovement.JumpTimer = jumpTimer;
            PlayerMovement.HandleJumpInput(true);
            PlayerMovement.JumpTimer.Should().Be(jumpTimer);
        }
    }
}