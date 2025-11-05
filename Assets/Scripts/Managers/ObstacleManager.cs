using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for List
using DG.Tweening; // Required for DOTween

public class ObstacleManager : Singleton<ObstacleManager>
{
    [Header("Spawning")]
    [SerializeField] private string obstacleTag = "Obstacle"; // Tag used in ObjectPoolManager
    [SerializeField] private Transform spawnPoint; // Where obstacles appear
    [SerializeField] private float spawnInterval = 3.0f; // Time between spawns

    [Header("Obstacle Setup")]
    [SerializeField] private int obstaclePartCount = 9;
    [Tooltip("The tag to apply to the invisible 'score' part.")]
    [SerializeField] private string scoreTag = "Score";
    [Tooltip("The tag all parts should have when they are solid obstacles.")]
    [SerializeField] private string originalPartTag = "Untagged"; // Or "Obstacle"
    [Tooltip("Check this if your game uses 2D Physics (Collider2D, Rigidbody2D).")]
    [SerializeField] private bool is2DGame = false;

    [Header("Tweening")]
    [SerializeField] private float targetYPosition = -7.0f;
    [SerializeField] private float tweenDuration = 5.0f;
    [SerializeField] private Ease moveEase = Ease.Linear;

    private Coroutine _spawnLoop;

    // Cache for performance, prevents GC alloc in loop
    private List<Transform> _obstaclePartsCache = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Example: Start spawning when the game starts
    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point is not set in ObstacleManager!", this);
            return;
        }
        StartSpawning();
    }

    // You can call this from a GameManager to start spawning
    public void StartSpawning()
    {
        if (_spawnLoop != null)
        {
            StopCoroutine(_spawnLoop);
        }
        _spawnLoop = StartCoroutine(SpawnLoop());
    }

    // Call this to stop spawning
    public void StopSpawning()
    {
        if (_spawnLoop != null)
        {
            StopCoroutine(_spawnLoop);
            _spawnLoop = null;
        }
    }

    #region --- Spawning and Preparation Logic ---

    private IEnumerator SpawnLoop()
    {
        // Endless loop
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        if (spawnPoint == null) return;

        Transform obstacleT = ObjectPoolManager.Instance.SpawnFromPool(
            obstacleTag,
            spawnPoint.position,
            Quaternion.identity
        );

        if (obstacleT == null)
        {
            Debug.LogWarning($"Could not spawn obstacle with tag '{obstacleTag}'. Pool empty or tag incorrect?");
            return;
        }

    
        if (!PrepareObstacleParts(obstacleT))
        {
            ObjectPoolManager.Instance.Despawn(obstacleT);
            return;
        }


        obstacleT.DOMoveY(targetYPosition, tweenDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {

                obstacleT.DOKill(); 
                ObjectPoolManager.Instance.Despawn(obstacleT);
            });
    }


    private bool PrepareObstacleParts(Transform obstacle)
    {
        if (obstacle.childCount < obstaclePartCount)
        {
            Debug.LogError($"Obstacle prefab needs {obstaclePartCount} children, but has {obstacle.childCount}.", obstacle);
            return false;
        }

        _obstaclePartsCache.Clear(); // Clear cache from previous run


        for (int i = 0; i < obstaclePartCount; i++)
        {
            Transform part = obstacle.GetChild(i);
            part.gameObject.SetActive(true); 
            ResetPartState(part);
            _obstaclePartsCache.Add(part);
        }

        int indexToModify = Random.Range(0, _obstaclePartsCache.Count);
        Transform partToModify = _obstaclePartsCache[indexToModify];
        ModifyPartForScore(partToModify);

        return true;
    }

    // Call this to clean up all active obstacles (e.g., on game over)
    public void ReturnAllObstacles()
    {
        StopSpawning();
        ObjectPoolManager.Instance.ReturnAllObjectsToPool();
    }

    #endregion

    #region --- Part State Helpers ---

    private void ResetPartState(Transform part)
    {
        part.gameObject.tag = originalPartTag;

        // Set Collider
        if (is2DGame)
        {
            var col = part.GetComponent<Collider2D>();
            if (col != null) col.isTrigger = false;
        }
        else
        {
            var col = part.GetComponent<Collider>();
            if (col != null) col.isTrigger = false;
        }

        // Set Opacity (Visible)
        SetPartOpacity(part, 1.0f);
    }

    private void ModifyPartForScore(Transform part)
    {
        part.gameObject.tag = scoreTag;

        // Set Collider
        if (is2DGame)
        {
            var col = part.GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }
        else
        {
            var col = part.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        // Set Opacity (Invisible)
        SetPartOpacity(part, 0.0f);
    }

    private void SetPartOpacity(Transform part, float alpha)
    {
        // Try to get a SpriteRenderer
        SpriteRenderer sprite = part.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
            return; // Found it, we're done
        }

        // If no Sprite, try to get a MeshRenderer
        Renderer mesh = part.GetComponent<Renderer>();
        if (mesh != null)
        {
            Color color = mesh.material.color;
            color.a = alpha;
            mesh.material.color = color;
            return;
        }
    }

    #endregion
}