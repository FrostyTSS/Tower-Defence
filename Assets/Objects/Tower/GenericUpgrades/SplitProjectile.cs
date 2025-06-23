using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Rendering.CameraUI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "SplitProjectile", menuName = "TowerInfo/Effects/SplitProjectileEffect", order = 1)]

public class SplitProjectile : UpgradeEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnFire(BaseTower TowerRef)
    {
        Debug.Log("on Fire working");
       if (TowerRef.ProjectileType)
        {
            Debug.Log("Buut projtype isnt");
            Quaternion RotatedForwardQ = Quaternion.LookRotation(TowerRef.CurrentTarget.transform.position - TowerRef.transform.position);
            Vector3 RotatedForward = RotatedForwardQ.eulerAngles;

            //left
            Vector3 LeftRot = RotatedForward + new Vector3(0, 5, 0);
            Vector3 RightRot = RotatedForward + new Vector3(0, -5, 0);
            SpawnSplitProj(LeftRot, TowerRef);
            SpawnSplitProj(RightRot, TowerRef);
          //  SpawnSplitProj(Quaternion.AngleAxis(45, TowerRef.transform.up) * TowerRef.transform.forward, TowerRef);

        }
    }

    void SpawnSplitProj(Vector3 Rotation, BaseTower TowerRef)
    {
        TowerRef.ProjectileList.Add(Instantiate(TowerRef.ProjectileType, TowerRef.transform.position, Quaternion.Euler(Rotation)).GetComponent<ProjectileBase>());
        TowerRef.ProjectileList[TowerRef.ProjectileList.Count - 1].ProjOwner = TowerRef;
        TowerRef.ProjectileList[TowerRef.ProjectileList.Count - 1].ProjTargetObj = TowerRef.CurrentTarget;
        TowerRef.ProjectileList[TowerRef.ProjectileList.Count - 1].Tracking = false;
        TowerRef.ProjectileList[TowerRef.ProjectileList.Count - 1].ExtraProjectileFailsafe();
       // TowerRef.ProjectileList[TowerRef.ProjectileList.Count - 1].transform.Rotate(new Vector3(0, Rotation, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
