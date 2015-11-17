using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{

    public GameObject preEnemy;

    //Tmer
    public float enemyTick;
    float enemyTimer = 0.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyTimer >= enemyTick)
        {
            Debug.Log("Tick");
            GameObject enemy = Instantiate(preEnemy, this.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
            enemyTimer = 0.0f;
        }

        enemyTimer += Time.deltaTime;
    }
}
