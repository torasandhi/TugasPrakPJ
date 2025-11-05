using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;
using Unity.Services.Authentication;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _currentScore = 0;
    private int _highScore = 0;
    private CloudSaveBindings _cloudModule;
    private bool _initialized;

    public int CurrentScore => _currentScore;
    public int HighScore => _highScore;

    protected override async void Awake()
    {
        base.Awake();
        await InitializeCloudModule();
    }

    private async Task InitializeCloudModule()
    {
        if (_initialized) return;

        try
        {
            // Ensure AuthenticationManager (and Unity Services) have a chance to initialize.
            // This will return quickly if already initialized.
            await AuthenticationManager.Instance.InitializeAsync();

            // If already signed in, grab the cloud module and load high score immediately.
            if (AuthenticationManager.Instance.IsSignedIn)
            {
                _cloudModule = AuthenticationManager.Instance.CloudModule;
                await LoadHighScore();
                _initialized = true;
                Debug.Log("ScoreManager: Initialized successfully (signed in).");
                return;
            }

            // Not signed in yet: subscribe to the AuthenticationService SignedIn event so we can initialize when auth completes.
            AuthenticationService.Instance.SignedIn += OnAuthSignedIn;

            // Optionally attempt anonymous sign-in fallback if you want guest saves to work.
            // Comment out the following line if you require explicit username sign-in first.
            await AuthenticationManager.Instance.SignInAnonymouslyAsync();
            // If anonymous sign-in succeeded, OnAuthSignedIn will be invoked and initialization will continue there.
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ScoreManager initialization failed: {ex.Message}");
        }
    }

    private async void OnAuthSignedIn()
    {
        // Called on auth sign-in (anonymous or username). Unsubscribe immediately.
        AuthenticationService.Instance.SignedIn -= OnAuthSignedIn;

        try
        {
            // Create or obtain the cloud module from AuthenticationManager
            _cloudModule = AuthenticationManager.Instance.CloudModule ?? new CloudSaveBindings(CloudCodeService.Instance);

            await LoadHighScore();
            _initialized = true;
            Debug.Log("ScoreManager: Initialized successfully after sign-in.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ScoreManager: failed to finish initialization after sign-in: {ex.Message}");
        }
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
        }
    }

    public void ResetCurrentScore()
    {
        _currentScore = 0;
    }

    public async Task SaveHighScore()
    {
        if (!_initialized)
        {
            await InitializeCloudModule();
        }

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

    public async Task SaveCurrentScore()
    {
        if (!_initialized)
        {
            await InitializeCloudModule();
        }

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
