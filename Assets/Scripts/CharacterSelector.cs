using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;

public class CharacterSelector : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData playerDataReference;

    [Header("UI (optional - will try to find by name if not assigned)")]
    [SerializeField] private Button btnCharacterLeft;
    [SerializeField] private Button btnCharacterRight;
    [SerializeField] private TMP_Text txtSelectedCharacter;

    private CloudSaveBindings cloudModule;
    private int selectedIndex = 0;

    private async void Start()
    {
        // Fallback: try to find buttons/text by name if not assigned in inspector
        if (btnCharacterLeft == null)
        {
            var go = GameObject.Find("btn-character-left");
            if (go != null) btnCharacterLeft = go.GetComponent<Button>();
        }
        if (btnCharacterRight == null)
        {
            var go = GameObject.Find("btn-character-right");
            if (go != null) btnCharacterRight = go.GetComponent<Button>();
        }
        if (txtSelectedCharacter == null)
        {
            var go = GameObject.Find("txt-selected-character");
            if (go != null) txtSelectedCharacter = go.GetComponent<TMP_Text>();
        }

        if (btnCharacterLeft != null) btnCharacterLeft.onClick.AddListener(OnLeftClicked);
        else Debug.LogWarning("CharacterSelector: left button not found or assigned.");

        if (btnCharacterRight != null) btnCharacterRight.onClick.AddListener(OnRightClicked);
        else Debug.LogWarning("CharacterSelector: right button not found or assigned.");

        if (playerDataReference == null)
        {
            Debug.LogError("CharacterSelector: playerDataReference is null. Assign your PlayerData ScriptableObject in the inspector.");
            return;
        }
        if (playerDataReference.listOfCharacters == null || playerDataReference.listOfCharacters.Count == 0)
        {
            Debug.LogError("CharacterSelector: PlayerData.listOfCharacters is empty.");
            return;
        }

        // Initialize services & cloud module (best-effort)
        await InitializeCloudModule();

        // Initialize selected index (start at 0). You can replace this with a saved value if available.
        selectedIndex = Mathf.Clamp(selectedIndex, 0, playerDataReference.listOfCharacters.Count - 1);
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (btnCharacterLeft != null) btnCharacterLeft.onClick.RemoveListener(OnLeftClicked);
        if (btnCharacterRight != null) btnCharacterRight.onClick.RemoveListener(OnRightClicked);
    }

    private void OnLeftClicked() => Cycle(-1);
    private void OnRightClicked() => Cycle(+1);

    private void Cycle(int delta)
    {
        if (playerDataReference == null || playerDataReference.listOfCharacters == null || playerDataReference.listOfCharacters.Count == 0)
            return;

        int count = playerDataReference.listOfCharacters.Count;
        selectedIndex = (selectedIndex + delta) % count;
        if (selectedIndex < 0) selectedIndex += count;

        UpdateUI();

        // fire-and-forget async save (errors logged)
        _ = SaveSelectedCharacterAsync(selectedIndex);
    }

    private void UpdateUI()
    {
        if (playerDataReference == null || playerDataReference.listOfCharacters == null || playerDataReference.listOfCharacters.Count == 0)
            return;

        var character = playerDataReference.listOfCharacters[selectedIndex];
        if (txtSelectedCharacter != null)
        {
            txtSelectedCharacter.text = character.name ?? $"Character {character.characterIndex}";
        }
        else
        {
            Debug.Log($"Selected Character: {character.name} (index {character.characterIndex})");
        }
    }

    private async Task InitializeCloudModule()
    {
        if (cloudModule != null) return;

        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in anonymously as {AuthenticationService.Instance.PlayerId}");
            }

            cloudModule = new CloudSaveBindings(CloudCodeService.Instance);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"CharacterSelector: Cloud initialization failed: {ex.Message}");
            cloudModule = null;
        }
    }

    private async Task SaveSelectedCharacterAsync(int characterIndex)
    {
        if (cloudModule == null)
        {
            await InitializeCloudModule();
            if (cloudModule == null)
            {
                Debug.LogWarning("CharacterSelector: Cloud module unavailable, skipping save.");
                return;
            }
        }

        try
        {
            await cloudModule.PUT_PlayerCharacter(characterIndex);
            Debug.Log($"Saved selected character index {characterIndex} to Cloud Save.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"CharacterSelector: Failed to save selected character: {ex.Message}");
        }
    }
}