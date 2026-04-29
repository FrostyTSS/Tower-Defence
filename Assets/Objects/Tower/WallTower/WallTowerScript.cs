using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;



public class WallTowerScript : BaseTower
{
    public bool DebugSearchEnemy = false;
    public GameObject WallHolder;
    public bool Rotating = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(RotateAroundOnBreak());
        }
    }

    public IEnumerator RotateAroundOnBreak()
    {

        float CurrentRot = 0;
        
        float MaxRot = 85; // idk why it goes 5 over normally? 85 if bugged
        float CurrentActualRot = transform.eulerAngles.y;
       // Debug.Log(CurrentActualRot);
        if (Rotating == false) // if a divisble of 90
        {
            Rotating = true;
            //WallHolder.transform.RotateAround(WallHolder.transform.position, Vector3.up, 90);
            while (CurrentRot <= MaxRot)
            {
              //  Debug.Log(CurrentRot);

                // AbilityTimer += Time.fixedDeltaTime + AbilityDelay;
                // DelayTimer -= Time.fixedDeltaTime;
                //AbilityTimer += Time.fixedDeltaTime;
                transform.Rotate(new Vector3(0, TimeBetweenShots, 0) * Time.fixedDeltaTime, Space.World);
                CurrentRot += TimeBetweenShots * Time.fixedDeltaTime;
                yield return null;
            }

            Rotating = false;
            transform.eulerAngles = new Vector3(0, (int)(CurrentActualRot + 90) ,0);
            
        }
        // transform.Rotate(new Vector3(0, MaxRot, 0), Space.World);
        yield return null;
    }

   
    //private void FixedUpdate() // trigger spin on wall break?
    //{
       // WallHolder.transform.RotateAround(WallHolder.transform.position, Vector3.up, TimeBetweenShots * Time.fixedDeltaTime);
    //}

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

    public override void ShootEnemy()
    {

      //  WallHolder.transform.RotateAround(WallHolder.transform.position, Vector3.up, 1.0f * Time.fixedDeltaTime);
        /*
        if (CurrentTarget)
        {
            if (Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range)
            {
                FindEnemy();
            }
            else
            {
                Debug.DrawRay(transform.position, (CurrentTarget.transform.position - transform.position), Color.green);
                
            }
        }
        */
    }

    
}
