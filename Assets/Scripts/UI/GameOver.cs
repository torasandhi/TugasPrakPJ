using System.Security.Authentication.ExtendedProtection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : Singleton<GameOver>
{
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highestScoreText;
    [SerializeField] private Button btnMainMenu;

    protected override void Awake()
    {
        base.Awake();
        if (btnMainMenu != null) btnMainMenu.onClick.AddListener(SaveAndResetScore);
    }

    public void ShowScore()
    {
        if (currentScoreText != null)
            currentScoreText.text = $"Score: {ScoreManager.Instance.CurrentScore}";

        if (highestScoreText != null)
            highestScoreText.text = $"High Score: {ScoreManager.Instance.HighScore}";
    }

    private async void SaveAndResetScore()
    {
        try
        {
            await ScoreManager.Instance.SaveHighScore();
            ScoreManager.Instance.ResetCurrentScore();
            SceneManager.Instance.LoadLevel("MainMenu", EGameState.MainMenu);
            UIManager.Instance.DestroyUIByString("ui-gameover");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save score: {ex.Message}");
        }
    }
}
