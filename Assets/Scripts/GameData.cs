using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/New Game Data")]
public class GameData : ScriptableObject
{
    [Header("Game Settings")]
    public string gameVersion = "1.0.0";
    public int maxPlayerLevel = 50;

    [Header("Player Stats")]
    public float playerBaseHealth = 100f;
    public float playerBaseSpeed = 5f;
    public float playerMaxSpeed = 5;
}
