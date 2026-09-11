using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour {

    private InputManager _inputManager;

    private void OnEnable() {
        _inputManager = AssetDatabase.LoadAssetAtPath<InputManager>("Assets/Game/Scripts/ScriptableObject/InputManager.asset");
        _inputManager.OpenMainMenuEvent += BackToMainMenu;
    }

    private void BackToMainMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}