using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<GameObject> moveRange = new List<GameObject>();
    private List<GameObject> attackRange = new List<GameObject>();

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
            AddAttackRange(showRange[i]);
        }
    }

    private void AddAttackRange(Vector2Int gridPos)
    {
        GameObject grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackRange"));
        if (grid == null) return;

        grid.transform.SetParent(transform);

        GridPosition gPos = grid.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = gridPos;
            gPos.SynchronizeGridPosition();
        }

        attackRange.Add(grid);
    }

    public void ClearAttackRange()
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

    public void ShowMoveRange(List<DijkstraMoveInfo> logicMoveRange)
    {
        ClearMoveRange();

        for (int i = 0; i < logicMoveRange.Count; i++)
        {
            AddMoveRange(logicMoveRange[i].position);
        }
    }

    public void AddMoveRange(Vector2Int gridPos)
    {
        GameObject grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MoveRange"));
        if (grid == null) return;

        grid.transform.SetParent(transform);

        GridPosition gPos = grid.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = gridPos;
            gPos.SynchronizeGridPosition();
        }

        moveRange.Add(grid);
    }

    public void ClearMoveRange()
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
}
