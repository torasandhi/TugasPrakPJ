using UnityEngine;

public class State_Gameplay : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Gameplay State.");
    }

    public void OnUpdate()
    {
        // - Check for win/loss conditions
        // - Listen for the pause button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Paused);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.GameOver);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Gameplay State.");
    }
}

