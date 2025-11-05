using UnityEngine;

public class State_GameOver : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Main Menu State.");
        UIManager.Instance.DestroyUIByString("ui-gameplay");
        UIManager.Instance.SpawnUIByString("ui-gameover");
        GameOver.Instance.ShowScore();
        GameManager.Instance.PauseGame();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Debug.Log("Exiting GameOver State.");
        UIManager.Instance.DestroyUIByString("ui-gameover");
        GameManager.Instance.ResumeGame();
    }
}
