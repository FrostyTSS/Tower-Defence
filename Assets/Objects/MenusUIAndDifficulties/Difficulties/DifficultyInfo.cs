using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "MapGameplayInfo/Difficulty", order = 1)]
public class DifficultyInfo : ScriptableObject
{

    public string DifficultyName;
    public int StartingHealth = 250;
    public int StartingCash = 250;
    public float BloonSpeedIncrease = 0;
    public Color MedalColour;
    public AudioClip VictoryTheme;
    public int DifficultyOrder = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
