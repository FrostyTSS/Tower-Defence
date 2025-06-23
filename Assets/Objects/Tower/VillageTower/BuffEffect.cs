//using UnityEditor.Experimental.GraphView;
using UnityEngine;
[CreateAssetMenu(fileName = "BuffEffect", menuName = "TowerInfo/Effects/Village/BuffEffect", order = 1)]
public class BuffEffect : UpgradeEffect
{
    public UpgradeInfo EffectForTowersInRange; // due to the way its setup, gotta do it like this :/ buff effect for village, this effect for other
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
        if (TowerRef is VillageTowerScript VillageRef)
        {
            Debug.Log("Added:" + EffectForTowersInRange.name);
            VillageRef.TowerBuffList.Add(EffectForTowersInRange);
        }
    }
}
