using TMPro;
using UnityEngine;

public class State_Gameplay : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Gameplay State.");
    }

    public void OnUpdate()
    {
        TMP_Text scoreText = UIManager.Instance.GetUI("ui-gameplay").transform.Find("txt-score").GetComponent<TMP_Text>();
        scoreText.text = ScoreManager.Instance.CurrentScore.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Paused);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.GameOver);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Gameplay State.");
    }
}

