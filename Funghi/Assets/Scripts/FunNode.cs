using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FunNode : Fungus
{

    public float radius { get; set; }
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

    }

    void OnMouseDown()
    {

        foreach(List<Tile> slimeList in slimePaths)
        {

            
        }
    }


}
