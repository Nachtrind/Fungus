using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FunNode : Fungus
{

    public float radius { get; set; }

    //Destroying Node + Slime Paths
    private float destroyTick = 0.8f;
    private float destroyTimer;
    private bool destroying = false;

    public Vector3 worldPos { get; set; }
    public List<List<Tile>> slimePaths { get; set; }

    void Awake()
    {
        List<List<Tile>> slimePath = new List<List<Tile>>();
    }

    // Use this for initialization
    void Start()
    {

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

    void OnMouseDown()
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


}
