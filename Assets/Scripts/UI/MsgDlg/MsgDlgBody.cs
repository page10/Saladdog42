using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 一个按钮
/// </summary>
public class MsgDlgBody : MonoBehaviour
{
    private MsgDlgButtonInfo m_button;
    public MsgDlgButtonInfo M_button{get { return m_button;}}
    public static float bodyHeight = 82.5f;  // 框框的高度 策划设计的

    /// <summary>
    /// 设置命令按钮的命令文字
    /// </summary>
    public void SetMbutton(MsgDlgButtonInfo button, int index)
    {
        m_button = button;
        CommandText commandText = GetComponentInChildren<CommandText>();
        commandText.GetComponent<Text>().text = button.commandText;
        GetComponent<RectTransform>().localPosition = new Vector2(0, -(index + 1) * bodyHeight);
    }

}
