using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _currentScore = 0;
    private int _highScore = 0;
    private CloudSaveBindings _cloudModule;

    public int CurrentScore => _currentScore;
    public int HighScore => _highScore;

    protected override void Awake()
    {
        base.Awake();
        InitializeCloudModule();
    }

    private async void InitializeCloudModule()
    {
        // Wait for authentication to be ready
        if (!AuthenticationManager.Instance.IsSignedIn)
        {
            Debug.LogWarning("ScoreManager: No user signed in, scores won't be saved to cloud.");
            return;
        }

        _cloudModule = AuthenticationManager.Instance.CloudModule;
        await LoadHighScore();
    }

    private async Task LoadHighScore()
    {
        if (_cloudModule == null) return;

        try
        {
            var data = await _cloudModule.GET_PlayerData();
            if (data != null)
            {
                _highScore = data.Score;
                Debug.Log($"Loaded high score: {_highScore}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to load high score: {ex.Message}");
        }
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        Debug.Log($"Score increased by {points}. Current score: {_currentScore}");

        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            Debug.Log($"New high score: {_highScore}!");

            // Fire and forget the high score save
            _ = SaveHighScore();
        }
    }

    public void ResetCurrentScore()
    {
        _currentScore = 0;
    }

    private async Task SaveHighScore()
    {
        if (_cloudModule == null)
        {
            Debug.LogWarning("ScoreManager: Cloud module not available, high score won't be saved.");
            return;
        }

        try
        {
            // Get current character info to maintain it while updating score
            var currentData = await _cloudModule.GET_PlayerData();
            int characterIndex = currentData?.LastUsedCharacterIndex ?? 0;
            string characterName = currentData?.lastUsedCharacterName ?? "";

            // Update with new high score while preserving character data
            await _cloudModule.PUT_PlayerData(_highScore, characterIndex, characterName);
            Debug.Log($"Saved high score {_highScore} to cloud save.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to save high score: {ex.Message}");
        }
    }

    // Optional: Force save current score even if not high score
    public async Task SaveCurrentScore()
    {
        if (_cloudModule == null) return;

        try
        {
            var currentData = await _cloudModule.GET_PlayerData();
            int characterIndex = currentData?.LastUsedCharacterIndex ?? 0;
            string characterName = currentData?.lastUsedCharacterName ?? "";

            await _cloudModule.PUT_PlayerData(_currentScore, characterIndex, characterName);
            Debug.Log($"Saved current score {_currentScore} to cloud save.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to save current score: {ex.Message}");
        }
    }
}
