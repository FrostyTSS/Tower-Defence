using UnityEngine;

//[CreateAssetMenu(fileName = "Radar", menuName = "TowerInfo/Effects/RadarEffect", order = 1)]
public class RadarUpgrade : UpgradeEffect
{
    public bool Additive = true;
    public float NewRange;
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
        if (Additive)
        {
            TowerRef.Range += NewRange;
        }
        else
        {
            TowerRef.Range = NewRange;
        }
    }
}
