using UnityEngine;

[CreateAssetMenu(fileName = "LeadBust", menuName = "TowerInfo/Effects/LeadBustingEffect", order = 1)]
public class EnableLeadPiercing : UpgradeEffect
{
    public override void OnUnlocked(BaseTower TowerRef)
    {
        if (TowerRef.ProjectileType)
        {
            TowerRef.ProjectileType.GetComponent<ProjectileBase>().LeadBreaking = true;
        }
    }
}
