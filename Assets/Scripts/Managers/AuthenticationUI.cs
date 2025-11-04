using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationUI : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    [Header("Buttons")]
    [SerializeField] private Button signInButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button playAsGuestButton;

    [Header("Status")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject loadingIndicator;

    private void Start()
    {
        signInButton.onClick.AddListener(() => _ = HandleSignIn());
        signUpButton.onClick.AddListener(() => _ = HandleSignUp());
        playAsGuestButton.onClick.AddListener(() => _ = HandleGuestPlay());

        SetInteractable(true);
        statusText.text = string.Empty;
        if (loadingIndicator) loadingIndicator.SetActive(false);
    }

    private void SetInteractable(bool interactable)
    {
        usernameField.interactable = interactable;
        passwordField.interactable = interactable;
        signInButton.interactable = interactable;
        signUpButton.interactable = interactable;
        playAsGuestButton.interactable = interactable;
    }

    private async Task HandleSignIn()
    {
        await HandleAuth(async () =>
        {
            await AuthenticationManager.Instance.SignInAsync(
                usernameField.text.Trim(),
                passwordField.text
            );
            OnAuthenticationSuccess();
        });
    }

    private async Task HandleSignUp()
    {
        await HandleAuth(async () =>
        {
            await AuthenticationManager.Instance.SignUpAsync(
                usernameField.text.Trim(),
                passwordField.text
            );
            OnAuthenticationSuccess();
        });
    }

    private async Task HandleGuestPlay()
    {
        await HandleAuth(async () =>
        {
            await AuthenticationManager.Instance.SignInAnonymouslyAsync();
            OnAuthenticationSuccess();
        });
    }

    private async Task HandleAuth(Func<Task> authAction)
    {
        SetInteractable(false);
        if (loadingIndicator) loadingIndicator.SetActive(true);
        statusText.text = "Please wait...";

        try
        {
            await authAction();
        }
        catch (AuthenticationException aex)
        {
            statusText.text = $"Error: {aex.Message}";
            SetInteractable(true);
        }
        catch (Exception ex)
        {
            statusText.text = "An error occurred. Please try again.";
            Debug.LogError($"Auth error: {ex.Message}");
            SetInteractable(true);
        }
        finally
        {
            if (loadingIndicator) loadingIndicator.SetActive(false);
        }
    }

    private void OnAuthenticationSuccess()
    {
        SceneManager.Instance.LoadLevel("MainMenu", EGameState.MainMenu);
    }

    private void OnDestroy()
    {
        if (signInButton) signInButton.onClick.RemoveAllListeners();
        if (signUpButton) signUpButton.onClick.RemoveAllListeners();
        if (playAsGuestButton) playAsGuestButton.onClick.RemoveAllListeners();
    }
}