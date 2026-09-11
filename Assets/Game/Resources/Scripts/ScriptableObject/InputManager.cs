using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObjects/InputManager")]
public class InputManager : ScriptableObject, PlayerInput.IGameplayActions {
    //Gameplay
    //Event set to delegate as a way to avoid doing null check using If/Else, to avoid race condition with null check
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction RunEvent = delegate { };
    public event UnityAction RunCancelledEvent = delegate { };
    public event UnityAction CrouchEvent = delegate { };
    public event UnityAction CrouchCancelledEvent = delegate { };
    public event UnityAction PerspectiveShiftEvent = delegate { };
    public event UnityAction ClimbEvent = delegate { };
    public event UnityAction GlideEvent = delegate { };
    public event UnityAction CancelClimbEvent = delegate { };
    public event UnityAction CancelGlideEvent = delegate { };
    public event UnityAction AttackEvent = delegate { };
    public event UnityAction AttackCancelledEvent = delegate { };
    public event UnityAction OpenMainMenuEvent = delegate { };

    //C# wrapper for InputAction schema
    private PlayerInput _playerInput;

    private void OnEnable() {
        if (_playerInput == null) {
            _playerInput = new PlayerInput();

            _playerInput.Gameplay.SetCallbacks(this);
        }
        _playerInput.Enable();
    }

    private void OnDisable() {
        _playerInput.Gameplay.Disable();
    }

    public void OnMove(InputAction.CallbackContext context) {
        Debug.Log(context.ReadValue <Vector2>());
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            JumpEvent.Invoke();
        }
    }

    public void OnRun(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            RunEvent.Invoke();
        }
        else if(context.phase == InputActionPhase.Canceled) {
            RunCancelledEvent.Invoke();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            CrouchEvent.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            CrouchCancelledEvent.Invoke();
        }
    }

    public void OnPerspectiveShift(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            PerspectiveShiftEvent.Invoke();
        }
    }

    public void OnClimb(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            ClimbEvent.Invoke();
        }
    }

    public void OnGlide(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            GlideEvent.Invoke();
        }
    }

    public void OnCancelClimb(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            CancelClimbEvent.Invoke();
        }
    }

    public void OnCancelGlide(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            CancelGlideEvent.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            AttackEvent.Invoke();
        }
        else if(context.phase == InputActionPhase.Canceled) {
            AttackCancelledEvent.Invoke();
        }
    }

    public void OnMainMenu(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Performed) {
            OpenMainMenuEvent.Invoke();
        }
    }

}
