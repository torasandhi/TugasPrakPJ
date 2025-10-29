using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class PlayerController : Singleton<PlayerController>
{
    private Controls input;

    protected override void Awake()
    {
        base.Awake();
        input = new Controls();
        input.Enable();
    }

    public void InputActivate(EGameState type)
    {
        foreach (var item in input)
        {
            item.Disable();
        }

        switch (type)
        {
            case EGameState.MainMenu:
                input.UI.Enable();
                break;
            case EGameState.Gameplay:
                input.Player.Enable();
                break;
            case EGameState.GameOver:
                input.UI.Enable();
                break;
        }
    }

    public void DisableInput()
    {
        foreach (var item in input)
        {
            item.Disable();
        }
    }

    public Controls GetCurrentInput()
    {
        return input;
    }
}

public enum E_PlayerState { Movement, Interacting, Dead }

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

