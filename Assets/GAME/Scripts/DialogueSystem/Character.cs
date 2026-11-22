using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Dialogue/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public AudioClip sound;
    public float tono = 1f;
}