using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct LastUsedCharacter
{
    public string name; 
    public int characterIndex;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public int playerScore;
    public List<LastUsedCharacter> listOfCharacters;
}
