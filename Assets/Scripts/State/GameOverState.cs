using UnityEngine;

public class State_GameOver : IState
{
    public void Enter()
    {
        Debug.Log("Entering Main Menu State.");
    }

    public void Update()
    {
    }

    public void Exit()
    {
        Debug.Log("Exiting GameOver State.");
    }
}
