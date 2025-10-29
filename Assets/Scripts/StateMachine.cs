using System.Collections.Generic;

public class State_Machine
{
    private Dictionary<E_GameState, IState> _states = new Dictionary<E_GameState, IState>();
    private IState _currentState;

    public State_Machine()
    {
        _states.Add(E_GameState.MainMenu, new State_MainMenu());
        _states.Add(E_GameState.Gameplay, new State_Gameplay());
        _states.Add(E_GameState.Paused, new State_Pause());
    }

    public void ChangeState(E_GameState nextState)
    {
        _currentState?.Exit();
        _currentState = _states[nextState];
        _currentState.Enter();
        GameManager.Instance.TriggerGameStateChange(nextState);
    }

    public void Update()
    {
        _currentState?.Update();
    }
}

