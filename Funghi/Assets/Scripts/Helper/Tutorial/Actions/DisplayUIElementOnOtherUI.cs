using UnityEngine;

namespace Tutorials
{
    public class DisplayUIElementOnOtherUI : TutorialAction
    {
        public GameObject Prefab;
        public RectTransform other;
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

            if (_prefabInstance == null || other == null) return false;
            var rt = _prefabInstance.transform as RectTransform;
            if (!rt) return false;
            rt.anchoredPosition = other.anchoredPosition+Offset;
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