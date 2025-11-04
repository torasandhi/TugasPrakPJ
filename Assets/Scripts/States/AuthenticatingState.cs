using UnityEngine;

public class State_Authenticate : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Main Menu State.");
        UIManager.Instance.SpawnUIByString("ui-authentication");
    }

    public void OnUpdate()
    { 
    }

    public void OnExit()
    {
        Debug.Log("Exiting Main Menu State.");
        UIManager.Instance.DestroyUIByString("ui-authentication");
    }
}
