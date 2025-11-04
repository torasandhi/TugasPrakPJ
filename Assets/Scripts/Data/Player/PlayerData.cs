using UnityEditor.Build.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public int playerScore;
    public int lastUsedCharacterIndex;
}
