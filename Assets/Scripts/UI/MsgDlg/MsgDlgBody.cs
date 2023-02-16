using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 一个按钮
/// </summary>
public class MsgDlgBody : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MsgDlgButtonInfo m_button;
    public MsgDlgButtonInfo M_button{get { return m_button;}}
    public static float bodyHeight = 82.5f;  // 框框的高度 策划设计的
    private int bodyIndex;  // 这是第几个body

    /// <summary>
    /// 设置命令按钮的命令文字
    /// </summary>
    public void SetMbutton(MsgDlgButtonInfo button, int index)
    {
        m_button = button;
        CommandText commandText = GetComponentInChildren<CommandText>();
        commandText.GetComponent<Text>().text = button.commandText;
        bodyIndex = index;
        GetComponent<RectTransform>().localPosition = new Vector2(0, -(index + 1) * bodyHeight);
    }

    /// <summary>
    /// 试试看unity按钮悬浮的api
    /// </summary>
    public int OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name + " GameObject");
        Debug.Log("bodyIndex: " + bodyIndex);    
        return bodyIndex;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Debug.Log("Cursor Exiting " + name + " GameObject");
    }

}
