using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Components
    [SerializeField]
    private Rigidbody _rigidbody;


    //MovementValues
    //Direction
    private Vector2 moveValue;
    private Vector3 moveDirection;

    //Speed
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _runSpeed;
    [SerializeField]
    private float _walkSpeedTransition = 30;
    [SerializeField]
    private float _speed;
    //States
    [SerializeField]
    private bool isMoving;
    [SerializeField]
    private bool isRun;

    //Rotation
    [SerializeField]
    private float _rotationSmoothTime;
    [SerializeField]
    private float _rotationSmoothVelocity;
    [SerializeField]
    private float rotationAngle;
    [SerializeField]
    private float smoothAngle;

    //Jump
    [SerializeField]
    private float _jumpForce;
    //Ground Detector
    [SerializeField]
    private Transform _groundDetector;
    [SerializeField]
    private float _groundDetectorRadius;
    [SerializeField]
    private LayerMask _groundLayer;
    //States
    [SerializeField]
    private bool _isGrounded;

    private void Awake() {
        //Assign components to variables
        _rigidbody = GetComponent<Rigidbody>();
        _groundDetector = transform.Find("GroundDetector");

        if (_walkSpeed <= 0) {
            _walkSpeed = 350;
        }

        if (_rotationSmoothTime <= 0) {
            _rotationSmoothTime = 0.1f;
        }

        if (_runSpeed <= _walkSpeed) {
            _runSpeed = _walkSpeed + 100;
        }

        if (_groundDetectorRadius <= 0) {
            _groundDetectorRadius = 0.2f;
        }

        if (_jumpForce <= 0) {
            _jumpForce = 500;
        }

        _speed = _walkSpeed;
    }


    //Function to move player on WASD direction. Listening to 'Move' InputAction
    public void OnMove(InputAction.CallbackContext context) {
        if (context.performed) {
            isMoving = true;
        }
        moveValue = context.ReadValue<Vector2>();
        Debug.Log(moveValue);
        if (context.canceled) {
            isMoving = false;
        }
    }


    //Function to make player run by shifting player to run mode. Listening to 'Run' InputAction
    public void OnRun(InputAction.CallbackContext context) {
        if (context.performed) {
            isRun = true;
        }
        else if (context.canceled) {
            isRun = false;
        }
    }

    //Function to make player jump. Listening to 'Jump' InputAction
    public void OnJump(InputAction.CallbackContext context) {
        Vector3 jumpDirection = Vector3.up;
        if (context.performed && _isGrounded) {
            _rigidbody.AddForce(jumpDirection * _jumpForce);
            Debug.Log("Player jumped!");
        }
    }

    //Check if character is grounded
    public void CheckIsGrounded() {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _groundDetectorRadius, _groundLayer);
    }

    public void FixedUpdate() {
        //updating grounded check
        CheckIsGrounded();

        //Runing logic
        if (isRun) {
            if (_speed < _runSpeed) { 
                _speed = _speed + _walkSpeedTransition * Time.deltaTime; 
            }
        }
        else {
            if (_speed > _walkSpeed) {
                _speed = _speed - _walkSpeedTransition * Time.deltaTime;
            }
        }
        

        //Movement with rotation
        if (moveValue.magnitude >= 0.1) {
            rotationAngle = Mathf.Atan2(moveValue.x, moveValue.y) * Mathf.Rad2Deg;
            smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            moveDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
        }
        //Case of moveValue = 0 as in stopping (move key no longer pressed)
        else {
            moveDirection = new Vector3(moveValue.x, 0, moveValue.y);
        }

        //Implementation of Addforce in FixedUpdate due to new inputAction don't use continual firing of function
        _rigidbody.AddForce(moveDirection * _speed * Time.deltaTime);
        //Debug.Log(moveDirection);
        //Debug.Log(moveDirection * _walkSpeed * Time.deltaTime);
    }

}
