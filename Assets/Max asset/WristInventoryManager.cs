using UnityEngine;

public class WristInventoryManager : MonoBehaviour
{
    public static WristInventoryManager Instance;

    [Header("Inventory State")]
    public bool hasCard = false;
    public bool hasScrewdriver = false;
    public bool hasFlashlight = false;

    [Header("Item Prefabs")]
    public GameObject cardPrefab;
    public GameObject screwdriverPrefab;
    public GameObject flashlightPrefab;

    [Header("Spawn Point")]
    public Transform itemSpawnPoint;

    [Header("UI Image Objects (show when stored)")]
    public GameObject cardImageObject;
    public GameObject screwdriverImageObject;
    public GameObject flashlightImageObject;

    [Header("Optional: prevent duplicate spawned item")]
    public bool despawnOldBeforeSpawn = true;

    private GameObject currentCardInstance;
    private GameObject currentScrewdriverInstance;
    private GameObject currentFlashlightInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateInventoryUI();
    }

    // ========== STORE ==========
    public void StoreItemByTag(string itemTag)
    {
        switch (itemTag)
        {
            case "Card":
                hasCard = true;
                Debug.Log("Stored: Card");
                break;

            case "Screwdriver":
                hasScrewdriver = true;
                Debug.Log("Stored: Screwdriver");
                break;

            case "Flashlight":
                hasFlashlight = true;
                Debug.Log("Stored: Flashlight");
                break;

            default:
                Debug.LogWarning("Unknown item tag: " + itemTag);
                return;
        }

        UpdateInventoryUI();
    }

    public bool HasItemByTag(string itemTag)
    {
        switch (itemTag)
        {
            case "Card":
                return hasCard;

            case "Screwdriver":
                return hasScrewdriver;

            case "Flashlight":
                return hasFlashlight;

            default:
                return false;
        }
    }

    // ========== UI ==========
    public void UpdateInventoryUI()
    {
        if (cardImageObject != null)
            cardImageObject.SetActive(hasCard);

        if (screwdriverImageObject != null)
            screwdriverImageObject.SetActive(hasScrewdriver);

        if (flashlightImageObject != null)
            flashlightImageObject.SetActive(hasFlashlight);
    }

    // ========== SPAWN ==========
    public void SpawnCard()
    {
        if (!hasCard)
        {
            Debug.Log("Card not stored yet.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogWarning("Card prefab is not assigned.");
            return;
        }

        if (itemSpawnPoint == null)
        {
            Debug.LogWarning("Item spawn point is not assigned.");
            return;
        }

        if (despawnOldBeforeSpawn && currentCardInstance != null)
        {
            Destroy(currentCardInstance);
        }

        currentCardInstance = Instantiate(cardPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
        Debug.Log("Spawned: Card");
    }

    public void SpawnScrewdriver()
    {
        if (!hasScrewdriver)
        {
            Debug.Log("Screwdriver not stored yet.");
            return;
        }

        if (screwdriverPrefab == null)
        {
            Debug.LogWarning("Screwdriver prefab is not assigned.");
            return;
        }

        if (itemSpawnPoint == null)
        {
            Debug.LogWarning("Item spawn point is not assigned.");
            return;
        }

        if (despawnOldBeforeSpawn && currentScrewdriverInstance != null)
        {
            Destroy(currentScrewdriverInstance);
        }

        currentScrewdriverInstance = Instantiate(screwdriverPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
        Debug.Log("Spawned: Screwdriver");
    }

    public void SpawnFlashlight()
    {
        if (!hasFlashlight)
        {
            Debug.Log("Flashlight not stored yet.");
            return;
        }

        if (flashlightPrefab == null)
        {
            Debug.LogWarning("Flashlight prefab is not assigned.");
            return;
        }

        if (itemSpawnPoint == null)
        {
            Debug.LogWarning("Item spawn point is not assigned.");
            return;
        }

        if (despawnOldBeforeSpawn && currentFlashlightInstance != null)
        {
            Destroy(currentFlashlightInstance);
        }

        currentFlashlightInstance = Instantiate(flashlightPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
        Debug.Log("Spawned: Flashlight");
    }

    // 可选：通用生成接口
    public void SpawnItemByTag(string itemTag)
    {
        switch (itemTag)
        {
            case "Card":
                SpawnCard();
                break;

            case "Screwdriver":
                SpawnScrewdriver();
                break;

            case "Flashlight":
                SpawnFlashlight();
                break;

            default:
                Debug.LogWarning("Unknown item tag: " + itemTag);
                break;
        }
    }

    // 可选：场景切换后重新指定新的生成点
    public void SetSpawnPoint(Transform newSpawnPoint)
    {
        itemSpawnPoint = newSpawnPoint;
    }
}