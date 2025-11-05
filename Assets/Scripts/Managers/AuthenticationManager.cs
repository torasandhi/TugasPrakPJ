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
    private CloudSaveBindings _cloudModule;
    public CloudSaveBindings CloudModule => _cloudModule;

    private bool _initialized;
    private TaskCompletionSource<bool> _cloudReadyTcs;
    private bool _signInInProgress;

    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
    public string PlayerId => AuthenticationService.Instance.PlayerId;

    protected override async void Awake()
    {
        base.Awake();
        await InitializeAsync();
    }

    // Initialize Unity Services and ensure anonymous sign-in + cloud bindings.
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            // subscribe early so OnSignedIn runs if sign-in completes concurrently
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            AuthenticationService.Instance.SignedOut += OnSignedOut;
            AuthenticationService.Instance.SignInFailed += OnSignInFailed;

            _cloudReadyTcs ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // If already signed in, create cloud bindings now.
            if (AuthenticationService.Instance.IsSignedIn)
            {
                OnSignedIn();
            }
            else
            {
                // Auto sign-in anonymously for a guest flow.
                // Guard against concurrent sign-in attempts and handle "already signing in" error.
                try
                {
                    await SignInAnonymouslyAsync();
                }
                catch (AuthenticationException aex)
                {
                    // If someone else is signing in concurrently, wait for completion instead of failing.
                    if (aex.Message != null && aex.Message.Contains("already signing in", StringComparison.OrdinalIgnoreCase))
                    {
                        // wait up to a short timeout for sign-in to complete
                        var timeoutMs = 5000;
                        var waited = 0;
                        var step = 200;
                        while (!AuthenticationService.Instance.IsSignedIn && waited < timeoutMs)
                        {
                            await Task.Delay(step);
                            waited += step;
                        }

                        if (AuthenticationService.Instance.IsSignedIn)
                        {
                            // Signed in by another caller — OnSignedIn should already have run.
                        }
                        else
                        {
                            // timed out, rethrow to let caller handle it
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            _initialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Authentication initialization failed: {ex.Message}");
            throw;
        }
    }

    // Anonymous sign-in only. Creates CloudSaveBindings in OnSignedIn.
    public async Task SignInAnonymouslyAsync()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return;

        // Prevent concurrent calls from this manager
        if (_signInInProgress)
            return;

        _cloudReadyTcs ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        _signInInProgress = true;
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously as {AuthenticationService.Instance.PlayerId}");
            // OnSignedIn handler will create CloudModule and set the TCS result.
        }
        catch (AuthenticationException aex)
        {
            // bubble up so caller can detect "already signing in" or other states
            Debug.LogWarning($"Anonymous sign-in failed: {aex.Message}");
            _cloudReadyTcs?.TrySetResult(false);
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Anonymous sign-in failed: {ex.Message}");
            _cloudReadyTcs?.TrySetResult(false);
            throw;
        }
        finally
        {
            _signInInProgress = false;
        }
    }

    private void OnSignedIn()
    {
        Debug.Log($"AuthenticationManager: Signed in as {AuthenticationService.Instance.PlayerId}");

        try
        {
            _cloudModule = new CloudSaveBindings(CloudCodeService.Instance);
            _cloudReadyTcs?.TrySetResult(true);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"AuthenticationManager: failed to create CloudSaveBindings: {ex.Message}");
            _cloudReadyTcs?.TrySetResult(false);
            _cloudModule = null;
        }
    }

    private void OnSignedOut()
    {
        Debug.Log("AuthenticationManager: Signed out");
        _cloudModule = null;
        _cloudReadyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private void OnSignInFailed(Exception ex)
    {
        Debug.LogWarning($"AuthenticationManager: Sign-in failed: {ex?.Message}");
        _cloudReadyTcs?.TrySetResult(false);
    }

    // Wait for CloudModule readiness. Returns true if CloudModule available within timeout.
    public async Task<bool> WaitForCloudModuleAsync(int timeoutMs = 3000)
    {
        if (CloudModule != null) return true;

        _cloudReadyTcs ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var delay = Task.Delay(timeoutMs);
        var completed = await Task.WhenAny(_cloudReadyTcs.Task, delay);
        if (completed == _cloudReadyTcs.Task)
        {
            try { return _cloudReadyTcs.Task.Result; }
            catch { return false; }
        }

        return false;
    }

    protected virtual void OnDestroy()
    {
        try
        {
            AuthenticationService.Instance.SignedIn -= OnSignedIn;
            AuthenticationService.Instance.SignedOut -= OnSignedOut;
            AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
        }
        catch
        {
            // ignore if AuthenticationService not available during shutdown
        }
    }
}