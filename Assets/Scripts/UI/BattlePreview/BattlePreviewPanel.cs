using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreviewPanel : MonoBehaviour
{
    /// <summary>
    /// 设置战斗显示文字
    /// </summary>
    public void SetPreviewText(string text)
    {
        BattlePreviewText battlePreviewText = GetComponentInChildren<BattlePreviewText>();
        battlePreviewText.GetComponent<Text>().text = text;
    }

}
