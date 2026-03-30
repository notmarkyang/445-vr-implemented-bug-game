using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityButton : MonoBehaviour
{
    [Header("UI 引用")]
    public Image loadingCircle;      // 拖入该按钮对应的加载圆圈
    public float activationTime = 1.0f; // 触发所需时间

    [Header("功能设置")]
    public UnityEvent onActivated;   // 进度满后执行的函数（比如换场景）

    private float timer = 0f;
    private bool isHandInside = false;

    void Start()
    {
        // 初始状态隐藏加载圈
        if (loadingCircle != null)
        {
            loadingCircle.gameObject.SetActive(false);
            loadingCircle.fillAmount = 0;
        }
    }

    void Update()
    {
        if (isHandInside)
        {
            timer += Time.deltaTime;

            if (loadingCircle != null)
            {
                loadingCircle.gameObject.SetActive(true);
                loadingCircle.fillAmount = timer / activationTime;
            }

            if (timer >= activationTime)
            {
                TriggerAbility();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand") || other.CompareTag("RightPoke"))
        {
            isHandInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand") || other.CompareTag("RightPoke"))
        {
            ResetButton();
        }
    }

    void TriggerAbility()
    {
        onActivated.Invoke(); 
        ResetButton();      
    }

    void ResetButton()
    {
        isHandInside = false;
        timer = 0f;
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0;
            loadingCircle.gameObject.SetActive(false);
        }
    }
}