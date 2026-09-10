using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour {
    //Input manager
    [SerializeField]
    private InputManager _inputManager;

    //Camera
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    private CameraControl _cameraControl;

    //Components
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private PlayerInput _playerInput;


    //Stance
    private PlayerStance _playerStance;

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
    private bool isRunning;

    //Movement Elevation
    [SerializeField]
    private Vector3 _upperStepOffset;
    [SerializeField]
    private float _stepCheckerDistance;
    [SerializeField]
    private float _stepForce;

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


    //Climbing
    [SerializeField]
    private Transform _climbDetector;
    [SerializeField]
    private float _climbCheckDistance;
    [SerializeField]
    private LayerMask _climbableLayer;
    [SerializeField]
    private Vector3 _climbOffset;
    [SerializeField]
    private float _climbSpeed;

    
    private void Awake() {
        //Assign MainCamera dan CameraControl
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        _cameraControl = GameObject.FindGameObjectWithTag("CameraControl").GetComponent<CameraControl>();

        //Assign components to variables
        _rigidbody = GetComponent<Rigidbody>();
        _groundDetector = transform.Find("GroundDetector");
        _climbDetector = transform.Find("ClimbDetector");

        //Initial Stance
        _playerStance = PlayerStance.Stand;


        //Default value

        _walkSpeed = _walkSpeed > 0 ? _walkSpeed : 350;
        _rotationSmoothTime = _rotationSmoothTime > 0 ? _rotationSmoothTime : 0.1f;
        _upperStepOffset = _upperStepOffset == Vector3.zero ? new Vector3(0, 0.3f, 0.3f) : _upperStepOffset;
        _stepCheckerDistance = _stepCheckerDistance > 0 ? _stepCheckerDistance : 0.1f;
        _stepForce = _stepForce > 0 ? _stepForce : 400f;
        _runSpeed = _runSpeed > _walkSpeed ? _runSpeed : _walkSpeed + 100;
        _groundDetectorRadius = _groundDetectorRadius > 0 ? _groundDetectorRadius : 0.2f;
        _jumpForce = _jumpForce > 0 ? _jumpForce : 500;
        _climbCheckDistance = _climbCheckDistance > 0 ? _climbCheckDistance : 1;
        _climbOffset = _climbOffset == Vector3.zero ? new Vector3(0, 1, 0.16f) : _climbOffset;
        _climbSpeed = _climbSpeed > 0 ? _climbSpeed : 20;

        //
        _speed = _walkSpeed;


        //Hides Cursor
        HideAndLockCursor();
    }

    private void OnEnable() {   
        _inputManager = AssetDatabase.LoadAssetAtPath<InputManager>("Assets/Game/Scripts/ScriptableObject/InputManager.asset");
        _inputManager.MoveEvent += OnMove;
        _inputManager.JumpEvent += OnJump;
        _inputManager.RunEvent += OnRun;
        _inputManager.RunCancelledEvent += OnStopRun;   
        _inputManager.ClimbEvent += OnClimb;
        _inputManager.CancelClimbGlideEvent += OnCancelClimb;
    }

    private void OnDisable() {
        _inputManager.MoveEvent -= OnMove;
        _inputManager.JumpEvent -= OnJump;
        _inputManager.RunEvent -= OnRun;
        _inputManager.RunCancelledEvent -= OnStopRun;
        _inputManager.ClimbEvent -= OnClimb;
        _inputManager.CancelClimbGlideEvent -= OnCancelClimb;
    }

    //Function to hides cursor
    private void HideAndLockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Function to check for movement vector2 value. Listening to 'Move' InputAction
    public void OnMove(Vector2 movement) {
        moveValue = movement;
        Debug.Log(moveValue);
    }


    //Function to process Vector2 value read through the OnMove function;
    private void ProcessMove() {
        if (_playerStance == PlayerStance.Stand) {
            switch(_cameraControl.cameraState) {
                case CameraState.ThirdPerson:
                    if (moveValue.magnitude >= 0.1) {
                        //Old rotationAngle without third person camera
                        //rotationAngle = Mathf.Atan2(moveValue.x, moveValue.y) * Mathf.Rad2Deg;

                        //New rotationAngle with third person camera
                        rotationAngle = Mathf.Atan2(moveValue.x, moveValue.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                        smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                        moveDirection = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                    }
                    //Case of moveValue = 0 as in stopping (move key no longer pressed)
                    else {
                        moveDirection = new Vector3(moveValue.x, 0, moveValue.y);
                    }
                    break;
                case CameraState.FirstPerson:
                    transform.rotation = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f);
                    Vector3 verticalDirection = moveValue.y * transform.forward;
                    Vector3 horizontalDirection = moveValue.x * transform.right;
                    moveDirection = verticalDirection + horizontalDirection;
                    break;
                default:
                    Debug.Log("Player has no camera mode");
                    break;
            }
            

            //Implementation of Addforce in FixedUpdate due to new inputAction don't use continual firing of function
            _rigidbody.AddForce(moveDirection * _speed * Time.deltaTime);
            CheckStep();
        }
        else if (_playerStance == PlayerStance.Climb) {
            Vector3 horizontal = moveValue.x * transform.right;
            Vector3 vertical = moveValue.y * transform.up;
            moveDirection = horizontal + vertical;
            if (moveDirection != Vector3.zero) {
                _rigidbody.AddForce(moveDirection * Time.deltaTime * _climbSpeed);
            }
            else {
                _rigidbody.linearVelocity = Vector3.zero;
            }
            
            Debug.Log(moveDirection * Time.deltaTime * _climbSpeed);
        }

        //Movement with rotation

        //Debug.Log(moveDirection);
        //Debug.Log(moveDirection * _walkSpeed * Time.deltaTime);
    }

    //Function to check if a surface with different elevation can be stepped into by player character
    private void CheckStep() {
        bool isHitLowerStep = Physics.Raycast(_groundDetector.position, transform.forward, _stepCheckerDistance);
        bool isHitUpperStep = Physics.Raycast(_groundDetector.position + _upperStepOffset, transform.forward, _stepCheckerDistance);

        if (isHitLowerStep && !isHitUpperStep) {
            _rigidbody.AddForce(0, _stepForce * Time.deltaTime, 0);
        }
    }


    //Function to make player run by shifting player to run mode. Listening to 'Run' InputAction
    public void OnRun() {
        isRunning = true;
    }

    public void OnStopRun() {
        isRunning = false;
    }

    //Function to make player jump. Listening to 'Jump' InputAction
    public void OnJump() {
        Vector3 jumpDirection = Vector3.up;
        if (_isGrounded) {
            _rigidbody.AddForce(jumpDirection * _jumpForce);
            Debug.Log("Player jumped!");
        }
    }

    //Check if character is grounded
    private void CheckIsGrounded() {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _groundDetectorRadius, _groundLayer);
    }

    //Function to enter the climbing stance
    public void OnClimb() {
        bool isInFrontOfClimbingWall = Physics.Raycast(_climbDetector.position, transform.forward, out RaycastHit hit, _climbCheckDistance, _climbableLayer);

        bool isNotClimbing = _playerStance != PlayerStance.Climb;

        if (isInFrontOfClimbingWall && isNotClimbing) {
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            _cameraControl.SetFPSClampedCamera(true, transform.rotation.eulerAngles);
            transform.position = hit.point - offset;
            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;
        }
    }


    public void OnCancelClimb() {
        if (_playerStance == PlayerStance.Climb) {
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * 1f;
            _cameraControl.SetFPSClampedCamera(false, transform.rotation.eulerAngles);
        }
    }


    private void FixedUpdate() {
        //updating grounded check
        CheckIsGrounded();

        //Runing logic
        if (isRunning) {
            if (_speed < _runSpeed) {
                _speed = _speed + _walkSpeedTransition * Time.deltaTime;
            }
        }
        else {
            if (_speed > _walkSpeed) {
                _speed = _speed - _walkSpeedTransition * Time.deltaTime;
            }
        }


        ProcessMove();
    }

}
