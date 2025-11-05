using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;

public class Test : MonoBehaviour
{
    private CloudSaveBindings module;
    private bool isInitialized = false;

    public async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as Player: {AuthenticationService.Instance.PlayerId}");
            }

            module = new CloudSaveBindings(CloudCodeService.Instance);
            isInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Initialization Failed!");
            Debug.LogException(ex);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L)) { ScoreManager.Instance.AddScore(10); }
    }

}