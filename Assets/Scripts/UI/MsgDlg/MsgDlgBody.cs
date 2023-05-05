using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// 一个按钮
/// </summary>
public class MsgDlgBody : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private MsgDlgButtonInfo m_button;
    public MsgDlgButtonInfo M_button{get { return m_button;}}
    public static float bodyHeight = 82.5f;  // 框框的高度 策划设计的
    private int bodyIndex;  // 这是第几个body

    private MsgdlgBodyIndex OnSelected;
    private CommandEvent buttonCommand;

    private Text commandText;

    private void Awake()
    {
        commandText = GetComponentInChildren<CommandText>().GetComponent<Text>();
    }

    /// <summary>
    /// 设置命令按钮的命令文字
    /// </summary>
    public void SetMbutton(MsgDlgButtonInfo button, int index, MsgdlgBodyIndex onSelected)
    {
        m_button = button;
        commandText.text = button.commandText;
        bodyIndex = index;
        OnSelected = onSelected;
        buttonCommand = button.commandEvent;
        transform.localPosition = new Vector2(0, -(index + 1) * bodyHeight);
    }

    /// <summary>
    /// 试试看unity按钮悬浮的api
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
 
        if (OnSelected != null)
        {
            OnSelected(bodyIndex);
        } 
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
//        Debug.Log("Cursor Exiting " + name + " GameObject");
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if ((pointerEventData.button == PointerEventData.InputButton.Left) && buttonCommand != null)
        {
            buttonCommand(m_button.parameters);
        }
    }
}
