using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // public CharacterController controller;
    private Dictionary<E_PlayerState, IPlayerState> _states = new Dictionary<E_PlayerState, IPlayerState>();
    private IPlayerState _currentState;

    private void Awake()
    {
        _states.Add(E_PlayerState.Movement, new PlayerState_Movement(this));
        // Add more states like Jumping, Attacking, Interacting, etc.
    }

    private void Start()
    {
        ChangeState(E_PlayerState.Movement);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(E_PlayerState nextState)
    {
        _currentState?.Exit();
        _currentState = _states[nextState];
        _currentState.Enter();
    }
}

public enum E_PlayerState { Movement, Interacting, Dead }

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

