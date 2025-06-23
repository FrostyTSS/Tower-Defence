using UnityEngine;
[CreateAssetMenu(fileName = "ProjectileSwap", menuName = "TowerInfo/Effects/GenericProjectileSwapEffect", order = 1)]
public class ProjectileSwap : UpgradeEffect
{
    public GameObject ProjectileToSwapTo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnUnlocked(BaseTower TowerRef)
    {
        TowerRef.ProjectileType = ProjectileToSwapTo;
    }
}
