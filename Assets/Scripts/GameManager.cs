using UnityEngine;
using System;

/// <summary>
/// MODIFIED from your file.
/// This now correctly subscribes to the state change event
/// and registers all your states in the Start() method.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public State_Machine GameStateMachine { get; private set; }

    public static event Action<EGameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        GameStateMachine = new State_Machine();

        // --- MODIFICATION ---
        // Subscribe to the event to automatically switch input maps.
        // We check if PlayerController exists first.
        if (PlayerController.Instance != null)
        {
            OnGameStateChanged += PlayerController.Instance.InputActivate;
        }
        else
        {
            Debug.LogError("GameManager couldn't find PlayerController.Instance on Awake!");
        }
        // ---------------------
    }

    private void Start()
    {
        // --- MODIFICATION ---
        // Register all your states with the state machine
        GameStateMachine.RegisterState(EGameState.MainMenu, new State_MainMenu());
        GameStateMachine.RegisterState(EGameState.Gameplay, new State_Gameplay());
        GameStateMachine.RegisterState(EGameState.Paused, new State_Pause());
        GameStateMachine.RegisterState(EGameState.Loading, new State_Loading());
        // (Add GameOver and Loading states here when you create their classes)
        // -----------------------

        // Start the game in the Main Menu
        SceneManager.Instance.LoadLevel("MainMenu", EGameState.MainMenu);
    }

    private void Update()
    {
        GameStateMachine.Update();
    }

    public void TriggerGameStateChange(EGameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }
}
