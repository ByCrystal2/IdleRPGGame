using System;
using System.Collections;
using UnityEngine;

public class PlayerSkillHandler : MonoBehaviour
{
    PlayerMotor player;
    [Header("MultiShot Skill Settings")]
    [SerializeField] float multiShotDuration = 5f; // Multishot yeteneginin surme suresi 
    [SerializeField] public int giantArrowDamageMultiplier = 5; // Giant Arrow yeteneginin hasar carpani
    [SerializeField] public int flamingArrowDamageMultiplier = 3; // Atesli ok yeteneginin hasar carpani
    [SerializeField] public float multiShotArrowDelay = 0.2f;
    private bool isMultiShotActive = false; 
    private bool isGiantArrowActive = false; 
    private bool isFlamingArrowActive = false; 


    private void Awake()
    {
        player = GetComponent<PlayerMotor>();
    }
    public void ActivateSkill(PlayerSkillType skillType)
    {
        if (!PlayerManager.Instance.IsThereAnEnemyNearby())
            return;

        Debug.Log($"Activating skill: {skillType}");
        switch (skillType)
        {
            case PlayerSkillType.None:
                break;
            case PlayerSkillType.Multishot:
                StartMultishot();
                break;
            case PlayerSkillType.GiantArrow:
                StartGiantArrow();
                break;
            case PlayerSkillType.FlamingArrow:
                StartFlamingArrow();
                break;
            default:
                break;
        }
    }

    

    public void MultiShotActivation(bool enable) => isMultiShotActive = enable;
    public void GiantArrowActivation(bool enable) => isGiantArrowActive = enable;
    public void FlamingArrowActivation(bool enable) => isFlamingArrowActive = enable;

    public bool IsMultiShotActive() => isMultiShotActive;
    public bool IsGiantArrowActive() => isGiantArrowActive;
    public bool IsFlamingArrowActive() => isFlamingArrowActive;
    public void StartMultishot()
    {
        StartCoroutine(IMuultishotTimer(multiShotDuration));
    }
    public void StartGiantArrow()
    {
        StartCoroutine(IGiantArrowTimer());
    }
    public void StartFlamingArrow()
    {
        StartCoroutine(IFlamingArrowTimer());
    }
    IEnumerator IMuultishotTimer(float duration)
    {
        MultiShotActivation(true);
        yield return new WaitForSeconds(duration);
        MultiShotActivation(false);
    }
    IEnumerator IGiantArrowTimer()
    {
        GiantArrowActivation(true);
        yield return null;
    }
    IEnumerator IFlamingArrowTimer()
    {
        FlamingArrowActivation(true);
        yield return null;
    }
}
public enum PlayerSkillType
{
    None,
    Multishot,
    GiantArrow,
    FlamingArrow
}
