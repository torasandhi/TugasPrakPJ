using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    public State_Machine GameStateMachine { get; private set; }

    public static event Action<E_GameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        GameStateMachine = new State_Machine();
    }

    private void Start()
    {
        GameStateMachine.ChangeState(E_GameState.MainMenu);
    }

    private void Update()
    {
        GameStateMachine.Update();
    }

    public void TriggerGameStateChange(E_GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }
}