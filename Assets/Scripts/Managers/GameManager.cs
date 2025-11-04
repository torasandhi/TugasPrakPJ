using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public State_Machine GameStateMachine { get; private set; }

    public static event Action<EGameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        GameStateMachine = new State_Machine();

        if (PlayerController.Instance != null)
        {
            OnGameStateChanged += PlayerController.Instance.InputActivate;
        }
        else
        {
            Debug.LogError("GameManager couldn't find PlayerController.Instance on Awake!");
        }
    }

    private void Start()
    {
        // Register all states with the state machine
        GameStateMachine.RegisterState(EGameState.MainMenu, new State_MainMenu());
        GameStateMachine.RegisterState(EGameState.Gameplay, new State_Gameplay());
        GameStateMachine.RegisterState(EGameState.Paused, new State_Pause());
        GameStateMachine.RegisterState(EGameState.Loading, new State_Loading());
        GameStateMachine.RegisterState(EGameState.Authenticate, new State_Authenticate());


        // Start the game to Authenticate
        SceneManager.Instance.LoadLevel("Authenticate", EGameState.Authenticate);
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
