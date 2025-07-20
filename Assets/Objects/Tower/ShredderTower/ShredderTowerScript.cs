using System.Collections.Generic;

using UnityEngine;




public class ShredderTowerScript : BaseTower
{
    public bool DebugSearchEnemy = false;
   
    public List<BaseEnemy> CurrentlySuckedEnemies;
    public int MaxSuckedEnemy = 4;
    public float TimeBetweenShreds = 1.5f;
    public int MaxShredCount = 2;

    //make private later
  public  float SecondsTillDamage = 0;
    public int ShreddedCount = 0;
    public bool ShreddingCycleDone = false;

    private void Start()
    {
    
    }

    private void FixedUpdate()
    {

        if (LevelPath && LevelPath.WaveInProgress)
        {
            //moved here so you don't have that awkward pause everytime it swaps targets
            if (DelayTimer > 0)
            {
                DelayTimer -= Time.fixedDeltaTime;
            }

            if (!CurrentTarget && Searching == false)
            {

                FindEnemy();



            }
            else if (CurrentTarget && Searching == false)
            {
                // DelayTimer -= Time.fixedDeltaTime;
                if (DelayTimer <= 0)
                {
                    DelayTimer = TimeBetweenShots;
                    ShootEnemy();
                }
            }
        }


        if (CurrentlySuckedEnemies.Count > 0)
        {
            SecondsTillDamage += Time.deltaTime;
            for (int i = 0; i < CurrentlySuckedEnemies.Count; i++)
            {
                BaseEnemy CurrentEnemy = CurrentlySuckedEnemies[i];
                if (CurrentEnemy == null || CurrentEnemy.Health <= 0)
                {
                    CurrentlySuckedEnemies.RemoveAt(i); // clear out list if they die to other methods, see if it works..
                }
                else
                {
                    
                    //CurrentEnemy.transform.position = Vector3.Slerp(CurrentEnemy.transform.position, this.gameObject.transform.position,  Time.fixedDeltaTime);
                    CurrentEnemy.transform.position = Vector3.MoveTowards(CurrentEnemy.transform.position, transform.position, 7.5f * Time.fixedDeltaTime);

                    if (Vector3.Distance(CurrentEnemy.transform.position, transform.position) <= 0.2f)
                    {
                        CurrentEnemy.TakeDamage(Damage, this);
                        LetEnemyGo(CurrentEnemy, i);
                    }
                }
                /*
                if (SecondsTillDamage >= TimeBetweenShreds)
                {
                    ShreddingCycleDone = true;
                    CurrentEnemy.TakeDamage(Damage, this);
                }
                */


            }

            /*
            if (ShreddingCycleDone)
            {
                ShreddingCycleDone = false;
                SecondsTillDamage = 0;
                ShreddedCount++;
                if (ShreddedCount >= MaxShredCount)
                {
                    Debug.Log("RELEASING ENEMEY");
                    ShreddedCount = 0;
                    LetEnemiesGo();
                    return;
                }
            }
            */
        }

       
    }





    public void LetEnemyGo(BaseEnemy enemy, int ListPlacement)
    {

        
            BaseEnemy CurrentEnemy = enemy;


            CurrentEnemy.StartCoroutine(CurrentEnemy.SpeedUp(7.5f, 1f));
            //  CurrentEnemy.CurrentSpeed = CurrentEnemy.MaxSpeed;
            if (PathHolder.instance && PathHolder.instance.Positions.Count > 0)
            {
                CurrentEnemy.CurrentPath -= 1;
                CurrentEnemy.SetTarget(LevelPath.Positions[CurrentEnemy.CurrentPath]);
                /*
                if (CurrentEnemy.CurrentPath - 1 >= 0) // check if closer to previous path node
                {
                    float CurrentDistance = Vector3.Distance(CurrentEnemy.transform.position, PathHolder.instance.Positions[CurrentEnemy.CurrentPath]);
                    float PrevDistance = Vector3.Distance(CurrentEnemy.transform.position, PathHolder.instance.Positions[CurrentEnemy.CurrentPath - 1]);
                    if (PrevDistance < CurrentDistance)
                    {
                        CurrentEnemy.CurrentPath -= 1;
                    }
                }
              */

            }

        
        CurrentlySuckedEnemies.RemoveAt(ListPlacement);
        //
    }


    public void LetEnemiesGo()
    {
       
        for (int i = 0; i < CurrentlySuckedEnemies.Count; i++)
        {
            BaseEnemy CurrentEnemy = CurrentlySuckedEnemies[i];


            CurrentEnemy.StartCoroutine(CurrentEnemy.SpeedUp(5f, 0.4f));
            //  CurrentEnemy.CurrentSpeed = CurrentEnemy.MaxSpeed;
            if (PathHolder.instance && PathHolder.instance.Positions.Count > 0)
            {
                CurrentEnemy.CurrentPath -= 1;
                CurrentEnemy.SetTarget(LevelPath.Positions[CurrentEnemy.CurrentPath]);
                /*
                if (CurrentEnemy.CurrentPath - 1 >= 0) // check if closer to previous path node
                {
                    float CurrentDistance = Vector3.Distance(CurrentEnemy.transform.position, PathHolder.instance.Positions[CurrentEnemy.CurrentPath]);
                    float PrevDistance = Vector3.Distance(CurrentEnemy.transform.position, PathHolder.instance.Positions[CurrentEnemy.CurrentPath - 1]);
                    if (PrevDistance < CurrentDistance)
                    {
                        CurrentEnemy.CurrentPath -= 1;
                    }
                }
              */

            }

        }
        CurrentlySuckedEnemies.Clear();
        //
    }

    public override void ShootEnemy()
    {

        //  WallHolder.transform.RotateAround(WallHolder.transform.position, Vector3.up, 1.0f * Time.fixedDeltaTime);
        
        if (CurrentTarget && CurrentlySuckedEnemies.Count < MaxSuckedEnemy && CurrentTarget.Targetable)
        {
            if (Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range || CurrentTarget.Targetable == false)
            {
                FindEnemy();
            }
            else
            {
                
                Debug.DrawRay(transform.position, (CurrentTarget.transform.position - transform.position), Color.green);
                CurrentTarget.Targetable = false;
                CurrentTarget.CurrentSpeed = 0;
                CurrentlySuckedEnemies.Add(CurrentTarget);
                SecondsTillDamage -= Time.deltaTime * 1.2f; // when add a new enemy, delay damage slightly
                if (CurrentlySuckedEnemies.Count >= MaxSuckedEnemy)
                {
                    Debug.Log("SUCKED MAX");
                }
                
            }
        }
        
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
