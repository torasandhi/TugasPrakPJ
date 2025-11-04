using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerDataReference;

    private Rigidbody2D _rb;
    private Controls _controls;

    private Vector2 _moveInput;
    private float _playerSpeed = 5.0f;

   private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public async Task LoadAndInstantiateCharacter()
    {
        if (playerDataReference != null && playerDataReference.listOfCharacters != null && playerDataReference.listOfCharacters.Count > 0)
        {
            try
            {
                var cloudModule = new CloudSaveBindings(CloudCodeService.Instance);
                var cloudData = await cloudModule.GET_PlayerData();
                
                int lastCharacterIndex = cloudData?.LastUsedCharacterIndex ?? 0;
                LastUsedCharacter character = playerDataReference.listOfCharacters.Find(c => c.characterIndex == lastCharacterIndex);
                
                if (character.prefab != null)
                {
                    GameObject characterInstance = Instantiate(character.prefab, transform.position, Quaternion.identity, transform);
                }
                else
                {
                    Debug.LogError("Character prefab is null! Check PlayerData ScriptableObject.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to load cloud data, using first character: {ex.Message}");
              
                if (playerDataReference.listOfCharacters[0].prefab != null)
                {
                    GameObject characterInstance = Instantiate(
                        playerDataReference.listOfCharacters[0].prefab, 
                        transform.position, 
                        Quaternion.identity, 
                        transform
                    );
                }
            }
        }
        else
        {
            Debug.LogError("PlayerData reference or character list is invalid! Check inspector references.");
        }
    }

    private async void Start()
    {
        _rb.gravityScale = 0;

        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController singleton not found! Make sure it's in your scene.");
            return;
        }
        _controls = PlayerController.Instance.GetCurrentInput();

        // Subscribe to the input actions
        if (_controls != null)
        {
            _controls.Player.Move.performed += OnMovePerformed;
            _controls.Player.Move.canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogError("Controls object is null! Check PlayerController.cs");
        }

        // Load and instantiate character
        await LoadAndInstantiateCharacter();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        Vector2 moveVelocity = _moveInput.normalized * _playerSpeed;
        _rb.linearVelocity = moveVelocity;

        if (moveVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveVelocity.y, moveVelocity.x) * Mathf.Rad2Deg - 90f;

            _rb.rotation = angle;
        }
    }

    private void OnDestroy()
    {
        if (_controls != null)
        {
            _controls.Player.Move.performed -= OnMovePerformed;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }
    }

    #region PlayerData

    public int GetPlayerDataScore()
    {
        return playerDataReference.playerScore;
    }

    public void SetPlayerDataScore()
    {

    }

    public void AddPlayerDataScore()
    {

    }

    public void SubtractPlayerDataScore()
    {

    }

    public int GetPlayerDataCharacterIndex()
    {
        if (playerDataReference != null && 
            playerDataReference.listOfCharacters != null && 
            playerDataReference.listOfCharacters.Count > 0)
        {
            return playerDataReference.listOfCharacters[0].characterIndex;
        }
        
        Debug.LogWarning("No characters found in PlayerData, returning default index 0");
        return 0;
    }

    public void SetPlayerDataCharacterIndex(int index)
    {
    }

    #endregion
}

