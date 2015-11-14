using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FunNode : Fungus
{

    public float radius { get; set; }
    public float maxHealth;
    float currentHealth;

    //Destroying Node + Slime Paths
    private float destroyTick = 0.8f;
    private float destroyTimer;
    private bool destroying = false;

    public Vector3 worldPos { get; set; }
    public List<List<Tile>> slimePaths { get; set; }

    public LayerMask enemyLayer;

    void Awake()
    {
        List<List<Tile>> slimePath = new List<List<Tile>>();
    }

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Node gets destroyed
        if (destroying && slimePaths.Count <= 0)
        {
            Destroy(this.gameObject);
        }

        if (destroying && destroyTimer >= destroyTick)
        {
            DestroySlime();
            destroyTick = 0.0f;
        }
        else
        {
            destroyTimer += Time.deltaTime;
        }


    }

    void KillNode()
    {
        destroying = true;
        this.GetComponent<Renderer>().enabled = false;
        FungusNetwork.Instance.nodes.Remove(this);
        FungusNetwork.Instance.fungi.Remove(this);
    }

    public void DestroySlime()
    {
        for (int i = slimePaths.Count - 1; i >= 0; i--)
        {
            if (slimePaths[i].Count > 0)
            {
                Tile toRemove = slimePaths[i][slimePaths[i].Count - 1];

                if (CenterOnTile(toRemove))
                {
                    slimePaths.Remove(slimePaths[i]);
                    continue;
                }

                if (toRemove.slime.usages == 1)
                {

                    slimePaths[i].RemoveAt(slimePaths[i].Count - 1);
                    toRemove.state = 0;
                    Destroy(toRemove.slime.gameObject);
                }
                else
                {
                    slimePaths[i].RemoveAt(slimePaths[i].Count - 1);
                    toRemove.slime.usages -= 1;
                }
            }
            if (slimePaths[i].Count == 0)
            {
                slimePaths.RemoveAt(i);
            }
        }
    }


    public bool CenterOnTile(Tile _tileToCheck)
    {
        if (WorldGrid.Instance.TileFromWorldPoint(FunCenter.Instance.transform.position) == _tileToCheck)
        {
            return true;
        }

        return false;
    }


    public void Damage(float _damage)
    {
        this.currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            KillNode();
        }
    }


    public void NormalAttack()
    {
        List<Enemy> enemies = GetEnemiesInRadius();

        if (enemies.Count > 0)
        {
            enemies[0].GotAttacked();
        }

    }



    private List<Enemy> GetEnemiesInRadius()
    {
        List<Enemy> enemies = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(worldPos, radius, enemyLayer);

        foreach (Collider co in colliders)
        {
            enemies.Add(co.GetComponent<Enemy>());
        }

        return enemies;
    }
}