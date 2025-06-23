using UnityEngine;



[System.Serializable]
[CreateAssetMenu(fileName = "UpgradeEffectBase", menuName = "TowerInfo/UpgradeEffectBase", order = 1)]

public class UpgradeEffect : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void OnHit(BaseTower TowerRef, BaseEnemy EnemyRef)
    {

    }
    public virtual void OnFire(BaseTower TowerRef)
    {

    }
    public virtual void OnUnlocked(BaseTower TowerRef)
    {
      
    }
}
