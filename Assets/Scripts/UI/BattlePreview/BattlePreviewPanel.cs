using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreviewPanel : MonoBehaviour
{
    private Text _battlePreviewText;

    private void Awake()
    {
        BattlePreviewText pt = GetComponentInChildren<BattlePreviewText>();
        _battlePreviewText = pt ? pt.GetComponent<Text>() : null;
    }

    /// <summary>
    /// 设置战斗显示文字
    /// </summary>
    public void SetPreviewText(string text)
    {
        if (_battlePreviewText) _battlePreviewText.text = text;
    }

}
