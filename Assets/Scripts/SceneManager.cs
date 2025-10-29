using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
// Make sure to add this namespace!
using UnityEngine.SceneManagement;

/// <summary>
/// This now correctly implements the WaitLoading coroutine,
/// using asynchronous loading to show a UI while the next scene loads.
/// </summary>
public class SceneManager : Singleton<SceneManager>
{
    string current_level, previous_level;

    protected override void Awake()
    {
        base.Awake();
        // Set the initial scene as the current level
        // Assumes you start with one scene already open
        current_level = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Public method to start loading a new level.
    /// </summary>
    public void LoadLevel(string level, EGameState state)
    {
        // We start the coroutine to handle the entire loading process.
        StartCoroutine(WaitLoading(level, state));
    }

    /// <summary>
    /// Handles the asynchronous loading process, including the loading screen.
    /// </summary>
    IEnumerator WaitLoading(string level, EGameState state)
    {
        // 1. Show loading screen & disable input
        UIManager.Instance.SpawnUIByString("ui-loading");
        PlayerController.Instance.DisableInput();

        // 2. Unload the old scene (if there is one)
        if (!string.IsNullOrEmpty(current_level))
        {
            AsyncOperation unloadOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(current_level);
            while (unloadOperation != null && !unloadOperation.isDone)
            {
                yield return null; // Wait for unload to finish
            }
        }

        // 3. Load the new scene additively
        AsyncOperation loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null; // Wait for load to finish
        }

        // 4. Set the newly loaded scene as the "active" one
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(level));

        // 5. Update internal tracking
        previous_level = current_level;
        current_level = level;

        // 6. Hide/Destroy the loading screen
        // (Using DestroyUIByString as HideUI wasn't in your UIManager)
        UIManager.Instance.DestroyUIByString("ui-loading");

        // 7. Change game state
        // The PlayerController.InputActivate will be called automatically
        // because we subscribed it to the OnGameStateChanged event in GameManager.
        GameManager.Instance.GameStateMachine.ChangeState(state);
    }

    public void SetCurrentLevel(string level)
    {
        current_level = level;
    }

    public string GetCurrentLevel()
    {
        return current_level;
    }

    public string GetPreviousLevel()
    {
        return previous_level;
    }
}
