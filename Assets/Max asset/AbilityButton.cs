using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityButton : MonoBehaviour
{
    [Header("UI 引用")]
    public Image loadingCircle;      
    public float activationTime = 1.0f; 

    [Header("功能设置")]
    public UnityEvent onActivated;

    private float timer = 0f;
    private bool isHandInside = false;

    void Start()
    {
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