using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WristStorageBox : MonoBehaviour
{
    [Header("Accepted Tags")]
    public string cardTag = "Card";
    public string screwdriverTag = "Screwdriver";
    public string flashlightTag = "Flashlight";

    [Header("Storage Settings")]
    public Transform snapPoint;
    public float storeDuration = 1.2f;
    public float successMessageDuration = 1f;

    [Header("References")]
    public WristStorageZone outerZone;
    public GameObject boxUI;
    public TMP_Text statusText;
    public Image progressBar;

    private bool isStoring = false;
    private GameObject currentItem;
    private string currentItemTag;
    private Coroutine storageRoutine;

    private void Start()
    {
        ShowIdleUI();
        ResetProgressBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStoring)
            return;

        GameObject rootObject = GetRootObject(other);
        if (rootObject == null)
            return;

        if (!IsStorableTag(rootObject.tag))
            return;

        if (WristInventoryManager.Instance == null)
        {
            Debug.LogWarning("WristInventoryManager instance not found.");
            return;
        }

        if (WristInventoryManager.Instance.HasItemByTag(rootObject.tag))
        {
            ShowTemporaryMessage("Already Stored");
            return;
        }

        StartStorage(rootObject);
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

    private bool IsStorableTag(string tagToCheck)
    {
        return tagToCheck == cardTag ||
               tagToCheck == screwdriverTag ||
               tagToCheck == flashlightTag;
    }

    private void StartStorage(GameObject item)
    {
        if (item == null)
            return;

        currentItem = item;
        currentItemTag = item.tag;
        isStoring = true;

        if (outerZone != null)
            outerZone.NotifyStorageStarted();

        SnapItemToPoint(currentItem);

        if (storageRoutine != null)
            StopCoroutine(storageRoutine);

        storageRoutine = StartCoroutine(StorageRoutine());
    }

    private void SnapItemToPoint(GameObject item)
    {
        if (item == null || snapPoint == null)
            return;

        item.transform.position = snapPoint.position;
        item.transform.rotation = snapPoint.rotation;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] allColliders = item.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }

        var allGrabComponents =
            item.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(true);
        foreach (var grab in allGrabComponents)
        {
            grab.enabled = false;
        }
    }

    private IEnumerator StorageRoutine()
    {
        ShowStoringUI();
        ResetProgressBar();

        float timer = 0f;

        while (timer < storeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (progressBar != null)
                progressBar.fillAmount = Mathf.Clamp01(timer / storeDuration);

            yield return null;
        }

        CompleteStorage();
    }

    private void CompleteStorage()
    {
        if (WristInventoryManager.Instance != null)
        {
            WristInventoryManager.Instance.StoreItemByTag(currentItemTag);
        }

        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        StartCoroutine(CompleteAndResetRoutine());
    }

    private IEnumerator CompleteAndResetRoutine()
    {
        ShowStoredUI();

        if (progressBar != null)
            progressBar.fillAmount = 1f;

        yield return new WaitForSecondsRealtime(successMessageDuration);

        currentItem = null;
        currentItemTag = "";
        isStoring = false;

        ShowIdleUI();
        ResetProgressBar();

        if (outerZone != null)
            outerZone.NotifyStorageFinished();
    }

    private void ResetProgressBar()
    {
        if (progressBar != null)
            progressBar.fillAmount = 0f;
    }

    private void ShowIdleUI()
    {
        if (boxUI != null)
            boxUI.SetActive(true);

        if (statusText != null)
            statusText.text = "Place item here";
    }

    private void ShowStoringUI()
    {
        if (boxUI != null)
            boxUI.SetActive(true);

        if (statusText != null)
            statusText.text = "Storing...";
    }

    private void ShowStoredUI()
    {
        if (boxUI != null)
            boxUI.SetActive(true);

        if (statusText != null)
            statusText.text = "Stored";
    }

    private void ShowTemporaryMessage(string message)
    {
        StartCoroutine(TemporaryMessageRoutine(message));
    }

    private IEnumerator TemporaryMessageRoutine(string message)
    {
        if (boxUI != null)
            boxUI.SetActive(true);

        if (statusText != null)
            statusText.text = message;

        ResetProgressBar();

        yield return new WaitForSecondsRealtime(0.8f);

        ShowIdleUI();
    }
}