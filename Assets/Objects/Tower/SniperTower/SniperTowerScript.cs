using System;
using System.Collections;
using UnityEngine;

public class SniperTowerScript : BaseTower
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    public bool AbilitySpinPause = false;
    public float RotateSpeed = 2.5f;
   // public float AbilityLength = 5f;
    public float AbilityDelay = 1f; // put on ability script
    //float Timer = 0;
   public float PauseTime = 2;
    public Vector3 InitalLinePos = Vector3.zero;
    public LineRenderer BulletTrail;
    //put time to spin in ability script
     IEnumerator AbilityShoot()
    {
        //pause for a second, raycast forward, if hit, shoot, pause a bit longer then keep spininng
        //  PinAbilityScript AbiRef = (PinAbilityScript)Ability;

        /*
        if (AbilityActive && AbilitySpinPause)
        {
            Timer += Time.fixedDeltaTime;
            //Debug.Log(Timer);

            transform.Rotate(new Vector3(0, RotateSpeed, 0), Space.World);
            if (Timer >= AbilityLength)
            {
                StopSpin();
            }

        }
        */

        float CurrentRot = 0;
        float MaxRot = 1800;
        DelayTimer = TimeBetweenShots / 3;
        int randomvalue = UnityEngine.Random.Range(0, 1);
        if (randomvalue == 1)
        {
            MaxRot += 360;
            RotateSpeed += 1;
        }
        
        if (Ability is SniperAbility sniperAbility) // if casting fails then 'skill is AttackSkill' returns false and nothing is assigned to attackSkill
        {
            MaxRot = sniperAbility.TotalRotAngles;
        }

        while (CurrentRot <= MaxRot)
        {
           
            // AbilityTimer += Time.fixedDeltaTime + AbilityDelay;
            DelayTimer -= Time.fixedDeltaTime;
            //AbilityTimer += Time.fixedDeltaTime;
            transform.Rotate(new Vector3(0, RotateSpeed, 0) * Time.fixedDeltaTime, Space.World); // do i do deltatime here...?
            CurrentRot += RotateSpeed * Time.fixedDeltaTime;
           
            if (DelayTimer <= 0) // double speed shot
            {
                AbilityShootAction();
                yield return new WaitForSeconds(AbilityDelay);
                DelayTimer = TimeBetweenShots / 3;
            }
           // DelayChecker();



                 yield return null;
        }





        /*
        float AbilityTimer = 0;
        //float DelayTimer = AbilityDelay;
        DelayTimer = TimeBetweenShots;
        while (AbilityTimer < AbilityLength)
        {
            Debug.Log(AbilityTimer);
           // AbilityTimer += Time.fixedDeltaTime + AbilityDelay;
            DelayTimer += Time.fixedDeltaTime;
            AbilityTimer += Time.fixedDeltaTime;
            transform.Rotate(new Vector3(0, RotateSpeed, 0), Space.World);
            //DelayTimer -= Time.fixedDeltaTime;
            //if (DelayTimer <= 0)
            // {
            //     DelayTimer = NewDelay;

            /*
            if (DelayTimer >= TimeBetweenShots / 2) // double speed shot
            {
                AbilityShootAction();
                DelayTimer = TimeBetweenShots;
            }
            */
        // yield return new WaitForSeconds(AbilityDelay);

        //yield return null;
        // }
        // }
        transform.Rotate(new Vector3(0, MaxRot, 0));
            yield return new WaitForSeconds(PauseTime);
        DelayTimer = TimeBetweenShots;
        AbilityActive = false;
    }

    public override void DelayChecker()
    {
        if (DelayTimer <=  1.25f && BulletTrail.GetPosition(1) != InitalLinePos && AbilityActive == false) // bang bang, hide flash trail
        {
            BulletTrail.SetPosition(1, InitalLinePos);
        }
    }

   public  void AbilityCaller()
    {
        AbilityActive = true;
        StartCoroutine(AbilityShoot());
    }

    public void AbilityShootAction()
    {
        RaycastHit hit;

        Debug.Log("Sniper Ability Test..");
        LayerMask layerMask = LayerMask.GetMask("Enemy");

        //line renderer, sound
        
        if (ProjectileSound && this.GetComponent<AudioSource>())
        {
            this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
        }
        //The cube will be 2 units wide (1 half in x), it will be 4 units high (2 half in y) and 6 units long (3 half in z).
        //BoxCast(pos, new Vector3(1,2,3), dir, Quaternion.LookRotation(dir));
        if (Physics.BoxCast(transform.position, new Vector3(1, 2, 3), transform.forward, out hit, Quaternion.LookRotation(transform.forward), 300, layerMask))
            {
            hit.transform.GetComponent<BaseEnemy>().TakeDamage(Damage, this);
            Debug.Log("Sniper Ability Bang!" + hit.point);
            if (BulletTrail)
            {
                BulletTrail.SetPosition(1, hit.transform.position);
            }

        }

        /*
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask   ))
        {
            hit.transform.GetComponent<BaseEnemy>().TakeDamage(Damage, this);
            if (BulletTrail)
            {
                BulletTrail.SetPosition(1, hit.transform.position);
            }
        }
        */
        else if (BulletTrail)
        {
            Debug.Log("Sniper Ability Miss.." + transform.forward * 200);
            Vector3 BulletPos = transform.forward * 200;
            BulletPos.y = transform.position.y + 0.15f;
            BulletTrail.SetPosition(1, BulletPos);
        }

    }

    private void Awake()
    {
        if (BulletTrail == null && this.GetComponent<LineRenderer>())
        {
            BulletTrail = this.GetComponent<LineRenderer>();
            InitalLinePos = transform.position; // add offset
            InitalLinePos.y += 0.15f;
            BulletTrail.SetPosition(0, InitalLinePos);
            BulletTrail.SetPosition(1, InitalLinePos);
        }
        else
        {
            BulletTrail.SetPosition(0, InitalLinePos);
            BulletTrail.SetPosition(1, InitalLinePos);
        }
    }

    /*
    public void AbilityActivate()
    {
        //AbilityActive = true;
        //InvokeRepeating("AbilityCaller", 0.5f, 0.5f);
    }
    */



    /*
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
    */
    public override void ShootEnemy()
    {
        //if laggy, move below under target.
        if (AbilityActive == false)
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

                    if (BulletTrail)
                    {
                        BulletTrail.SetPosition(1, CurrentTarget.transform.position);
                    }
                    if (ProjectileSound && this.GetComponent<AudioSource>())
                    {
                        this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
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
}
