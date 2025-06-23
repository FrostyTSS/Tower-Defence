using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameAOE", menuName = "TowerInfo/Effects/FlameAOEEffect", order = 1)]
public class FlamePin : UpgradeEffect
{
    public GameObject FireProjectile;
    /*
    public override void OnFire(BaseTower TowerRef)
    {

    }
    */
    public override void OnUnlocked(BaseTower TowerRef)
    {
        TowerRef.ProjectileType = FireProjectile;
        if (TowerRef is PinTowerScript PinRef)
        {
            PinRef.ProjectileAmount = 32;
            PinRef.TimeBetweenShots *= 0.55f;
            
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
