using UnityEngine;

public class State_Pause : IState
{
    public void Enter()
    {
        Debug.Log("Entering Pause State.");
        // - Enable player input
        // - Start spawning enemies
        // - Begin the level timer
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(E_GameState.Gameplay);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Pause State.");
        // - Disable player input
        // - Stop spawning enemies
    }
}

