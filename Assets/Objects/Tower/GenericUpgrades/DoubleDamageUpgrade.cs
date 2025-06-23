using UnityEngine;


//[CreateAssetMenu(fileName = "DoubleDamage", menuName = "TowerInfo/Effects/DoubleDamageEffect", order = 1)]

public class DoubleDamageUpgrade : UpgradeEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public int MultiplierModifier = 2;
    public override void OnUnlocked(BaseTower TowerRef)
    {
        TowerRef.Damage *= MultiplierModifier;
    }
}
