// File: Scripts/UI/PanelSwitcher.cs
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    [Header("From → To (どのパネルからどのパネルへ)")]
    public GameObject fromPanel;
    public GameObject toPanel;

    /// <summary>
    /// ボタンから呼び出す。指定されたパネル間で遷移する。
    /// </summary>
    public void SwitchPanel()
    {
        if (fromPanel != null)
        {
            fromPanel.SetActive(false);  // 理由：元のパネルを閉じる
        }

        if (toPanel != null)
        {
            toPanel.SetActive(true);     // 理由：遷移先パネルを表示する
        }
    }
}
