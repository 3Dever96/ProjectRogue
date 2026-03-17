using UnityEngine.InputSystem;
using UnityEngine;

namespace ProjectRogue.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        public Vector2 Move { get { return move; } }
        public bool Jump { get { return jump; } }

        PlayerInput input;

        Vector2 move;
        bool jump;

        void OnEnable()
        {
            if (input == null)
            {
                input = GetComponent<PlayerInput>();
            }

            input.onActionTriggered += OnAction;
        }

        void OnDisable()
        {
            input.onActionTriggered -= OnAction;
        }

        void OnAction(InputAction.CallbackContext context)
        {
            switch (context.action.name) 
            {
                case "Move":
                    move = context.ReadValue<Vector2>();
                    break;
                case "Jump":
                    SetInputBool(ref jump, context);
                    break;
            }
        }

        void SetInputBool(ref bool value, InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                value = true;
            }

            if (context.canceled)
            {
                value = false;
            }
        }
    }
}
