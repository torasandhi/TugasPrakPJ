using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;

[DefaultExecutionOrder(-99)]
public class AuthenticationManager : Singleton<AuthenticationManager>
{
    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
    public string PlayerId => AuthenticationService.Instance.PlayerId;
    
    // Add this to store the username
    private string _username;
    public string Username => _username;

    private CloudSaveBindings _cloudModule;
    public CloudSaveBindings CloudModule => _cloudModule;

    private bool _initialized;

    protected override async void Awake()
    {
        base.Awake();
        await InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += OnSignedIn;
            AuthenticationService.Instance.SignedOut += OnSignedOut;
            AuthenticationService.Instance.SignInFailed += OnSignInFailed;

            if (AuthenticationService.Instance.IsSignedIn)
                OnSignedIn();

            _initialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Authentication initialization failed: {ex.Message}");
            throw;
        }
    }

    private void OnSignedIn()
    {
        Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
        _cloudModule = new CloudSaveBindings(CloudCodeService.Instance);
    }

    private void OnSignedOut()
    {
        Debug.Log("Signed out");
        _cloudModule = null;
    }

    private void OnSignInFailed(Exception ex)
    {
        Debug.LogWarning($"Sign-in failed: {ex?.Message}");
    }

    public async Task SignUpAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new ArgumentException("Username and password required");

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            _username = username; // Store username after successful signup
            Debug.Log($"Sign up succeeded for user: {username}");
        }
        catch (AuthenticationException aex)
        {
            Debug.LogWarning($"Sign up failed: {aex.Message}");
            throw;
        }
    }

    public async Task SignInAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new ArgumentException("Username and password required");

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            _username = username; // Store username after successful signin
            Debug.Log($"Sign in succeeded for user: {username}");
        }
        catch (AuthenticationException aex)
        {
            Debug.LogWarning($"Sign in failed: {aex.Message}");
            throw;
        }
    }

    public async Task SignInAnonymouslyAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in anonymously as {AuthenticationService.Instance.PlayerId}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Anonymous sign-in failed: {ex.Message}");
                throw;
            }
        }
    }

    public void SignOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();
    }
}
