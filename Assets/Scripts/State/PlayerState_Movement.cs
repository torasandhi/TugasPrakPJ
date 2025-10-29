using UnityEngine;

public class PlayerState_Movement : IPlayerState
{
    private PlayerController _player;

    public PlayerState_Movement(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Player entering Movement state.");
    }

    public void Update()
    {
        // - Read movement input from an InputManager
        // - Move the player's CharacterController
        // - Update the Animator with speed values
    }

    public void Exit()
    {
        Debug.Log("Player exiting Movement state.");
    }
}

