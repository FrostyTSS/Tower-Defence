using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveInfo", menuName = "PathInfo/WaveInfo")]
public class WaveInfo : ScriptableObject
{
    public List<WaveGroupInfo> WaveOrder;
    public string DialogueToLoad;
}


[Serializable]
public struct WaveGroupInfo 
{
    //public EnemyDef.EnemySpawnType EnemyType;

    public GameObject Enemy;
    public float DelayUntilNextEnemyType;

    //for spam, do later
      public int AmountOfEnemy;
    public float SwarmDelay;
    public bool EnemyCamo;
    
}