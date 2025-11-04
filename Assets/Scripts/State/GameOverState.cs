using UnityEngine;

public class State_GameOver : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Main Menu State.");
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Debug.Log("Exiting GameOver State.");
    }
}
