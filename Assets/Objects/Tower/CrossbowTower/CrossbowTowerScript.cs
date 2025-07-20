using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;



public class CrossbowTowerScript : BaseTower
{
    public bool DebugSearchEnemy = false;
    


    private void Start()
    {
      
    }
    private void FixedUpdate()
    {
    //    WallHolder.transform.RotateAround(WallHolder.transform.position, Vector3.up, TimeBetweenShots * Time.fixedDeltaTime);
    }

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
    */

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

    */

    

    
}
