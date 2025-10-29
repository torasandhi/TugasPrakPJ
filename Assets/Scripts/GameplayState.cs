using UnityEngine;

public class State_Gameplay : IState
{
    public void Enter()
    {
        Debug.Log("Entering Gameplay State.");
        // - Enable player input
        // - Start spawning enemies
        // - Begin the level timer
        // - Show the gameplay HUD
    }

    public void Update()
    {
        // - Check for win/loss conditions
        // - Listen for the pause button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(E_GameState.Paused);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Gameplay State.");
        // - Disable player input
        // - Stop spawning enemies
        // - Hide the gameplay HUD
    }
}

