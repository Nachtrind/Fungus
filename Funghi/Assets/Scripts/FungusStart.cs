using UnityEngine;
using System.Collections;

public class FungusStart : MonoBehaviour
{
    public FungusNode fungusNodePrefab;
    public FungusCore fungusCenterPrefab;

    public AfterSpawnBehaviour afterSpawn = AfterSpawnBehaviour.Destroy;

    public enum AfterSpawnBehaviour { Destroy, Disable, Nothing }

    public void Spawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        Instantiate(fungusNodePrefab, transform.position, Quaternion.identity);
        yield return null; //Time for the node to fully register
        Instantiate(fungusCenterPrefab, transform.position, Quaternion.identity);
        switch (afterSpawn)
        {
            case AfterSpawnBehaviour.Destroy:
                Destroy(gameObject);
                break;
            case AfterSpawnBehaviour.Disable:
                gameObject.SetActive(false);
                break;
            case AfterSpawnBehaviour.Nothing:
                break;
        }
    }

    Color gizmoColor = new Color(0.25f, 1, 0f, 0.5f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
