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


}


