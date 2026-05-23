using System;
using UnityEngine;
using UnityEngine.UI;

public class LeftBottomPanelController : MonoBehaviour
{
    [SerializeField] Button upgradeButton;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }

    private void OnUpgradeButtonClicked()
    {
        UIManager.Instance.ShowUpgradePanel();
    }
}
