using Bogus;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensions;
using UnityEngine;

namespace Tests
{
    public class BasePlayerMovementTests
    {
        protected PlayerMovement playerMovement;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var player = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public class PlayerMovementTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldCreate() => playerMovement.Should().NotBeNull();
    }

    public class HandleMovementInputTests : BasePlayerMovementTests
    {
        [Test]
        public void ShouldSetPlayerMovementVectorToInputVector()
        {
            var inputVector = new Vector2();
            playerMovement.HandleMovementInput(inputVector, true);
            playerMovement._movementVector.Should().Be(inputVector);
        }
    }

    [TestFixture]
    public class WhenStartedMoving : BasePlayerMovementTests
    {
        readonly Vector2 _inputVector = new Vector2();
        private Faker _faker;
        private float _precision = 0.0001f;

        [OneTimeSetUp]
        public void Configuration()
        {
            _faker = new Faker();

            playerMovement.CameraTarget = new GameObject();
            playerMovement.CameraTarget.transform.forward = _faker.Random.Vector3(0, 10);
            playerMovement.CameraTarget.transform.localRotation = _faker.Random.Quaternion(0, 10);
        }

        [Test]
        public void ShouldSetTransformForwardToNormalizedCameraHorizontalForward()
        {
            var normalizedCameraHorizontalForward = playerMovement.CameraTarget.transform.forward;
            normalizedCameraHorizontalForward.y = 0;
            normalizedCameraHorizontalForward.Normalize();


            playerMovement.HandleMovementInput(_inputVector, true);


            playerMovement.transform.forward.x.Should()
                .BeApproximately(normalizedCameraHorizontalForward.x, _precision);

            playerMovement.transform.forward.y.Should().Be(0);

            playerMovement.transform.forward.z.Should()
                .BeApproximately(normalizedCameraHorizontalForward.z, _precision);

            playerMovement.transform.forward.Should().BeEquivalentTo(normalizedCameraHorizontalForward);
        }

        [Test]
        public void ShouldResetCameraTargetRotationButKeepHorizontalIncline()
        {
            var targetHorizontalRotation = playerMovement.CameraTarget.transform.localRotation.eulerAngles.x;
            var expectedRotation = Quaternion.Euler(targetHorizontalRotation, 0, 0);

            playerMovement.HandleMovementInput(_inputVector, true);

            var targetLocalRotation = playerMovement.CameraTarget.transform.localRotation;

            targetLocalRotation.Should().BeEquivalentTo(expectedRotation);
        }

        [Test]
        public void ShouldNotHaveRotationAroundZAxis()
        {
            playerMovement.HandleMovementInput(_inputVector, true);

            var targetLocalRotation = playerMovement.CameraTarget.transform.localRotation;

            targetLocalRotation.eulerAngles.z.Should().Be(0);
        }
    }
}