using UnityEngine;
using System.Collections;
public class PinTowerScript : BaseTower
{

    public int ProjectileAmount = 8;
    public AudioClip ClumpedShot;

    public override void FindEnemy()
    {
        /*
        for (int i = 0; i < ProjectileList.Count; i++)
        {
            if (ProjectileList[i] != null)
            {
                Destroy(ProjectileList[i].gameObject);
            }
        }
        ProjectileList.Clear();
        */
      //  CleanupProjectiles();
        if (LevelPath.Enemies.Count > 0)
        {
            CurrentTarget = null;
           // Searching = true;


            //scan for closest cuz it never targets anyways
            LayerMask mask = LayerMask.GetMask("Enemy");
            //make sure its the right layer
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range, mask);
            foreach (var hitCollider in hitColliders)
            {
                //hitCollider.SendMessage("AddDamage");
                if (hitCollider.GetComponent<BaseEnemy>())
                {
                    CurrentTarget = hitCollider.GetComponent<BaseEnemy>();
                  //  Searching = false;
                    break;
                }
            }
        }
        //CurrentTarget = CurrentClosestEnemy;

        

    }

    public void PinShoot()
    {


        if (ProjectileSound && this.GetComponent<AudioSource>())
        {
            this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
        }

        //if ((ProjectileList.Count + 1) * Damage <= CurrentTarget.Health) // make sure you only use the amount of shots necessary to kill
        // {
        //get the angle of where the main projectile should be
        Quaternion RotatedForwardQ = Quaternion.LookRotation(CurrentTarget.transform.position - transform.position);
        Vector3 RotatedForward = RotatedForwardQ.eulerAngles;

        Vector3 ProjectileAngle = RotatedForward + new Vector3(0, 0, 0);
        for (int i = 0; i < ProjectileAmount; i++)
        {
            ProjectileAngle += new Vector3(0, 360 / ProjectileAmount, 0);
            // Debug.Log(ProjectileAngle);
            ProjectileList.Add(Instantiate(ProjectileType, transform.position, Quaternion.Euler(ProjectileAngle)).GetComponent<ProjectileBase>()); // fucked brackets
            ProjectileList[ProjectileList.Count - 1].ProjOwner = this;
            ProjectileList[ProjectileList.Count - 1].ProjTargetObj = CurrentTarget;
            //copied from splitproj
            ProjectileList[ProjectileList.Count - 1].Tracking = false;
            ProjectileList[ProjectileList.Count - 1].ExtraProjectileFailsafe();

        }
        //apply upgrades after all projs
        for (int j = 0; j < UpgradeList.Count; j++) // better then number ids maybe?
        {
            for (int l = 0; l < UpgradeList.Count; l++) // better then number ids maybe?
            {

                UpgradeList[j].Effects[l].OnFire(this);
            }
        }
        // }



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

    //the things you gotta do when you can't call coroutines from SOs...
    public void AbilityCaller()
    {
        StartCoroutine(AbilityLoop( ));
    }
    IEnumerator AbilityLoop()
    {
        PinAbilityScript AbiRef = (PinAbilityScript)Ability;
        

        float AbilityTimer = 0;
        float DelayTimer = AbiRef.NewDelay;
        while (AbilityTimer < AbiRef.TimeToLastFor)
        {
            Debug.Log(AbilityTimer);
            AbilityTimer += Time.fixedDeltaTime + AbiRef.NewDelay;
            //DelayTimer -= Time.fixedDeltaTime;
            //if (DelayTimer <= 0)
            // {
            //     DelayTimer = NewDelay;
            if (ClumpedShot)
            {
                PathHolder.instance.GetComponent<AudioSource>().PlayOneShot(ClumpedShot);
            }
            PinShoot(AbiRef.AbilityProjType, AbiRef.AbilityAmount);
                yield return new WaitForSeconds(AbiRef.NewDelay);
           // }
        }
        yield return null;
    }


    public void PinShoot(GameObject ProjectileTypeI, int AmountI) // for abilities
    {
        
        //if ((ProjectileList.Count + 1) * Damage <= CurrentTarget.Health) // make sure you only use the amount of shots necessary to kill
        // {
        //get the angle of where the main projectile should be
        Quaternion RotatedForwardQ = Quaternion.LookRotation(transform.position);
        Vector3 RotatedForward = RotatedForwardQ.eulerAngles;

        Vector3 ProjectileAngle = RotatedForward + new Vector3(0, 0, 0);
        for (int i = 0; i < AmountI; i++)
        {
            if (ProjectileSound && this.GetComponent<AudioSource>())
            {
                this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
            }
            ProjectileAngle += new Vector3(0, 360 / AmountI, 0);
            // Debug.Log(ProjectileAngle);
            ProjectileList.Add(Instantiate(ProjectileTypeI, transform.position, Quaternion.Euler(ProjectileAngle)).GetComponent<ProjectileBase>()); // fucked brackets
            ProjectileList[ProjectileList.Count - 1].ProjOwner = this;
            ProjectileList[ProjectileList.Count - 1].ProjTargetObj = CurrentTarget;
            //copied from splitproj
            ProjectileList[ProjectileList.Count - 1].Tracking = false;
            ProjectileList[ProjectileList.Count - 1].ExtraProjectileFailsafe();

        }
        



       
    }

    /*
    public void ShootEnemy(GameObject ProjectileTypeNew, float NewTimeBetweenShots, int Amount)
    {
        GameObject OriginalProj = ProjectileType;
        int OriginalAmount = ProjectileAmount;
        float OriginalTimeBetweenShots = TimeBetweenShots;

        ProjectileType = ProjectileTypeNew;
        TimeBetweenShots = NewTimeBetweenShots;
        ProjectileAmount = Amount;

        PinShoot();

        ProjectileType = OriginalProj;
        TimeBetweenShots = OriginalTimeBetweenShots;
        ProjectileAmount = OriginalAmount;

    }
    */
    public override void ShootEnemy()
    {
        if (CurrentTarget)
        {
            //CleanupProjectiles();
            //Debug.Log(Vector3.Distance(this.transform.position, CurrentTarget.transform.position));
            if (Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range)
            {
                FindEnemy();
            }
            else
            {
                PinShoot();
            }
        }
        else
        {
            FindEnemy();
        }
    }
}
