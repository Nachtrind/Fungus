using UnityEngine;

namespace Tutorials
{
    public class DisplayUIElement : TutorialAction
    {
        public GameObject Prefab;
        public bool PositionOnSource;
        public Vector2 Offset;

        GameObject _prefabInstance;
        public override void OnInitialize(Tutorial t, float time)
        {
            if (Timeout > 0)
            {
                StartTime = time;
            }
            if (_prefabInstance == null && Prefab != null)
            {
                _prefabInstance = Instantiate(Prefab);
                _prefabInstance.transform.parent = t.UIAnchor;
            }
        }

        public override bool Execute(Tutorial t, float time)
        {
            if (Timeout > 0)
            {
                if (time - StartTime > Timeout)
                {
                    return false;
                }
            }

            if (_prefabInstance == null) return false;
            var rt = _prefabInstance.transform as RectTransform;
            if (!rt) return false;
            var pos = Vector2.zero;
            if (PositionOnSource)
            {
                if (CachedEventInfo.LinkedEntity != null)
                {
                    pos = Camera.main.WorldToScreenPoint(CachedEventInfo.LinkedEntity.transform.position);
                }
                if (CachedEventInfo.LinkedSpawner != null)
                {
                    pos = Camera.main.WorldToScreenPoint(CachedEventInfo.LinkedSpawner.transform.position);
                }
                pos = pos + Offset;
            }
            else
            {
                pos = Offset;
            }
            rt.anchoredPosition = pos;
            return true;
        }

        public override void OnCleanup(Tutorial t)
        {
            if (_prefabInstance != null)
            {
                Destroy(_prefabInstance);
            }
        }
    }
}