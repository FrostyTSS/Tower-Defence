using UnityEngine;

public class PinTowerScript : BaseTower
{

    public int ProjectileAmount = 8;


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
                        ProjectileList.Add(Instantiate(ProjectileType, transform.position, Quaternion.Euler(ProjectileAngle)).GetComponent<ProjectileBase>());
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
        }
        else
        {
            FindEnemy();
        }
    }
}
