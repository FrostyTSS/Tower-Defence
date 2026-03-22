using System;
using System.Collections;
using UnityEngine;

public class SniperTowerScript : BaseTower
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool AbilityActive = false;
    public bool AbilitySpinPause = false;
    float Timer = 0;
    float PauseTime = 2;
    //put time to spin in ability script
     IEnumerator AbilityShoot()
    {
        //pause for a second, raycast forward, if hit, shoot, pause a bit longer then keep spininng
        yield return new WaitForSeconds(PauseTime);
    }

     void AbilityCaller()
    {
        StartCoroutine(AbilityShoot());
    }

    public void AbilityActivate()
    {
        AbilityActive = true;
        InvokeRepeating("AbilityCaller", 0.5f, 0.5f);
    }

    void FixedUpdate()
    {
        if (AbilityActive && AbilitySpinPause)
        {
            Timer += Time.fixedDeltaTime;
            //Debug.Log(Timer);
            /*
            transform.Rotate(new Vector3(0, Speed, 0), Space.World);
            if (Timer >= TimeToSpin)
            {
                StopSpin();
            }
            */
        }
        //transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(0, (transform.rotation.eulerAngles.y + Time.deltaTime) * Speed, 0));
    }

    public void StartSpin()
    {
       // SwordSwingObj.SetActive(true);
        AbilityActive = true;
        this.GetComponent<BaseTower>().RotateToShoot = false;
    }

    public void StopSpin()
    {

        AbilityActive = false;
        this.GetComponent<BaseTower>().RotateToShoot = true;
       // SwordSwingObj.SetActive(false);
        Timer = 0;
    }

    public override void ShootEnemy()
    {
        if (CurrentTarget && CurrentTarget.Targetable)
        {
            CleanupProjectiles();

            if (!CurrentTarget && Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range)
            {
                FindEnemy();
            }
            else if (CurrentTarget.EnemyFutureDamage < CurrentTarget.Health)
            {
                CurrentTarget.EnemyFutureDamage += Damage;
                if (RotateToShoot)
                {
                    transform.LookAt(CurrentTarget.transform);
                }
                for (int i = 0; i < UpgradeList.Count; i++)
                {
                    for (int j = 0; j < UpgradeList[i].Effects.Count; j++)
                    {
                        UpgradeList[i].Effects[j].OnHit(this, CurrentTarget);
                    }
                }
                CurrentTarget.TakeDamage(Damage, this);
                //if out of range
                /*
                if (CurrentTarget.Health <= 0 && Searching == false)
                {
                    // Destroy(CurrentTarget.gameObject);
                    // LevelPath.Enemies.TrimExcess();
                    CurrentTarget.KillEnemy();
                    FindEnemy();
                }
                */

                /*
                if ((ProjectileList.Count + 1) * Damage <= CurrentTarget.Health) // make sure you only use the amount of shots necessary to kill
                {
                    if (RotateToShoot)
                    {
                        transform.LookAt(CurrentTarget.transform);
                    }
                    // Quaternion RotatedForwardQ = Quaternion.LookRotation(TowerRef.CurrentTarget.transform.position - TowerRef.transform.position);
                    ProjectileList.Add(Instantiate(ProjectileType, transform.position, Quaternion.LookRotation(CurrentTarget.transform.position - transform.position)).GetComponent<ProjectileBase>());
                    ProjectileList[ProjectileList.Count - 1].ProjOwner = this;
                    ProjectileList[ProjectileList.Count - 1].ProjTargetObj = CurrentTarget;
                    for (int i = 0; i < UpgradeList.Count; i++) // better then number ids maybe?
                    {
                        UpgradeList[i].Effect.OnFire(this);
                    }
                }

                //ProjectileList[ProjectileList.Count - 1].ProjTargetPos = CurrentTarget.transform.position;
                Debug.Log("Shooting!");
                /*
                Debug.DrawRay(transform.position, (CurrentTarget.transform.position - transform.position), Color.green);
                CurrentTarget.Health -= Damage;
                //if out of range
                if (CurrentTarget.Health <= 0 && Searching == false)
                {
                    // Destroy(CurrentTarget.gameObject);
                    // LevelPath.Enemies.TrimExcess();
                    CurrentTarget.Death();
                    FindEnemy();
                }
                */
            }
            else
            {
                FindEnemy();
            }
        }
        else
        {
            FindEnemy();
        }
    }
}
