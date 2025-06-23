using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Upgrade", menuName = "TowerInfo/UpgradeInfo", order = 1)]
public class UpgradeInfo : ScriptableObject
{
    [SerializeField]
    public string UpgradeName;
    public string UpgradeDescription;
    //public string NextUpgradeName; // ah damn I should just try referencing it..
    public UpgradeInfo NextUpgrade;
    public Sprite UpgradeImage;
    [SerializeField]
    public int Cost;
    public bool StartingUpgrade;
    [SerializeReference]
    //public UpgradeEffect Effect;
    public List<UpgradeEffect> Effects;



}


