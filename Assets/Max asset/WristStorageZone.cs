using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WristStorageZone : MonoBehaviour
{
    [Header("Normal Storable Tags")]
    public string cardTag = "Card";
    public string screwdriverTag = "Screwdriver";
    public string flashlightTag = "Flashlight";

    [Header("Giant Shrinkable Tags")]
    public string giantCardTag = "GiantCard";
    public string giantScrewdriverTag = "GiantScrewdriver";
    public string giantFlashlightTag = "GiantFlashlight";

    [Header("Objects To Show / Hide")]
    public GameObject storageBoxRoot;
    public GameObject detectorHintUI;
    public TMP_Text detectorHintText;

    [Header("Hint Text")]
    public string defaultHintMessage = "Place item into storage box";
    public string giantHintMessage = "Giant item detected. Scan to shrink.";
    public string scanNoResultMessage = "No valid item found.";
    public string scanFoundNormalMessage = "Item detected.";
    public string scanFoundGiantMessage = "Giant item detected. Ready to shrink.";
    public string shrinkSuccessMessage = "Item shrunk. Pick it up and store it.";

    [Header("Scan Settings")]
    public Transform scanOrigin;
    public float scanRadius = 3f;
    public LayerMask scanLayerMask = ~0;

    [Header("Shrink Settings")]
    public Transform shrinkSpawnPoint;
    public float giantShrinkScaleMultiplier = 0.2f;

    [Header("Visual Guidance")]
    public LineRenderer giantTargetLine;
    public GameObject giantActionUI;

    [Header("Gate Control")]
    public bool requireScanBeforeStorage = true;
    public float storageArmedDuration = 8f;

    private bool storageArmed = false;
    private Coroutine storageArmedRoutine;

    [HideInInspector] public bool isStorageInProgress = false;

    private readonly HashSet<GameObject> zoneRelevantObjects = new HashSet<GameObject>();
    private readonly List<Behaviour> outlinedComponents = new List<Behaviour>();

    private GameObject currentGiantTarget;

    private void Start()
    {
        HideStorageBox();
        HideDetectorHint();
        HideGiantActionUI();
        ClearTargetLine();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject rootObject = GetRootObject(other);
        if (rootObject == null)
            return;

        if (!IsRelevantTag(rootObject.tag))
            return;

        zoneRelevantObjects.Add(rootObject);

        if (requireScanBeforeStorage && !storageArmed)
            return;

        RefreshZoneState();
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject rootObject = GetRootObject(other);
        if (rootObject == null)
            return;

        if (!IsRelevantTag(rootObject.tag))
            return;

        zoneRelevantObjects.Remove(rootObject);

        if (!isStorageInProgress)
        {
            RefreshZoneState();
        }
    }

    private GameObject GetRootObject(Collider other)
    {
        if (other == null)
            return null;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
            return rb.gameObject;

        return other.transform.root.gameObject;
    }

    public bool IsNormalStorableTag(string tagToCheck)
    {
        return tagToCheck == cardTag ||
               tagToCheck == screwdriverTag ||
               tagToCheck == flashlightTag;
    }

    public bool IsGiantTag(string tagToCheck)
    {
        return tagToCheck == giantCardTag ||
               tagToCheck == giantScrewdriverTag ||
               tagToCheck == giantFlashlightTag;
    }

    public bool IsRelevantTag(string tagToCheck)
    {
        return IsNormalStorableTag(tagToCheck) || IsGiantTag(tagToCheck);
    }

    private void RefreshZoneState()
    {
        CleanupZoneObjects();

        if (requireScanBeforeStorage && !storageArmed)
        {
            HideStorageBox();
            return;
        }

        if (zoneRelevantObjects.Count > 0)
        {
            ShowStorageBox();

            if (ContainsAnyGiantInZone())
                ShowDetectorHint(giantHintMessage);
            else
                ShowDetectorHint(defaultHintMessage);
        }
        else
        {
            HideDetectorHint();
            HideStorageBox();
        }
    }

    private void CleanupZoneObjects()
    {
        zoneRelevantObjects.RemoveWhere(obj => obj == null);
    }

    private bool ContainsAnyGiantInZone()
    {
        foreach (GameObject obj in zoneRelevantObjects)
        {
            if (obj != null && IsGiantTag(obj.tag))
                return true;
        }
        return false;
    }

    public void ShowStorageBox()
    {
        if (storageBoxRoot != null)
            storageBoxRoot.SetActive(true);
    }

    public void HideStorageBox()
    {
        if (storageBoxRoot != null)
            storageBoxRoot.SetActive(false);
    }

    public void ShowDetectorHint(string message)
    {
        if (detectorHintUI != null)
            detectorHintUI.SetActive(true);

        if (detectorHintText != null)
            detectorHintText.text = message;
    }

    public void HideDetectorHint()
    {
        if (detectorHintUI != null)
            detectorHintUI.SetActive(false);
    }

    public void NotifyStorageStarted()
    {
        isStorageInProgress = true;
        HideDetectorHint();
    }

    public void NotifyStorageFinished()
    {
        isStorageInProgress = false;
        storageArmed = false;

        HideDetectorHint();
        HideStorageBox();
        HideGiantActionUI();
        ClearTargetLine();
        ClearAllOutlines();

        if (storageArmedRoutine != null)
        {
            StopCoroutine(storageArmedRoutine);
            storageArmedRoutine = null;
        }

        RefreshZoneState();
    }

    // ========= SCAN PULSE =========

    public void TriggerScanPulse()
    {
        ClearAllOutlines();
        currentGiantTarget = null;
        HideGiantActionUI();
        ClearTargetLine();

        Vector3 origin = scanOrigin != null ? scanOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, scanRadius, scanLayerMask);

        GameObject nearestGiant = null;
        float nearestGiantDistance = float.MaxValue;

        bool foundAnyRelevant = false;

        foreach (Collider hit in hits)
        {
            GameObject rootObject = GetRootObject(hit);
            if (rootObject == null)
                continue;

            if (!IsRelevantTag(rootObject.tag))
                continue;

            foundAnyRelevant = true;
            TryEnableOutline(rootObject);

            if (IsGiantTag(rootObject.tag))
            {
                float distance = Vector3.Distance(origin, rootObject.transform.position);
                if (distance < nearestGiantDistance)
                {
                    nearestGiantDistance = distance;
                    nearestGiant = rootObject;
                }
            }
        }

        if (!foundAnyRelevant)
        {
            ShowDetectorHint(scanNoResultMessage);
            return;
        }
        ArmStorageGate();

        if (nearestGiant != null)
        {
            currentGiantTarget = nearestGiant;
            ShowDetectorHint(scanFoundGiantMessage);
            ShowGiantActionUI();
            UpdateTargetLine(currentGiantTarget);
        }
        else
        {
            ShowDetectorHint(scanFoundNormalMessage);
        }
    }

    private void ArmStorageGate()
    {
        storageArmed = true;

        if (storageArmedRoutine != null)
            StopCoroutine(storageArmedRoutine);

        storageArmedRoutine = StartCoroutine(StorageArmedCountdown());
    }

    private IEnumerator StorageArmedCountdown()
    {
        yield return new WaitForSeconds(storageArmedDuration);

        if (!isStorageInProgress)
        {
            storageArmed = false;
            HideStorageBox();
        }

        storageArmedRoutine = null;
    }

    public void ConfirmShrinkCurrentGiant()
    {
        if (currentGiantTarget == null)
        {
            ShowDetectorHint("No giant item selected.");
            HideGiantActionUI();
            ClearTargetLine();
            return;
        }

        string newTag = ConvertGiantTagToNormalTag(currentGiantTarget.tag);

        if (string.IsNullOrEmpty(newTag))
        {
            ShowDetectorHint("Target tag conversion failed.");
            HideGiantActionUI();
            ClearTargetLine();
            return;
        }

        currentGiantTarget.transform.localScale *= giantShrinkScaleMultiplier;
        currentGiantTarget.tag = newTag;

        if (shrinkSpawnPoint != null)
        {
            currentGiantTarget.transform.position = shrinkSpawnPoint.position;
            currentGiantTarget.transform.rotation = shrinkSpawnPoint.rotation;
        }

        Rigidbody rb = currentGiantTarget.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider[] allColliders = currentGiantTarget.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in allColliders)
        {
            col.enabled = true;
        }

        var allGrabComponents =
            currentGiantTarget.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(true);
        foreach (var grab in allGrabComponents)
        {
            grab.enabled = true;
        }

        TryEnableOutline(currentGiantTarget);

        ShowDetectorHint(shrinkSuccessMessage);
        HideGiantActionUI();
        ClearTargetLine();

        if (!zoneRelevantObjects.Contains(currentGiantTarget))
            zoneRelevantObjects.Add(currentGiantTarget);

        currentGiantTarget = null;
        RefreshZoneState();
    }

    private string ConvertGiantTagToNormalTag(string giantTag)
    {
        if (giantTag == giantCardTag) return cardTag;
        if (giantTag == giantScrewdriverTag) return screwdriverTag;
        if (giantTag == giantFlashlightTag) return flashlightTag;
        return string.Empty;
    }

    // ========= OUTLINE =========

    private void TryEnableOutline(GameObject target)
    {
        if (target == null)
            return;

        Behaviour[] allBehaviours = target.GetComponentsInChildren<Behaviour>(true);
        foreach (Behaviour behaviour in allBehaviours)
        {
            if (behaviour == null)
                continue;

            if (behaviour.GetType().Name == "Outline")
            {
                behaviour.enabled = true;

                if (!outlinedComponents.Contains(behaviour))
                    outlinedComponents.Add(behaviour);
            }
        }
    }

    public void ClearAllOutlines()
    {
        for (int i = 0; i < outlinedComponents.Count; i++)
        {
            if (outlinedComponents[i] != null)
                outlinedComponents[i].enabled = false;
        }

        outlinedComponents.Clear();
    }

    // ========= LINE =========

    private void Update()
    {
        if (currentGiantTarget != null)
        {
            UpdateTargetLine(currentGiantTarget);
        }
    }

    private void UpdateTargetLine(GameObject target)
    {
        if (giantTargetLine == null || target == null)
            return;

        giantTargetLine.enabled = true;
        giantTargetLine.positionCount = 2;

        Vector3 startPos = scanOrigin != null ? scanOrigin.position : transform.position;
        Vector3 endPos = target.transform.position;

        giantTargetLine.SetPosition(0, startPos);
        giantTargetLine.SetPosition(1, endPos);
    }

    private void ClearTargetLine()
    {
        if (giantTargetLine != null)
        {
            giantTargetLine.positionCount = 0;
            giantTargetLine.enabled = false;
        }
    }

    private void ShowGiantActionUI()
    {
        if (giantActionUI != null)
            giantActionUI.SetActive(true);
    }

    private void HideGiantActionUI()
    {
        if (giantActionUI != null)
            giantActionUI.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = scanOrigin != null ? scanOrigin.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, scanRadius);
    }
}