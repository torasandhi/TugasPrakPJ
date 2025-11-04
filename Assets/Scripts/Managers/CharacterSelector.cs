using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine.InputSystem.Android;

public class CharacterSelector : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData playerDataReference;

    [Header("UI (optional - will try to find by name if not assigned)")]
    [SerializeField] private Button btnCharacterLeft;
    [SerializeField] private Button btnCharacterRight;
    [SerializeField] private Button btnGameStart;
    [SerializeField] private TMP_Text txtSelectedCharacter;

    private CloudSaveBindings cloudModule;
    private int selectedIndex = 0;
    private string selectedName = " ";

    private async void Start()
    {
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

        if (btnGameStart != null) btnGameStart.onClick.AddListener(OnGameStart);



        // Initialize services & cloud module (best-effort)
        await InitializeCloudModule();
        await LoadSavedCharacterData();

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
    private void OnGameStart()
    {
        SceneManager.Instance.LoadLevel("Gameplay", EGameState.Gameplay);
        _ = SaveSelectedCharacterAsync(selectedIndex, selectedName);
    }

    private void Cycle(int delta)
    {
        if (playerDataReference == null || playerDataReference.listOfCharacters == null || playerDataReference.listOfCharacters.Count == 0)
            return;

        int count = playerDataReference.listOfCharacters.Count;
        selectedIndex = (selectedIndex + delta) % count;
        if (selectedIndex < 0) selectedIndex += count;
        selectedName = playerDataReference.listOfCharacters[selectedIndex].name;

        UpdateUI();
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
            // Use AuthenticationManager instead of direct initialization
            if (!AuthenticationManager.Instance.IsSignedIn)
            {
                Debug.LogWarning("CharacterSelector: No user is signed in!");
                return;
            }

            cloudModule = AuthenticationManager.Instance.CloudModule;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"CharacterSelector: Cloud initialization failed: {ex.Message}");
            cloudModule = null;
        }
    }

    private async Task LoadSavedCharacterData()
    {
        if (cloudModule == null) return;

        try
        {
            var data = await cloudModule.GET_PlayerData();
            if (data != null)
            {
                selectedIndex = data.LastUsedCharacterIndex;
                selectedName = data.lastUsedCharacterName;
                Debug.Log($"Loaded character data: Index={selectedIndex}, Name={selectedName}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to load character data: {ex.Message}");
            // Fallback to first character
            selectedIndex = 0;
            selectedName = playerDataReference?.listOfCharacters?[0].name ?? "Character 1";
        }
    }

    private async Task SaveSelectedCharacterAsync(int characterIndex, string characterName)
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
            // save the selected character index
            await cloudModule.PUT_PlayerCharacter(characterIndex);

            // save the selected character name
            await cloudModule.PUT_Character(characterName);

            Debug.Log($"Saved selected character index {characterIndex} ('{characterName}') to Database.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"CharacterSelector: Failed to save selected character: {ex.Message}");
        }
    }
}