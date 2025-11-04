using UnityEngine;

public class State_Gameplay : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Gameplay State.");
        // - Enable player input
        // - Start spawning enemies
        // - Begin the level timer
        // - Show the gameplay HUD
    }

    public void OnUpdate()
    {
        // - Check for win/loss conditions
        // - Listen for the pause button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Paused);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Gameplay State.");
        // - Disable player input
        // - Stop spawning enemies
        // - Hide the gameplay HUD
    }
}

