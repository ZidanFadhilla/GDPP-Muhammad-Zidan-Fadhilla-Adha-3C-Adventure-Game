using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Input Manager
    [SerializeField]
    private InputManager _inputManager;


    public CameraState cameraState;
    [SerializeField]
    private CinemachineCamera _fpsCamera;
    [SerializeField]
    private CinemachineCamera _tpsCamera;

    private void Awake() {
        _fpsCamera = GameObject.FindGameObjectWithTag("FirstPersonCamera").GetComponent<CinemachineCamera>();
        _tpsCamera = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineCamera>();
    }

    private void Start() {
        cameraState = CameraState.ThirdPerson;
        _tpsCamera.gameObject.SetActive(true);
        _fpsCamera.gameObject.SetActive(false);
    }

    private void OnEnable() {
        _inputManager.PerspectiveShiftEvent += SwitchCamera;
    }

    private void OnDisable() {
        _inputManager.PerspectiveShiftEvent -= SwitchCamera;
    }

    public void SwitchCamera() {
        if(cameraState == CameraState.FirstPerson) {
            Debug.Log("Switching camera to Third Person!");
            cameraState = CameraState.ThirdPerson;
            _tpsCamera.gameObject.SetActive(true);
            _fpsCamera.gameObject.SetActive(false);
        }
        else if (cameraState == CameraState.ThirdPerson) {
            Debug.Log("Switching camera to First Person!");
            cameraState = CameraState.FirstPerson;
            _fpsCamera.gameObject.SetActive(true);
            _tpsCamera.gameObject.SetActive(false);
        }
    }

    public void SetFPSClampedCamera(bool isClamped, Vector3 playerRotation) {
        CinemachinePanTilt panTilt = _fpsCamera.GetComponent<CinemachinePanTilt>();
        if (isClamped) {
            panTilt.PanAxis.Wrap = false;
            panTilt.PanAxis.Range = new Vector2(playerRotation.y - 45, playerRotation.y + 45);
        }
        else {
            panTilt.PanAxis.Range = new Vector2(-180, 180);
            panTilt.PanAxis.Wrap = true;
        }
    }
}
