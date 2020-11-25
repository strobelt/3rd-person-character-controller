using Cinemachine;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace Tests.EditMode
{
    public class BaseSampleSceneTests
    {
        protected Scene SampleScene;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            EditorSceneManager.OpenScene(@"Assets\Scenes\SampleScene.unity", OpenSceneMode.Single);
            SampleScene = SceneManager.GetActiveScene();
        }
    }

    [TestFixture]
    public class SampleSceneTests : BaseSampleSceneTests
    {
        [Test]
        public void ShouldLoadTheCorrectScene()
            => SampleScene.name.Should().Be("SampleScene");

        [Test]
        public void ShouldHaveAPlayer()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.tag == "Player");

        [Test]
        public void ShouldHaveAGameObjectControllableByThePlayer()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.GetComponent<CharacterController>() != null);

        [Test]
        public void ShouldHaveAGameObjectMovableByThePlayer()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.GetComponent<PlayerMovement>() != null);

        [Test]
        public void ShouldHaveAMainCamera()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.tag == "MainCamera");

        [Test]
        public void ShouldHaveACineMachineFollowCam()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.GetComponent<CinemachineVirtualCamera>() != null);

        [Test]
        public void CineMachineFollowCamShouldFollowPlayerCameraTarget()
        {
            var sceneObjects = SampleScene.GetRootGameObjects();
            var playerObject = sceneObjects.FirstOrDefault(go => go.tag == "Player");
            var cameraTarget = playerObject.GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.gameObject.name == "CameraTarget");

            var followCam = sceneObjects.FirstOrDefault(go => go.GetComponent<CinemachineVirtualCamera>() != null);

            followCam.GetComponent<CinemachineVirtualCamera>().Follow.gameObject.Should().Be(cameraTarget.gameObject);
        }

        [Test]
        public void ShouldHaveAnInputEventSystem()
            => SampleScene.GetRootGameObjects().Should().Contain(go => go.GetComponent<InputSystemUIInputModule>() != null);
    }
}