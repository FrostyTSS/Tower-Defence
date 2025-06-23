using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;



public class DartTowerScript : BaseTower
{
    public bool DebugSearchEnemy = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //to do: strong targeting

    /*
    void FixedUpdate()
    {
        if (LevelPath.WaveInProgress || DebugSearchEnemy == true)
        {
            if (!CurrentTarget && Searching == false )
            {

                FindEnemy();



            }
            else if (CurrentTarget && Searching == false)
            {
                DelayTimer -= Time.fixedDeltaTime;
                if (DelayTimer <= 0)
                {
                    DelayTimer = TimeBetweenShots;
                    ShootEnemy();
                }
            }
        }
    }


    /*
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(0, 0.7f, 0.7f, 0.35f);
        Gizmos.DrawSphere(transform.position, Range);
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelPath.WaveInProgress)
        {
            if (!CurrentTarget && Searching == false)
            {

                FindEnemy();



            }
            else if (CurrentTarget && Searching == false)
            {
                DelayTimer -= Time.fixedDeltaTime;
                if (DelayTimer <= 0)
                {
                    DelayTimer = TimeBetweenShots;
                    ShootEnemy();
                }
            }
        }
    }

    
    /*
    public override void ShootEnemy()
    {
        if (CurrentTarget)
        {
            if (Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range)
            {
                FindEnemy();
            }
            else
            {
                Debug.DrawRay(transform.position, (CurrentTarget.transform.position - transform.position), Color.green);
                CurrentTarget.Health -= 1;
                //if out of range
                if (CurrentTarget.Health <= 0 && Searching == false)
                {
                    // Destroy(CurrentTarget.gameObject);
                    // LevelPath.Enemies.TrimExcess();
                    CurrentTarget.Death();
                    FindEnemy();
                }
            }
        }
    }

    */
}
