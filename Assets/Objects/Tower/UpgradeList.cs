using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "TowerInfo/UpgradeList", order = 1)]
public class UpgradeList : ScriptableObject
{
    public List<UpgradeInfo> PossibleUpgradeList;

}