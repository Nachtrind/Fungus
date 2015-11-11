using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TestInput : MonoBehaviour
{

    Tile lastClicked;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Click"))
        {
            FungusNetwork.Instance.CreateNewNode(GetClickedTile().worldPosition);
        }

    }

    private Tile GetClickedTile()
    {
        return WorldGrid.Instance.TileFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lastClicked.worldPosition, 0.1f);

        if (path.Count > 0)
        {
            foreach (Vector3 v in path)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }

    }*/

}