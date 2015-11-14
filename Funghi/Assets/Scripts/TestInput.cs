using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TestInput : MonoBehaviour
{
    public LayerMask center;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Node"))
        {
            FungusNetwork.Instance.CreateNewNode(GetClickedTile().worldPosition);
        }

        if (Input.GetButtonDown("Click"))
        {
            FunCenter.Instance.MoveToNewTile(GetClickedTile());
        }


    }

    private bool HitCenter(Vector3 _mousePos)
    {
        bool hitCenter = false;
        _mousePos = new Vector3(_mousePos.x, _mousePos.y, Camera.main.transform.position.z);
        Ray ray = new Ray(_mousePos, new Vector3(0, 0, 20.0f));
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100.0f, center))
        {
            Debug.Log("Found Center");
            hitCenter = true;
        }

        return hitCenter;
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