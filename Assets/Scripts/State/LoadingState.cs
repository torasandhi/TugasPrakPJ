using Unity.VisualScripting;
using UnityEngine;

public class State_Loading : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Loading State.");

    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Debug.Log("Exiting Loading State.");

    }
}

