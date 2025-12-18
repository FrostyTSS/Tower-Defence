using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : BaseEnemy
{

    public List<GameObject> EnemiesToSpawnOnDefeat;

    public override void KillEnemy()
    {

        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;

            Debug.Log("CHANGE COLOUR");
            main.startColor = this.GetComponent<MeshRenderer>().material.color;
            GetComponent<ParticleSystem>().Play();
            // can't  do this via the lower method. Why? Who knows.
            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
        }
        // StartCoroutine("BossEnemySpawn");
        BossEnemySpawn();
        Debug.Log("KILL ENEMY");
        //Destroy(gameObject);
        PathManager.Money += MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies


      
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            BossEnemySpawn();
        }
    }

    void BossEnemySpawn()
    {
        for (int i = 0; i < EnemiesToSpawnOnDefeat.Count; i++)
        {//why did i have it at start pos and not this pos?
            PathManager.Enemies.Add(
                    Instantiate(EnemiesToSpawnOnDefeat[i], transform.position, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
            PathManager.Enemies[PathManager.Enemies.Count - 1].CurrentPath = CurrentPath;
        }
       }

    /*
    IEnumerator BossEnemySpawn()
    {
        for (int i = 0; i < EnemiesToSpawnOnDefeat.Count; i++)
        {//why did i have it at start pos and not this pos?
            PathManager.Enemies.Add(
                    Instantiate(EnemiesToSpawnOnDefeat[i], transform.position, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
            PathManager.Enemies[PathManager.Enemies.Count - 1].CurrentPath = CurrentPath;
            yield return new WaitForSeconds(0.25f);
        }
     
    }
    */

}
