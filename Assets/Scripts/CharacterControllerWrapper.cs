using UnityEngine;

namespace Assets.Scripts
{
    public interface ICharacterControllerWrapper
    {
        bool IsGrounded();
        void Move(Vector3 motion);
    }

    public class CharacterControllerWrapper : ICharacterControllerWrapper
    {
        public CharacterController CharacterController;

        public bool IsGrounded() => CharacterController.isGrounded;

        public void Move(Vector3 motion) => CharacterController.Move(motion);
    }
}