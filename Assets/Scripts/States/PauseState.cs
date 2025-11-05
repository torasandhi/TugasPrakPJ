using UnityEngine;

public class State_Pause : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Pause State.");
        UIManager.Instance.SpawnUIByString("ui-pause");
        GameManager.Instance.PauseGame();
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Gameplay);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Pause State.");
        UIManager.Instance.DestroyUIByString("ui-pause");
        GameManager.Instance.ResumeGame();
    }
}