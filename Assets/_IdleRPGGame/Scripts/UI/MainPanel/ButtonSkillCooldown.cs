using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSkillCooldown : MonoBehaviour
{
    [Header("UI Elemanlari")]
    [SerializeField] private GameObject cooldownObj;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TMP_Text cooldownText;  
    private SkillButtonClickHandler m_skillHandlerButton;
    Button m_button;
    [Header("Yetenek Ayarları")]
    [SerializeField] private float cooldownTime = 5f;
    private float currentCooldownTimer = 0f;
    private bool isCooldown = false;

    private void Start()
    {
        m_skillHandlerButton = GetComponent<SkillButtonClickHandler>();
        m_button = GetComponent<Button>();
        cooldownImage.fillAmount = 0f;
        cooldownText.text = "";
        cooldownObj.SetActive(false);
    }
    private void Update()
    {
        if (isCooldown)
        {
            ApplyCooldown();
        }
    }
    public void StartCoolDown()
    {
        isCooldown = true;
        currentCooldownTimer = cooldownTime;
        m_skillHandlerButton.isClickable = false;
        m_button.interactable = false;
        cooldownObj.SetActive(true);
    }
    private void ApplyCooldown()
    {
        currentCooldownTimer -= Time.deltaTime;

        if (currentCooldownTimer <= 0f)
        {
            isCooldown = false;
            currentCooldownTimer = 0f;

            cooldownImage.fillAmount = 0f;
            cooldownText.text = "";
            m_skillHandlerButton.isClickable = true;
            m_button.interactable = true;
            cooldownObj.SetActive(false);
        }
        else
        {
           cooldownImage.fillAmount = currentCooldownTimer / cooldownTime;
           cooldownText.text = Mathf.CeilToInt(currentCooldownTimer).ToString();            
        }
    }
}