using UnityEngine;

public class State_GameOver : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Main Menu State.");
        UIManager.Instance.SpawnUIByString("ui-gameover");
        GameOver.Instance.ShowScore();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Debug.Log("Exiting GameOver State.");
        UIManager.Instance.DestroyUIByString("ui-gameover");
    }
}
