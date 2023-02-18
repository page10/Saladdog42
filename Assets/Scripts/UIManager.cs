using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<GameObject> moveRange = new List<GameObject>();
    private List<GameObject> attackRange = new List<GameObject>();
    private List<GameObject> attackableMoveRange = new List<GameObject>();

    /// <summary>
    /// 对于各类单位类型 格子的可触及范围
    /// </summary>
    List<CoveredRange> logicCoverRange = new List<CoveredRange>();
    private MsgDlg msgDlg;
    private Transform msgCanvasTransform; 
    private string msgdlgPath = "Prefabs/UI/Msgdlg";

    private void Awake() {
       msgCanvasTransform = GameObject.Find("Canvas").transform;

    }

    // ///<summary>
    // /// 根据选中的角色和对应移动范围，得到操作类型和对应的操作范围
    // ///</summary>
    // // 02172023 把这个函数重构以下 原本返回的List V2Int改成 List<敌人类型和V2int的struct> 
    // // 估计之后还得对应改显示相关的函数
    // private List<CoveredRange>[] GetAttackRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize)
    // {
    //     //if (moveRange.Count == 0) return new List<CoveredRange>();
    //     GridPosition currGridPos = characterAttack.gameObject.GetComponent<GridPosition>();

    //     List<CoveredRange>[] coverRange = new List<CoveredRange>[2];
    //     List<CoveredRange> atkRange = new List<CoveredRange>();
    //     for (int i = 0; i < moveRange.Count; i++)
    //     {
    //         List<Vector2Int> thisGridAtkRange = characterAttack.GetAttackRange(moveRange[i], mapSize, Constants.TargetType_Foe);
    //         for (int j = 0; j < thisGridAtkRange.Count; j++)
    //         {
    //             CoveredRange currRange = new CoveredRange(Constants.TargetType_Foe,thisGridAtkRange[j]);
    //             if (!atkRange.Contains(currRange) && thisGridAtkRange[j] != currGridPos.grid)
    //             {
    //                 atkRange.Add(currRange);
    //             }
    //         }
    //     }
    //     //atkRange.Remove(currGridPos.grid);  // 去掉我自己

    //     List<CoveredRange> healRange = new List<CoveredRange>();
    //     for (int i = 0; i < moveRange.Count; i++)
    //     {
    //         List<Vector2Int> thisGridhealRange = characterAttack.GetAttackRange(moveRange[i], mapSize, Constants.TargetType_Ally);
    //         for (int j = 0; j < thisGridhealRange.Count; j++)
    //         {
    //             CoveredRange currhealRange = new CoveredRange(Constants.TargetType_Ally,thisGridhealRange[j]);
    //             if (!healRange.Contains(currhealRange))
    //             {
    //                 healRange.Add(currhealRange);
    //             }
    //         }
    //     }

    //     List<CoveredRange> selfRange = new List<CoveredRange>();
    //     CoveredRange currSelfRange = new CoveredRange(Constants.TargetType_Ally,currGridPos.grid);



    //     return coverRange;

    // }

    private void GetCoveredRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize)
    {
        logicCoverRange.Clear();
        GridPosition currGridPos = characterAttack.gameObject.GetComponent<GridPosition>();
        for (int i = 0; i < moveRange.Count; i++)
        {
            List<CoveredRange> currRange = characterAttack.GetWeaponRange(moveRange[i], mapSize);
            for (int j = 0; j < currRange.Count; j++)
            {
                if (!logicCoverRange.Contains(currRange[j]))
                {
                    logicCoverRange.Add(currRange[j]);
                }
            }
        }       
    }

    public void ShowAttackRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize)
    {

        List<Vector2Int> attackShowRange = new List<Vector2Int>();
        for (int i = 0; i < logicCoverRange.Count; i++)
        {
            if ((Constants.TargetType_Foe & logicCoverRange[i].targetType) == Constants.TargetType_Foe)
            {
                attackShowRange.Add(logicCoverRange[i].gridPos);
            }
        }

        for (int i = 0; i < moveRange.Count; i++)
        {
            if (attackShowRange.Contains(moveRange[i]))
            {
                attackShowRange.Remove(moveRange[i]);
            }
        }

        for (int i = 0; i < attackShowRange.Count; i++)
        {
            AddRange(SignType.Attack, attackShowRange[i]);
        }
    }

    ///<summary>
    ///直接传入攻击范围，在地图上显示
    ///</summary>
    public void ShowAttackableMoveRange(List<Vector2Int> atkRange)
    {

        for (int i = 0; i < atkRange.Count; i++)
        {
            AddRange(SignType.AttackableMove, atkRange[i]);
        }
    }


    public void ShowMoveRange(List<DijkstraMoveInfo> logicMoveRange)
    {
        ClearRange(SignType.Move);

        for (int i = 0; i < logicMoveRange.Count; i++)
        {
            AddRange(SignType.Move, logicMoveRange[i].position);
        }
    }



    public void AddRange(SignType signType, Vector2Int gridPos)
    {
        GameObject grid = null;
        switch (signType)
        {
            case SignType.Move:  // 移动范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MoveRange"));
                    if (grid == null) return;
                }
                break;
            case SignType.Attack:  // 攻击范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackRange"));
                    if (grid == null) return;
                }
                break;
            case SignType.AttackableMove:  // 可移动并攻击的范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackableMoveRange"));
                    if (grid == null) return;
                }
                break;
        }

        grid.transform.SetParent(transform);

        GridPosition gPos = grid.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = gridPos;
            gPos.SynchronizeGridPosition();
        }

        switch (signType)
        {
            case SignType.Move:  // 移动范围
                {
                    moveRange.Add(grid);
                }
                break;
            case SignType.Attack:  // 攻击范围
                {
                    attackRange.Add(grid);
                }
                break;
            case SignType.AttackableMove:  // 可移动并攻击的范围
                {
                    attackableMoveRange.Add(grid);
                }
                break;
        }

    }

    /// <summary>
    /// 根据传入的参数 清空对应的范围显示
    /// </summary>
    public void ClearRange(SignType signType)
    {
        switch (signType)
        {
            case SignType.Move:  // 移动范围
                {
                    if (moveRange.Count > 0)
                    {
                        foreach (GameObject gameRange in moveRange)
                        {
                            Destroy(gameRange);
                        }
                        moveRange.Clear();
                    }
                }
                break;
            case SignType.Attack:  // 攻击范围
                {
                    if (attackRange.Count > 0)
                    {
                        foreach (GameObject gameRange in attackRange)
                        {
                            Destroy(gameRange);
                        }
                        attackRange.Clear();
                    }
                }
                break;
            case SignType.AttackableMove:  // 可移动并攻击的范围
                {
                    if (attackableMoveRange.Count > 0)
                    {
                        foreach (GameObject gameRange in attackableMoveRange)
                        {
                            Destroy(gameRange);
                        }
                        attackableMoveRange.Clear();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 清空所有的范围显示
    /// </summary>
    public void ClearAllRange()
    {
        if (moveRange.Count > 0)
        {
            foreach (GameObject gameRange in moveRange)
            {
                Destroy(gameRange);
            }
            moveRange.Clear();
        }
        if (attackRange.Count > 0)
        {
            foreach (GameObject gameRange in attackRange)
            {
                Destroy(gameRange);
            }
            attackRange.Clear();
        }
        if (attackableMoveRange.Count > 0)
        {
            foreach (GameObject gameRange in attackableMoveRange)
            {
                Destroy(gameRange);
            }
            attackableMoveRange.Clear();
        }
    }

    public void ShowMsgDlg(List<MsgDlgButtonInfo> commandButtons)
    {
        if (msgDlg == null)
        {
            GameObject msgDlgObject = Instantiate<GameObject>(Resources.Load<GameObject>(msgdlgPath));
            msgDlgObject.transform.SetParent(msgCanvasTransform);  // transform set parent 了之后 gameobject也就setParent了 
            msgDlgObject.transform.localPosition = new Vector3(180,180,0);  // set一下localPosition 不然 就会在左下角生成
            msgDlgObject.transform.localScale = Vector3.one;
            msgDlg = msgDlgObject.GetComponent<MsgDlg>();
            msgDlgObject.SetActive(true);
            msgDlg.CreateMsgDlg(commandButtons);
        }
        else
        {
            msgDlg.gameObject.SetActive(true);
            msgDlg.CreateMsgDlg(commandButtons);
        }

    }

    public void HideMsgDlg()
    {
        msgDlg.gameObject.SetActive(false);
    }

    // /// <summary>
    // /// 调用msgdlg 根据鼠标位置 返回对应命令index 如果鼠标不在UI范围内就返回nullIndex
    // /// </summary>
    // public void GetIndexByPoint(Vector2 pointerPosition)
    // {
    //     msgDlg.GetIndexByPoint(pointerPosition);
    // }
}


