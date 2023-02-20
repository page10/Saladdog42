using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理按钮信息和文字 生成按钮
/// </summary>
public class MsgDlg : MonoBehaviour
{
    private MsgDlgBottom msgDlgBottom;
    private MsgdlgBodiesContainer msgdlgBodiesContainer;  // 存放bodies的container 解决一下显示层级问题
    private string bodyPrefabPath = "Prefabs/UI/MsgDlgBody";
    private List<MsgDlgBody> bodies = new List<MsgDlgBody>();  // 存放按钮body的list
    //private int pointerCommandIndex;  // 鼠标选中的命令index 现在用不到了
    private SelectSign selectSign;  // 选中命令高亮框

    private void Awake() {
        selectSign = GetComponentInChildren<SelectSign>();
        msgdlgBodiesContainer = GetComponentInChildren<MsgdlgBodiesContainer>();
    }

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
            bodies[i].SetMbutton(commandButtons[i], i, BodyOnSelected);
        }
        msgDlgBottom = gameObject.GetComponentInChildren<MsgDlgBottom>();  // 拿到小孩里的msgDlgBottom
        msgDlgBottom.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(0, -(commandButtons.Count + 1) * MsgDlgBody.bodyHeight);
    }

    
    /// <summary>
    /// 生成对应的body
    /// </summary>
    private void CreateBody (MsgDlgButtonInfo button, string bodyPrefabPath, int index)
    {
        GameObject body = Instantiate<GameObject>(Resources.Load<GameObject>(bodyPrefabPath));
        MsgDlgBody msgDlgBody = body.GetComponent<MsgDlgBody>();
        if (!msgDlgBody)
        {
            bodies.Add(msgDlgBody);
            msgDlgBody.SetMbutton(button,index,BodyOnSelected);
            msgDlgBody.transform.SetParent(msgdlgBodiesContainer.transform);  // 设置父级 树形展开 谁是枝 坐标会跟着父级动
        }       
    }

    private void DestroyBody(MsgDlgBody body)
    {
        GameObject.Destroy(body.gameObject);
    }

    private void BodyOnSelected (int idx) {
        selectSign.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(0, -(idx + 1) * MsgDlgBody.bodyHeight);
    }

  
    
}
