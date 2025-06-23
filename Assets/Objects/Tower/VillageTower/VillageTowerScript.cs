using System.Collections.Generic;
using UnityEngine;

public class VillageTowerScript : BaseTower
{

    public List<BaseTower> CurrentlyAffectedTowers;
    public List<UpgradeInfo> TowerBuffList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetUpgradeCount()
    {
        return UpgradeList.Count;
    }

    //public override void FixedUpdate()
    private void FixedUpdate()
    {
        {
            // base.FixedUpdate();
            DelayTimer -= Time.fixedDeltaTime;
            if (DelayTimer <= 0)
            {
                DelayTimer = TimeBetweenShots;
                Debug.Log("Village Upgrade Sweep");
                ScanForTowers();
            }
        }
    }

    void UnapplyAllUpgradesOnRemoval()
    {
        for (int i = 0; i < CurrentlyAffectedTowers.Count; i++)
        {
            RemoveTowerBuffs(CurrentlyAffectedTowers[i], this);
        }
    }

    void RemoveTowerBuffs(BaseTower TowerRef, VillageTowerScript BuffRef)
    {
        for (int i = 0; i < TowerRef.UpgradeList.Count; i++) // better then number ids maybe?
        {
            for (int j = 0; j < BuffRef.TowerBuffList.Count; j++)
            {
                if (TowerRef.UpgradeList[i] == BuffRef.TowerBuffList[j])
                {
                    TowerRef.UpgradeList.RemoveAt(i);
                    Debug.Log("Removed at" + i + "<tower > buff" + j);
                }
            }
           
        }
        TowerRef.UpgradeList.TrimExcess();
    }

    void ApplyBuffsToAffectedTowers()
    {
        Debug.Log("APPLY!");
        for (int i = 0; i < CurrentlyAffectedTowers.Count; i++)
        {
            Debug.Log("COUNT!!");
            for (int j = 0; j < TowerBuffList.Count; j++)
            {
                Debug.Log("UPGRADE!!");
                CurrentlyAffectedTowers[i].ApplyUpgrade(TowerBuffList[j], true); // apply 
            }
           
        }
    }

    void ScanForTowers()
    {

        LayerMask mask = LayerMask.GetMask("Tower");
        //make sure its the right layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range, mask);
        foreach (var hitCollider in hitColliders)
        {
            //hitCollider.SendMessage("AddDamage");
            if (hitCollider.GetComponent<BaseTower>() && hitCollider.GetComponent<BaseTower>() is not VillageTowerScript)
            {
                BaseTower CollidiedTower = hitCollider.GetComponent<BaseTower>();
                if (CollidiedTower.CurrentBuffer == null) // if no buff tower
                {
                    CollidiedTower.CurrentBuffer = this;
                    Debug.Log("CurrentBuffer");
                    CurrentlyAffectedTowers.Add(hitCollider.GetComponent<BaseTower>());
                }
                else if (CollidiedTower.CurrentBuffer && GetUpgradeCount() > CollidiedTower.CurrentBuffer.GetUpgradeCount()) //if this tower is better
                {
                    for (int i = 0; i < CollidiedTower.CurrentBuffer.CurrentlyAffectedTowers.Count; i++)
                    {
                        if (CollidiedTower == CollidiedTower.CurrentBuffer.CurrentlyAffectedTowers[i])
                        {
                            RemoveTowerBuffs(CollidiedTower, CollidiedTower.CurrentBuffer);
                            CollidiedTower.CurrentBuffer.CurrentlyAffectedTowers.RemoveAt(i);
                            Debug.Log("Changed Buffer!");
                            break;
                           
                        }
                    }
                    CollidiedTower.CurrentBuffer = this;
                    CurrentlyAffectedTowers.Add(hitCollider.GetComponent<BaseTower>());
                }

               
            }
        }

       
        ApplyBuffsToAffectedTowers();
    }
    
}
