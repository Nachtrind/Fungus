using UnityEngine;
using System.Collections;

public class RangeIndicator : MonoBehaviour
{

    [SerializeField]
    FungusNode node;
    [SerializeField]
    Renderer rend;
    Material material;
    int healthProperty;
    int ringProperty;
    Coroutine activeInterpolation;
    [SerializeField, Range(0, 1f)]
    float inactiveStrength = 0.1f;
    [SerializeField, Range(0, 1f)]
    float activeStrength = 0.75f;

    float currentRange;

    void Start()
    {
        if (!rend)
        {
            rend = GetComponent<Renderer>();
        }
        if (!node)
        {
            node = GetComponentInParent<FungusNode>();
        }
        if (!node || !rend) { Destroy(gameObject); }
        SetupNodeConnection();
        SetupMaterial();
        SetupRange();
        SetupHP();
        SetupActive();
    }

    void SetupNodeConnection()
    {
        node.OnHealthChanged += SetHP;
        node.OnToggleActive += SetActive;
    }

    void SetupMaterial()
    {
        material = rend.material;
        rend.material = material;
        healthProperty = Shader.PropertyToID("_HP");
        ringProperty = Shader.PropertyToID("_ColorMargin");
    }

    void SetupRange()
    {
        Vector3 scaleCompensation = node.transform.localScale;
        scaleCompensation = new Vector3(1f / scaleCompensation.x, 1f / scaleCompensation.y, 1f / scaleCompensation.z);
        currentRange = node.GetAbilityRange();
        transform.localScale = (currentRange + 0.1f) * scaleCompensation;
    }

    void SetupHP()
    {
        SetHP(100, 100);
    }

    void SetupActive()
    {
        Color c = material.GetColor(ringProperty);
        material.SetColor(ringProperty, new Color(c.r, c.g, c.b, inactiveStrength));
    }

    void SetActive(bool state)
    {
        if (activeInterpolation != null) { StopCoroutine(activeInterpolation); }
        activeInterpolation = StartCoroutine(InterpolateActiveState(state, 8f));
    }

    void SetHP(float currentHP, float maxHP)
    {
        float normalizedHP = Mathf.Clamp01(currentHP / maxHP);
        if (material)
        {
            material.SetFloat(healthProperty, normalizedHP);
        }
    }

    IEnumerator InterpolateActiveState(bool newState, float speed)
    {
        if (!material) { yield break; }
        Color c = material.GetColor(ringProperty);
        float newValue = newState ? activeStrength : inactiveStrength;
        float oldValue = c.a;
        float t = 0f;
        while (t < 1f)
        {
            c.a = Mathf.Lerp(oldValue, newValue, Mathf.SmoothStep(0, 1f, t));
            material.SetColor(ringProperty, c);
            t += Time.deltaTime * speed;
            yield return null;
        }
        activeInterpolation = null;
    }

    void OnDestroy()
    {
        if (node)
        {
            node.OnHealthChanged -= SetHP;
            node.OnToggleActive -= SetActive;
        }
        if (material)
        {
            material.SetFloat(healthProperty, 0f);
        }
    }
}
