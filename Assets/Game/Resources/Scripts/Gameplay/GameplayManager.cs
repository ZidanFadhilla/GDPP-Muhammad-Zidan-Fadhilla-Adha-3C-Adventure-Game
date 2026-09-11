using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour {

    [SerializeField]
    private InputManager _inputManager;

    private void OnEnable() {
        _inputManager.OpenMainMenuEvent += BackToMainMenu;
    }

    private void BackToMainMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}