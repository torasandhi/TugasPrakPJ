using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // Add references to your UI panels (e.g., Main Menu, HUD, Pause Menu)
    // public GameObject mainMenuPanel;
    // public GameObject gameplayHudPanel;

    protected override void Awake()
    {
        base.Awake();
        // Subscribe to game state changes to automatically show/hide UI
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(E_GameState newState)
    {
        // Hide all panels first
        // mainMenuPanel.SetActive(false);
        // gameplayHudPanel.SetActive(false);

        // Show the correct panel based on the new state
        // if (newState == E_GameState.MainMenu) mainMenuPanel.SetActive(true);
        // if (newState == E_GameState.Gameplay) gameplayHudPanel.SetActive(true);
    }
}
