using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreviewPanel : MonoBehaviour
{
    BattlePreviewText battlePreviewText;
    private string previewText;
    private void Awake()
    {
        battlePreviewText = GetComponentInChildren<BattlePreviewText>();
        previewText = battlePreviewText.GetComponent<Text>().text;
    }

    /// <summary>
    /// 设置战斗显示文字
    /// </summary>
    public void SetPreviewText(string text)
    {
        
        previewText = text;
    }

}
