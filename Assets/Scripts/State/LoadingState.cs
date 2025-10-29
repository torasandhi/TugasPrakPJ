using Unity.VisualScripting;
using UnityEngine;

public class State_Loading : IState
{
    public void Enter()
    {
        Debug.Log("Entering Loading State.");

    }

    public void Update()
    {
    }

    public void Exit()
    {
        Debug.Log("Exiting Loading State.");

    }
}

