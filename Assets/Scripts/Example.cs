using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using System.Threading.Tasks;

public class Example : MonoBehaviour
{
    async void Start()
    {
        await InitializeServicesAsync();
        await RunExampleAsync();
    }

    private async Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e}");
        }
    }

    private async Task RunExampleAsync()
    {
        try
        {
            var module = new ScoreModuleBindings(CloudCodeService.Instance);

            //await module.InitializeCloudSave();
            await module.AddScore(25);

            Debug.Log("Score submitted successfully!");
        }
        catch (CloudCodeException e)
        {
            Debug.LogError($"Cloud Code error: {e}");
        }
    }
}
