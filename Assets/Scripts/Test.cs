using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
// Make sure this namespace matches your bindings file
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;

public class Test : MonoBehaviour
{
    private CloudSaveBindings module;
    private bool isInitialized = false;
    // We remove 'playerScore' because we'll get it from the server

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

            Debug.Log("---  INPUT FOR TESTING. ---");
            Debug.Log("PRESS [UP ARROW] to PUT (Score: 100, Char: 1)");
            Debug.Log("PRESS [RIGHT ARROW] to GET all data");
            Debug.Log("PRESS [DOWN ARROW] to PUT (Score: 250, Char: 2)");
            Debug.Log("PRESS [LEFT ARROW] to DELETE all data");
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

    //public async void Update()
    //{
    //    // Don't run until initialization is complete
    //    if (!isInitialized)
    //    {
    //        return;
    //    }

    //    try
    //    {
    //        // 3. PUT (Set 1)
    //        if (Input.GetKeyDown(KeyCode.UpArrow))
    //        {
    //            Debug.Log("Attempting to PUT (Score: 100, Char: 1)...");
    //            await module.PUT_PlayerData(100, 1);
    //            Debug.Log("PUT complete.");
    //        }

    //        // 4. GET 
    //        if (Input.GetKeyDown(KeyCode.RightArrow))
    //        {
    //            Debug.Log("Attempting to GET all player data...");
    //            // 'data' will be a PlayerData object
    //            var data = await module.GET_PlayerData();
    //            Debug.Log($"GET complete. Score: {data.Score}, CharacterIndex: {data.LastUsedCharacterIndex}");
    //        }

    //        // 5. PUT (Set 2)
    //        if (Input.GetKeyDown(KeyCode.DownArrow))
    //        {
    //            Debug.Log("Attempting to PUT (Score: 250, Char: 2)...");
    //            await module.PUT_PlayerData(250, 2);
    //            Debug.Log("PUT complete.");
    //        }

    //        // 1. DELETE 
    //        if (Input.GetKeyDown(KeyCode.LeftArrow))
    //        {
    //            Debug.Log("Attempting to DELETE all player data...");
    //            await module.DELETE_PlayerData();
    //            Debug.Log("DELETE complete.");
    //        }
    //    }
    //    catch (CloudCodeException exception)
    //    {
    //        // tell if the script name is wrong
    //        // or if a function is failing on the server.
    //        Debug.LogError($"Cloud Code Error: {exception.Message}\nDetails: {exception.Reason}");
    //        Debug.LogException(exception);
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogException(ex);
    //    }
    //}
}