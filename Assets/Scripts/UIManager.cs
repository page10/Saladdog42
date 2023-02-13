using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<GameObject> moveRange = new List<GameObject>();
    private List<GameObject> attackRange = new List<GameObject>();
    private List<GameObject> attackableMoveRange = new List<GameObject>();
    private MsgDlg msgDlg;

    private void Awake() {
        msgDlg = new MsgDlg();
        msgDlg.gameObject.SetActive(false);  // 一开始先把它隐藏掉
    }

    ///<summary>
    /// 根据选中的角色和对应移动范围，得到攻击范围
    ///</summary>
    private List<Vector2Int> GetAttackRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize)
    {
        if (moveRange.Count == 0) return new List<Vector2Int>();
        GridPosition currGridPos = characterAttack.gameObject.GetComponent<GridPosition>();

        List<Vector2Int> atkRange = new List<Vector2Int>();
        for (int i = 0; i < moveRange.Count; i++)
        {
            List<Vector2Int> thisGridAtkRange = characterAttack.GetAttackRange(moveRange[i], mapSize, true);
            for (int j = 0; j < thisGridAtkRange.Count; j++)
            {
                if (!atkRange.Contains(thisGridAtkRange[j]))
                {
                    atkRange.Add(thisGridAtkRange[j]);
                }
            }
        }
        atkRange.Remove(currGridPos.grid);  // 去掉我自己

        // for (int i = 0; i < atkRange.Count; i++)
        // {
        //     Debug.Log(" atkRange " + i + ":  " +atkRange[i]);  
        // }

        return atkRange;

    }

    public void ShowAttackRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize)
    {

        List<Vector2Int> showRange = GetAttackRange(characterAttack, moveRange, mapSize);
        for (int i = 0; i < moveRange.Count; i++)
        {
            if (showRange.Contains(moveRange[i]))
            {
                showRange.Remove(moveRange[i]);
            }
        }

        for (int i = 0; i < showRange.Count; i++)
        {
            AddRange(SignType.Attack, showRange[i]);
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
        GameObject grid = new GameObject();
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
        msgDlg.gameObject.SetActive(true);
        msgDlg.CreateMsgDlg(commandButtons);
    }

    public void HideMsgDlg()
    {
        msgDlg.gameObject.SetActive(false);
    }
}


