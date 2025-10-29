using UnityEngine;

public class State_Pause : IState
{
    public void Enter()
    {
        Debug.Log("Entering Pause State.");
        UIManager.Instance.SpawnUIByString("ui-pause");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Gameplay);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Pause State.");
        UIManager.Instance.DestroyUIByString("ui-pause");
    }
}