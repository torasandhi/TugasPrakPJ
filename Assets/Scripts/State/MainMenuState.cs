using UnityEngine;

/// <summary>
/// MODIFIED from your file.
/// Added a simple check for the Space Bar in Update()
/// to provide a way to transition to the Gameplay state.
/// </summary>
public class State_MainMenu : IState
{
    public void Enter()
    {
        Debug.Log("Entering Main Menu State.");
        UIManager.Instance.SpawnUIByString("ui-mainmenu");
    }

    public void Update()
    {
        // This state is usually driven by UI button events.

        // Simple way to start the game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // You could use your SceneManager here to load the level first!
            // For example:
            // SceneManager.Instance.LoadLevel("YourGameplaySceneName", EGameState.Gameplay);

            // For now, we'll just change the state.
            SceneManager.Instance.LoadLevel("Gameplay", EGameState.Gameplay);

        }
        // ----------------
    }

    public void Exit()
    {
        Debug.Log("Exiting Main Menu State.");
        UIManager.Instance.DestroyUIByString("ui-mainmenu");
    }   
}
