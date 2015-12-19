using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Generates a random Position between min and max radius to place the spawned human at")]
    public class RandomRangePosition : PositionModule
    {
        public float minRadius = 0f;
        public float maxRadius = 1f;
        public override void Apply(Entity e, ModuleWorker worker)
        {
            Vector2 rndPos = Random.insideUnitCircle;
            Vector3 newPosition = new Vector3(Mathf.Clamp(rndPos.x, minRadius, maxRadius), 0, Mathf.Clamp(rndPos.y, minRadius, maxRadius));
            e.transform.position += newPosition;
            worker.ProcessNext(e);
        }

        Color minRColor = new Color(0, 0.5f, 1f, 0.33f);
        Color maxRColor = new Color(0, 1f, 1f, 0.33f);
        public override void DrawGizmos(Vector3 center)
        {
            Gizmos.color = minRColor;
            Gizmos.DrawWireSphere(center, minRadius);
            Gizmos.color = maxRColor;
            Gizmos.DrawWireSphere(center, maxRadius);
        }
    }
}