using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This was the missing State_Machine class that your GameManager relies on.
/// It handles registering, changing, and updating states.
/// </summary>
public class State_Machine
{
    private Dictionary<EGameState, IState> _states = new Dictionary<EGameState, IState>();
    private IState _currentState;

    public void RegisterState(EGameState stateEnum, IState state)
    {
        _states[stateEnum] = state;
    }

    public void ChangeState(EGameState newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        if (_states.TryGetValue(newState, out IState nextState))
        {
            _currentState = nextState;
            _currentState.OnEnter();

            // This is the crucial line to tell GameManager the state has changed
            // which will then trigger the PlayerController to swap input maps.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameStateChange(newState);
            }
        }
        else
        {
            Debug.LogError($"State {newState} is not registered!");
        }
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdate();
        }
    }
}
