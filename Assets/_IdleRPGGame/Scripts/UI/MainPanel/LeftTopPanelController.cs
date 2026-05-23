using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftTopPanelController : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI txtHealthBar;

    [SerializeField] Slider levelBarSlider;
    [SerializeField] TextMeshProUGUI txtLevelBarHeader;
    [SerializeField] TextMeshProUGUI txtLevelBar;
    private void OnEnable()
    {
        EventBus<Health>.Subscribe(EventType.UpgradeCharacter, (x)=> OnHealthChanged(x));
        EventBus<PlayerStats>.Subscribe(EventType.ExpChange, (x)=> OnLevelExpChanged(x));
    }
    private void OnDisable()
    {
        EventBus<Health>.Unsubscribe(EventType.UpgradeCharacter, (x) => OnHealthChanged(x));
        EventBus<PlayerStats>.Unsubscribe(EventType.ExpChange, (x) => OnLevelExpChanged(x));
    }
    void OnHealthChanged(Health playerHealth)
    {
        txtHealthBar.text = $"{playerHealth.currentHealth}/{playerHealth.GetMaxHealth()}";
        healthBarSlider.value = (float)playerHealth.currentHealth / playerHealth.GetMaxHealth();
    }
    void OnLevelExpChanged(PlayerStats state)
    {
        if (state.level >= state.MaxLevel)
        {
            txtLevelBarHeader.text = $"Level: {state.level} (Max)";
            txtLevelBar.text = "MAX LEVEL";
            levelBarSlider.value = 1f;
        }
        else
        {
            int requiredXP = state.LevelUpStages[state.level];
            txtLevelBarHeader.text = $"Level: {state.level}";
            txtLevelBar.text = $"{state.currentXP} / {requiredXP}";
            levelBarSlider.value = (float)state.currentXP / requiredXP;
        }
    }
}
