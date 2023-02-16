using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理按钮信息和文字 生成按钮
/// </summary>
public class MsgDlg : MonoBehaviour
{
    private MsgDlgBottom msgDlgBottom;
    private string bodyPrefabPath = "Prefabs/UI/MsgDlgBody";
    private List<MsgDlgBody> bodies = new List<MsgDlgBody>();  // 存放按钮body的list
    private int pointerCommandIndex;  // 鼠标选中的命令index

    /// <summary>
    /// 根据传入的按钮信息list 对应排列UI 数量和位置
    /// </summary>
    public void CreateMsgDlg(List<MsgDlgButtonInfo> commandButtons)
    {
        for (int i = bodies.Count; i < commandButtons.Count; i++)
        {
            CreateBody (commandButtons[i], bodyPrefabPath, i);
        }
        while (bodies.Count > commandButtons.Count)
        {
            DestroyBody(bodies[commandButtons.Count]);
            bodies.RemoveAt(commandButtons.Count);
        }
        for (int i = 0; i < commandButtons.Count; i++)
        {
            bodies[i].SetMbutton(commandButtons[i], i);
        }
        msgDlgBottom = gameObject.GetComponentInChildren<MsgDlgBottom>();  // 拿到小孩里的msgDlgBottom
        msgDlgBottom.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(0, -(commandButtons.Count + 1) * MsgDlgBody.bodyHeight);
    }
    // 02112023 下一步 找到可以执行的操作 生成传入List<MsgDlgButton>

    
    /// <summary>
    /// 生成对应的body
    /// </summary>
    private void CreateBody (MsgDlgButtonInfo button, string bodyPrefabPath, int index)
    {
        GameObject body = Instantiate<GameObject>(Resources.Load<GameObject>(bodyPrefabPath));
        MsgDlgBody msgDlgBody = body.GetComponent<MsgDlgBody>();
        if (msgDlgBody != null)
        {
            bodies.Add(msgDlgBody);
            msgDlgBody.SetMbutton(button,index);
            msgDlgBody.transform.SetParent(transform);  // 设置父级 树形展开 谁是枝 坐标会跟着父级动
        }       
    }

    private void DestroyBody(MsgDlgBody body)
    {
        GameObject.Destroy(body.gameObject);
    }

    /// <summary>
    /// 根据鼠标位置 返回对应命令index 如果鼠标不在UI范围内就返回nullIndex
    /// </summary>
    public void GetIndexByPoint(Vector2 pointerPosition)  
    {
        RectTransform rect = transform.GetComponent<RectTransform>();  // 我自己的rect
        if (
            pointerPosition.x < rect.position.x - rect.sizeDelta.x / 2 || 
            pointerPosition.x > rect.position.x + rect.sizeDelta.x / 2 || 
            pointerPosition.y < rect.position.y - rect.sizeDelta.y / 2 * bodies.Count|| 
            pointerPosition.y > rect.position.y + rect.sizeDelta.y / 2 
        )  // 光标没有在UI框范围内
        {
            pointerCommandIndex = Constants.nullCommandIndex;  // 没选中任何命令
            Debug.Log("pointerPosition : " + pointerPosition);
            Debug.Log("pointerCommandIndex : " + pointerCommandIndex);
            return;
        }
        else
        {
            pointerCommandIndex = (int)((pointerPosition.y - rect.position.y + rect.sizeDelta.y / 2) / rect.sizeDelta.y);
            Debug.Log("pointerPosition : " + pointerPosition);
            Debug.Log("pointerCommandIndex : " + pointerCommandIndex);
        }
        return;
    }
    
}
