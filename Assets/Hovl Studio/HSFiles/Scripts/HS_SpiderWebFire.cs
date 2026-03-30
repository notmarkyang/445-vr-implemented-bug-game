using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_SpiderWebFire : MonoBehaviour
{
    public ParticleSystem fireEffect;
    public bool enableRepeating = true;
    public float repeatTime = 20;
    private bool canRepear = true;
    public bool useMouseHover = true;
    public Collider triggerCollider;

    // Material/dissolve settings
    public string dissolveParam = "_Dissolve";
    public float dissolveDuration = 1f;
    private Renderer webRenderer;
    private Material webMaterial;

    void Start()
    {
        // Try to find a MeshRenderer on this GameObject or its children
        webRenderer = GetComponent<MeshRenderer>();
        if (webRenderer == null)
            webRenderer = GetComponentInChildren<MeshRenderer>();

        if (webRenderer != null)
            webMaterial = webRenderer.material; // creates an instance so we can modify it at runtime
        else
            Debug.LogWarning("HS_SpiiderWebFire: No MeshRenderer found on " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        // If a specific triggerCollider is assigned, only react when that collider is the one entering.
        // Otherwise react to any collider that enters.
        if (triggerCollider != null)
        {
            if (other != triggerCollider)
                return;
        }

        TryActivate();
    }

    // mouse hover behaviour: invoke same action when mouse moves over the object
    // Requirements: this GameObject must have a Collider (or the collider must be on the same GameObject),
    // and the script must be enabled. OnMouseEnter is provided by Unity's legacy input system.
    void OnMouseEnter()
    {
        if (useMouseHover)
            TryActivate();
    }

    private void TryActivate()
    {
        if (!canRepear)
            return;

        canRepear = false;

        if (fireEffect != null)
            fireEffect.Play();

        // start dissolve if we have a material instance
        if (webMaterial != null)
            StartCoroutine(DissolveRoutine(1f, 0f, dissolveDuration));
        else
            Debug.LogWarning("HS_SpiiderWebFire: webMaterial is null, cannot animate dissolve.");

        if (enableRepeating)
            StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator DissolveRoutine(float from, float to, float duration)
    {
        float t = 0f;
        // ensure starting value
        webMaterial.SetFloat(dissolveParam, from);

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float value = Mathf.Lerp(from, to, t);
            webMaterial.SetFloat(dissolveParam, value);
            yield return null;
        }

        webMaterial.SetFloat(dissolveParam, to);
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(repeatTime);
        canRepear = true;
        webMaterial.SetFloat(dissolveParam, 1);
    }
}
