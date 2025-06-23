using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffStats", menuName = "TowerInfo/Effects/BuffStatsEffect", order = 1)]
public class BuffAttributes : UpgradeEffect
{

    public List<AttributeBuff> AttributeBuffList;

    public enum AttributeTypes
    {
        Damage, Range, AttackSpeed, Pierce
    }

    [Serializable]
    public struct AttributeBuff
    {
       public AttributeTypes AttributeType;
        public float BuffAmount;
        public bool Multiplicative;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public override void OnUnlocked(BaseTower TowerRef)
    {
        for (int i = 0; i < AttributeBuffList.Count; i++)
        {
            switch (AttributeBuffList[i].AttributeType)
            {
                case AttributeTypes.Damage:
                    if (AttributeBuffList[i].Multiplicative)
                    {
                        TowerRef.Damage *= (int)AttributeBuffList[i].BuffAmount;
                    }
                    else
                    {
                        TowerRef.Damage += (int)AttributeBuffList[i].BuffAmount;
                    }
                    break;

                case AttributeTypes.Range:
                    if (AttributeBuffList[i].Multiplicative)
                    {
                        TowerRef.Range *= AttributeBuffList[i].BuffAmount;
                    }
                    else
                    {
                        TowerRef.Range += AttributeBuffList[i].BuffAmount;
                    }
                    break;
                case AttributeTypes.AttackSpeed:
                    if (AttributeBuffList[i].Multiplicative)
                    {
                        TowerRef.TimeBetweenShots *= AttributeBuffList[i].BuffAmount;
                    }
                    else
                    {
                        Debug.Log("AttackSpeedBuffTest");
                        TowerRef.TimeBetweenShots += AttributeBuffList[i].BuffAmount;
                    }
                    break;
                case AttributeTypes.Pierce:
                    if (AttributeBuffList[i].Multiplicative)
                    {
                        TowerRef.BasePierce *= (int)AttributeBuffList[i].BuffAmount;
                    }
                    else
                    {
                        TowerRef.BasePierce += (int)AttributeBuffList[i].BuffAmount;
                    }
                    break;

            }
        }
        
    }
}
