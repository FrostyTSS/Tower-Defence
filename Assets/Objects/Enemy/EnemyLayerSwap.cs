using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyLayerList", menuName = "EnemyInfo/EnemyPopOrderList", order = 1)]
public class EnemyLayerSwap : ScriptableObject
{
    public List<GameObject> LayerPopOrder;
}
