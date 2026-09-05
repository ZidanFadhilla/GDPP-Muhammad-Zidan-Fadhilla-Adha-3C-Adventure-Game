using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Components
    [SerializeField]
    private Rigidbody _rigidbody;

    //Values
    [SerializeField]
    private float _walkSpeed;
    private float _sprintModifier;
    private Vector3 moveDirection;

    //Toggles
    [SerializeField]
    private bool canMove;

    private void Awake() {
        //Assign components to variables
        _rigidbody = GetComponent<Rigidbody>();

        canMove = true;
        if (_walkSpeed <= 0) {
            _walkSpeed = 350;
        }

        if (_sprintModifier <= 0) {
            _sprintModifier = 1.5f;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        Vector2 moveValue = context.ReadValue<Vector2>();
        moveDirection = new Vector3(moveValue.x, 0, moveValue.y);

        

    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("Player jumped!");
        }
    }

    public void FixedUpdate() {
        if (canMove) {
            _rigidbody.AddForce(moveDirection * _walkSpeed * Time.deltaTime);
        }
    }

}
