using UnityEngine;

public class State_MainMenu : IState
{
    public void Enter()
    {
        Debug.Log("Entering Main Menu State.");
        // - Show the main menu UI panel
        // - Play main menu music
        // - Ensure the cursor is visible
    }

    public void Update()
    {
        // This state is usually driven by UI button events.
    }

    public void Exit()
    {
        Debug.Log("Exiting Main Menu State.");
        // - Hide the main menu UI panel
        // - Stop main menu music
    }
}

