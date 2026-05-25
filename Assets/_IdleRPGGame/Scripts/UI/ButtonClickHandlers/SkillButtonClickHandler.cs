using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PlayerSkillType skillType;
    PlayerSkillHandler handler;
    ButtonSkillCooldown btnSkillCooldown;
    public bool isClickable;
    private void Awake()
    {
        handler = FindFirstObjectByType<PlayerSkillHandler>();
        btnSkillCooldown = GetComponent<ButtonSkillCooldown>();
        isClickable = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isClickable) return;
        Debug.Log($"Skill button clicked: {skillType}");
        handler.ActivateSkill(skillType);

        if(PlayerManager.Instance.IsThereAnEnemyNearby())
        btnSkillCooldown.StartCoolDown();
    }
}
