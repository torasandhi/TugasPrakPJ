using UnityEngine;

/// <summary>
/// MODIFIED from your file.
/// Added a simple check for the Space Bar in Update()
/// to provide a way to transition to the Gameplay state.
/// </summary>
public class State_MainMenu : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Main Menu State.");
        UIManager.Instance.SpawnUIByString("ui-mainmenu");
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.Instance.LoadLevel("Gameplay", EGameState.Gameplay);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Main Menu State.");
        UIManager.Instance.DestroyUIByString("ui-mainmenu");
    }   
}
