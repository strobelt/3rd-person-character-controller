using Assets.Scripts;
using Bogus;
using FluentAssertions;
using NSubstitute;
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
            PlayerMovement.CharacterControllerWrapper = Substitute.For<ICharacterControllerWrapper>();
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

    [TestFixture]
    public class HandlePlayerMovementTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldCalculateHorizontalVelocity()
        {
            PlayerMovement.PlayerVelocity = Vector3.zero;
            var movementVector = Faker.Random.Vector2();
            PlayerMovement.MovementVector = movementVector;
            var expectedVelocity =
                PlayerMovement.transform.right * movementVector.x +
                PlayerMovement.transform.forward * movementVector.y;
            expectedVelocity *= PlayerMovement.MovementSpeed;

            PlayerMovement.HandlePlayerMovement();

            var playerHorizontalVelocity = PlayerMovement.PlayerVelocity;
            playerHorizontalVelocity.y = 0;
            playerHorizontalVelocity.Should().BeEquivalentTo(expectedVelocity);
        }

        [Test]
        public void ShouldSetIsJumpingToFalseWhenIsJumpingAndJumpTimerIsLessThanOrEqualTo0()
        {
            PlayerMovement.IsJumping = true;
            PlayerMovement.JumpTimer = Faker.Random.Float(-10, 0);
            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.IsJumping.Should().BeFalse();
        }

        [Test]
        public void ShouldHavePositiveVerticalVelocityIfIsJumpingAndJumpTimerIsGreaterThan0()
        {
            PlayerMovement.PlayerVelocity.y = 0;
            PlayerMovement.IsJumping = true;
            PlayerMovement.JumpTimer = Faker.Random.Float(1);

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.PlayerVelocity.y.Should().BeGreaterThan(0);
        }

        [Test]
        public void ShouldDecreaseJumpTimerIfIsJumpingAndJumpTimerIsGreaterThan0()
        {
            PlayerMovement.JumpTimer = Faker.Random.Float(1);
            PlayerMovement.IsJumping = true;
            var initialJumpTimer = PlayerMovement.JumpTimer;

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.JumpTimer.Should().BeLessThan(initialJumpTimer);
        }

        [Test]
        public void ShouldApplyGravityToVerticalVelocity()
        {
            PlayerMovement.PlayerVelocity.y = Faker.Random.Float(10);
            PlayerMovement.IsJumping = false;
            var initialVerticalVelocity = PlayerMovement.PlayerVelocity.y;

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.PlayerVelocity.y.Should()
                .Be(initialVerticalVelocity + PlayerMovement.GravityAcceleration * Time.deltaTime);
        }

        [Test]
        public void ShouldRotatePlayerIfMoving()
        {
            PlayerMovement.MovementVector = Faker.Random.Vector2(0, 10);
            var lookVector = Faker.Random.Vector2(0, 10);
            PlayerMovement.LookVector = lookVector;
            var initialPlayerRotation = PlayerMovement.transform.rotation.eulerAngles;
            var expectedRotation =
                Quaternion.Euler(
                    initialPlayerRotation +
                    Vector3.up * lookVector.x * PlayerMovement.RotationDelta
                );

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.transform.rotation.Should().BeEquivalentTo(expectedRotation);
        }

        [Test]
        public void ShouldTiltCameraIfLookingWhileMoving()
        {
            PlayerMovement.MovementVector = Faker.Random.Vector3(0, 10);
            var lookVector = Faker.Random.Vector2(0, 10);
            PlayerMovement.LookVector = lookVector;
            var initialCameraRotation = PlayerMovement.CameraTarget.transform.rotation.eulerAngles;
            var expectedRotation =
                Quaternion.Euler(
                    initialCameraRotation +
                    Vector3.right * lookVector.y * PlayerMovement.RotationDelta
                );

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.CameraTarget.transform.rotation.eulerAngles.x.Should()
                .BeApproximately(expectedRotation.eulerAngles.x, 0.0001f);
        }

        [Test]
        public void ShouldRotateCameraIfNotMoving()
        {
            PlayerMovement.CameraTarget.transform.rotation = Quaternion.identity;
            PlayerMovement.MovementVector = Vector3.zero;
            PlayerMovement.GravityAcceleration = 0;
            PlayerMovement.LookVector = Faker.Random.Vector2(0, 10);
            var initialCameraRotation = PlayerMovement.CameraTarget.transform.rotation;

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.CameraTarget.transform.rotation
                .Should()
                .NotBeEquivalentTo(initialCameraRotation);
        }

        [Test]
        public void ShouldRotateCameraByLookVectorIfNotMoving()
        {
            PlayerMovement.CameraTarget.transform.rotation = Quaternion.identity;
            PlayerMovement.MovementVector = Vector3.zero;
            PlayerMovement.GravityAcceleration = 0;
            var lookVector = Faker.Random.Vector2(0, 10);
            PlayerMovement.LookVector = lookVector;
            var initialCameraRotation = PlayerMovement.CameraTarget.transform.rotation.eulerAngles;
            var expectedRotation =
                Quaternion.Euler(
                    initialCameraRotation +
                    Vector3.up * lookVector.x * PlayerMovement.RotationDelta +
                    Vector3.right * lookVector.y * PlayerMovement.RotationDelta
                );

            PlayerMovement.HandlePlayerMovement();

            PlayerMovement.CameraTarget.transform.rotation
                .Should()
                .BeEquivalentTo(expectedRotation);
        }
    }
}