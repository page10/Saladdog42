using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<GameObject> moveRange = new List<GameObject>();
    private List<GameObject> attackRange = new List<GameObject>();
    private List<GameObject> attackableMoveRange = new List<GameObject>();

    private List<Vector2Int> attackHealRange = new List<Vector2Int>();  // 攻击范围和移动范围都覆盖的范围

    /// <summary>
    /// 对于各类单位类型 格子的可触及范围
    /// </summary>
    List<CoveredRange> logicCoverRange = new List<CoveredRange>();
    private MsgDlg msgDlg;
    private Transform msgCanvasTransform; 
    private string msgdlgPath = "Prefabs/UI/Msgdlg";

    private BattlePreviewPanel battlePreviewPanel;
    private string battlePreviewPanelPath = "Prefabs/UI/BattlePreviewPanel";

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
    
    /// <summary>
    /// 显示移动、攻击、治疗范围
    /// </summary>
    /// <param name="characterAttack">攻击能力</param>
    /// <param name="moveRange">移动逻辑范围</param>
    /// <param name="mapSize">地图范围</param>
    /// <param name="occupiedGrids">被占领的格子</param>
    public void ShowAllRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize, List<Vector2Int> occupiedGrids)
    {
        ClearRange(SignType.Move);
        attackHealRange.Clear();
        
        ShowAttackRange(characterAttack, moveRange, mapSize, occupiedGrids);
        ShowMoveRange(moveRange, occupiedGrids);
    }
    

    private void ShowAttackRange(CharacterAttack characterAttack, List<Vector2Int> moveRange, Vector2Int mapSize, List<Vector2Int> occupiedGrids)
    {
        GetCoveredRange(characterAttack, moveRange, mapSize);
        List<Vector2Int> attackShowRange = new List<Vector2Int>();
        
        // 优先显示治疗范围
        List<Vector2Int> healShowRange = new List<Vector2Int>();
        for (int i = 0; i < logicCoverRange.Count; i++)
        {
            //Debug.Log("logicCoverRange[i].targetType: " + logicCoverRange[i].targetType);
            if ((Constants.TargetType_Ally & logicCoverRange[i].targetType) == Constants.TargetType_Ally)
            {
                healShowRange.Add(logicCoverRange[i].gridPos);
            }
        }
        
        // 显示攻击范围
        for (int i = 0; i < logicCoverRange.Count; i++)
        {
            if ((Constants.TargetType_Foe & logicCoverRange[i].targetType) == Constants.TargetType_Foe)
            {
                if (healShowRange.Contains(logicCoverRange[i].gridPos))
                {
                    if (!attackHealRange.Contains(logicCoverRange[i].gridPos))
                    {
                        attackHealRange.Add(logicCoverRange[i].gridPos);
                    }
                }
                else
                {
                    attackShowRange.Add(logicCoverRange[i].gridPos);
                }
            }
        }
        
        // 裁剪治疗范围
        for (int i = 0; i < attackHealRange.Count; i++)
        {
            if (healShowRange.Contains(attackHealRange[i]))
            {
                healShowRange.Remove(attackHealRange[i]);
            }
        }
        // 裁剪移动范围中被占据的格子
        for (int i = 0; i < occupiedGrids.Count; i++)
        {
            if (moveRange.Contains(occupiedGrids[i]))
            {
                moveRange.Remove(occupiedGrids[i]);
            }
        }
        // 裁剪可攻击治疗对于移动的范围
        for (int i = 0; i < moveRange.Count; i++)
        {
            if (attackHealRange.Contains(moveRange[i]))
            {
                attackHealRange.Remove(moveRange[i]);
            }
        }
        
        
        // 显示
        for (int i = 0; i < healShowRange.Count; i++)  
        {
            AddRange(SignType.Heal, healShowRange[i]);
        }
        for (int i = 0; i < attackShowRange.Count; i++)  
        {
            AddRange(SignType.Attack, attackShowRange[i]);
        }
        
        for (int i = 0; i < attackHealRange.Count; i++)  
        {
            AddRange(SignType.AttackHeal, attackHealRange[i]);
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


    private void ShowMoveRange(List<Vector2Int> logicMoveRange, List<Vector2Int> occupiedGrids)
    {
        for (int i = 0; i < logicMoveRange.Count; i++)
        {
            if (occupiedGrids.Contains(logicMoveRange[i]))
            {
                continue;  // 被占领的格子不加入移动范围
            }
            AddRange(SignType.Move, logicMoveRange[i]);
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
                    if (!grid) return;
                }
                break;
            case SignType.Attack:  // 攻击范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackRange"));
                    if (!grid) return;
                }
                break;
            case SignType.AttackableMove:  // 可移动并攻击的范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackableMoveRange"));
                    if (!grid) return;
                }
                break;
            case SignType.Heal:  // 治疗范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/HealRange"));
                    if (!grid) return;
                }
                break;
            case SignType.AttackHeal:  // 攻击和治疗范围
                {
                    grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AttackHealRange"));
                    if (!grid) return;
                }
                break;
        }

        grid.transform.SetParent(transform);

        GridPosition gPos = grid.GetComponent<GridPosition>();
        if (gPos)
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
            case SignType.AttackHeal:  // 攻击和治疗范围
                {
                    attackRange.Add(grid);
                }
                break;
            case SignType.Heal:  // 治疗范围
                {
                    attackRange.Add(grid);
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
        attackHealRange.Clear();
    }

    public void ShowMsgDlg(List<MsgDlgButtonInfo> commandButtons)
    {
        if (!msgDlg)
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
    
    public void ShowBattlePreviewPanel(List<BattleResInfo> previewResult)
    {
        string previewShowText = "";
        foreach (BattleResInfo battleRes in previewResult)
        {
            previewShowText += (battleRes.attacker + "攻击" + battleRes.defender + "造成" + battleRes.damage + "点伤害" + "\n");
        }
        
        if (!battlePreviewPanel)
        {
            GameObject battlePreviewPanelObject = Instantiate<GameObject>(Resources.Load<GameObject>(battlePreviewPanelPath));
            battlePreviewPanelObject.transform.SetParent(msgCanvasTransform);  // transform set parent 了之后 gameobject也就setParent了 
            battlePreviewPanelObject.transform.localPosition = new Vector3(-225, -200, 0);  // set一下localPosition 
            battlePreviewPanelObject.transform.localScale = Vector3.one;
            battlePreviewPanel = battlePreviewPanelObject.GetComponent<BattlePreviewPanel>();
            battlePreviewPanelObject.SetActive(true);
            battlePreviewPanel.SetPreviewText(previewShowText);
        }
        else
        {
            battlePreviewPanel.gameObject.SetActive(true);
            battlePreviewPanel.SetPreviewText(previewShowText);
        }
    }
    
    public void HideBattlePreviewPanel()
    {
        if (battlePreviewPanel)
        {
            battlePreviewPanel.gameObject.SetActive(false);
        }
        
    }


}


