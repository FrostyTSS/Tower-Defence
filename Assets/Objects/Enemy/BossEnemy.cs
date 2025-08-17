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
        Debug.Log("KILL ENEMY");
        //Destroy(gameObject);
        PathManager.Money += MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies


        StartCoroutine("BossEnemySpawn");
    }




    IEnumerator BossEnemySpawn()
    {
        for (int i = 0; i < EnemiesToSpawnOnDefeat.Count; i++)
        {
            PathManager.Enemies.Add(
                    Instantiate(EnemiesToSpawnOnDefeat[i], PathManager.StartPosition, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
            yield return new WaitForSeconds(0.25f);
        }
        // Code goes here!
    }

}
