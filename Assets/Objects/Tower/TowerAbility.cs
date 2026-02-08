using UnityEngine;



[System.Serializable]
[CreateAssetMenu(fileName = "TowerAbilityBase", menuName = "TowerInfo/TowerAbilityBase", order = 1)]

public class TowerAbility : ScriptableObject
{

    public float Cooldown = 5f;
    public Sprite AbilityImage;
    public AudioClip AbilityActivateNoise;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public virtual void OnUse(BaseTower TowerRef)
    {

    }
    
}
