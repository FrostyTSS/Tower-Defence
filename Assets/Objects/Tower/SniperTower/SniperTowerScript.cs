using UnityEngine;

public class SniperTowerScript : BaseTower
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
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
