using SingletonSystem;
using UnityEngine;

public class UIManager : PersistentSinleton<UIManager>
{
    [SerializeField] GameObject pnlUpgrade;

    public void ShowUpgradePanel()
    {
        pnlUpgrade.SetActive(true);
    }
}
