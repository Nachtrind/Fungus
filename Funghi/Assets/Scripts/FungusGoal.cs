using UnityEngine;
using NodeAbilities;

public class FungusGoal : MonoBehaviour
{
    public NodeAbility unlockedAbility;
    public string nextLevelName = "";

    void Update()
    {
        if (GameWorld.Instance.Core)
        {
            if (Vector3.Distance(transform.position, GameWorld.Instance.Core.transform.position) < 0.2f)
            {
                GameWorld.Instance.OnCoreTouchedGoal(this);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1f, 0, 0.33f);
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

}
