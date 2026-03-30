using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRSelectEquipNotifier : MonoBehaviour
{
    [Header("要通知的手环菜单控制器")]
    [SerializeField] private WristMenuController wristMenuController;

    [Header("交互成功后要激活的物体（可选）")]
    [SerializeField] private GameObject objectToActivate;

    [Header("是否只触发一次")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private XRSimpleInteractable xrSimpleInteractable;
    private bool hasTriggered = false;

    private void Awake()
    {
        xrSimpleInteractable = GetComponent<XRSimpleInteractable>();

        if (xrSimpleInteractable != null)
        {
            xrSimpleInteractable.selectEntered.AddListener(OnSelected);
        }
        else
        {
            Debug.LogWarning("XRSelectEquipNotifier: 没找到 XRSimpleInteractable，脚本不会生效。", this);
        }
    }

    private void OnDestroy()
    {
        if (xrSimpleInteractable != null)
        {
            xrSimpleInteractable.selectEntered.RemoveListener(OnSelected);
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (wristMenuController != null)
        {
            wristMenuController.SetEquippedStatus(true);
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}