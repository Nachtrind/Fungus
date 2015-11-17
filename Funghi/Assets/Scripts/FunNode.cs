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
    private List<List<Tile>> slimePaths = new List<List<Tile>>();
    public List<List<Tile>> SlimePaths
    {
        get { return slimePaths; }
        set { slimePaths = value; }
    }

    public LayerMask enemyLayer;

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Node gets destroyed
        if (destroying && SlimePaths.Count <= 0)
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
        for (int i = SlimePaths.Count - 1; i >= 0; i--)
        {
            if (SlimePaths[i].Count > 0)
            {
                Tile toRemove = SlimePaths[i][SlimePaths[i].Count - 1];

                if (CenterOnTile(toRemove))
                {
                    SlimePaths.Remove(SlimePaths[i]);
                    continue;
                }

                if (toRemove.slime.usages == 1)
                {

                    SlimePaths[i].RemoveAt(SlimePaths[i].Count - 1);
                    toRemove.state = 0;
                    Destroy(toRemove.slime.gameObject);
                }
                else
                {
                    SlimePaths[i].RemoveAt(SlimePaths[i].Count - 1);
                    toRemove.slime.usages -= 1;
                }
            }
            if (SlimePaths[i].Count == 0)
            {
                SlimePaths.RemoveAt(i);
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