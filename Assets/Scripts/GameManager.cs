using System.Net.Http.Headers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MovementManager movementManager;
    private Camera currentCamera;

    private int currentPlayerIndex = 0;

    private int playerCount = 2;
    private List<CharacterObject>[] characters;  // all rabbits(characters)
    private CharacterObject selectedCharacter;  // selected character
    private GameObject currEnemy; //当前选中的敌人
    private MapGenerator mapGenerator;
    private UIManager uiManager;
    private Vector2 currMousePosition;  // 当前鼠标位置


    // 之后要被重构掉的东西
    private bool statusChain = false;  // 状态链
    List<MsgDlgButtonInfo> msgDlgButtonInfos = new List<MsgDlgButtonInfo>();  
    private Vector2Int lastPosition;  // 上一个状态时候的character位置


    private void Awake()
    {
        currentCamera = GameObject.Find("Camera").GetComponent<Camera>();
        movementManager = GetComponent<MovementManager>();
        mapGenerator = GetComponent<MapGenerator>();
        uiManager = GetComponent<UIManager>();
        StartGame(2);
        //GameState.gameControlState = GameControlState.SelectCharacter;
        
        //debug用 之后删了
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("attack", ()=>{Debug.Log("attack");}));
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("exchange", ()=>{Debug.Log("exchange");}));
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("skip", ()=>{Debug.Log("skip");}));
    }

    public int waitTick = 0;
    private void FixedUpdate()
    {

        switch (GameState.gameControlState)
        {
            case GameControlState.NewTurn:
                {
                    if (currentPlayerIndex != 0)
                    {
                        ChangeGameState(GameControlState.EndTurn);
                        return;
                    }
                    uiManager.ClearAllRange();
                    //重置可移动角色的移动状态    
                    //这里直接把所有角色的可移动状态重置吧 反正玩家也操作不了敌人队伍
                    for (int i = 0; i < playerCount; i++)
                    {
                        for (int j = 0; j < characters[i].Count; j++)
                        {
                            characters[i][j].animator.NewTurn();
                        }
                    }

                    if (currentPlayerIndex == 0)
                    {
                        ChangeGameState(GameControlState.SelectCharacter);
                    }

                }
                break;
            case GameControlState.SelectCharacter:
                {
                    if (Input.GetMouseButton(0) && waitTick <= 0)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
                    {
                        Vector2Int selectedGridPos = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition)
                            );
                        SelectedCharacterInfo currentCharacter = GetCharacterInSelectedGrid(selectedGridPos);
                        if (currentCharacter.playerIndex == currentPlayerIndex)  // 选中了我方角色
                        {
                            selectedCharacter = characters[currentCharacter.playerIndex][currentCharacter.characterIndex];
                            if (characters[currentPlayerIndex][currentCharacter.characterIndex].animator.IsMoveFinished(false) == true)  // 移动完了
                            {
                                Debug.Log("test");
                            }
                            else // 还没移动
                            {
                                movementManager.GetMoveRange(selectedCharacter.gameObject.GetComponent<CharacterMovement>(), GetOccupiedGrids(currentCharacter), GetAllyGrids(currentCharacter));
                                uiManager.ShowMoveRange(movementManager.LogicMoveRange);
                                uiManager.ShowAttackRange(selectedCharacter.gameObject.GetComponent<CharacterAttack>(), MovementManager.GetV2IntFromDijkstraRange(movementManager.LogicMoveRange), mapGenerator.mapSize);  //拿到攻击范围V2Int List
                                ChangeGameState(GameControlState.ShowRange);

                                waitTick = 10;
                            }

                        }
                        else if (currentCharacter.playerIndex == Constants.nullPlayerIndex)  // 选到了空格子
                        {

                        }
                        else  // 选中了敌方角色
                        {
                            
                        }

                    }
                }
                break;
            case GameControlState.ShowRange:
                {
                    if (Input.GetMouseButton(0) && waitTick <= 0)
                    {
                        Vector2Int currSelectGrid = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition));
                        SelectedCharacterInfo currentCharacter = GetCharacterInSelectedGrid(currSelectGrid);

                        Vector2Int characterGrid = selectedCharacter.gameObject.GetComponent<GridPosition>().grid;  // 自己那格
                        //Debug.Log("currSelectGrid , characterGrid" + currSelectGrid + "  " + characterGrid);
                        if (currentCharacter.playerIndex == 0 && currSelectGrid != characterGrid)
                        {
                            //选中了我方角色 除了自己
                            Debug.Log("selected ally characters");

                        }
                        else if (currentCharacter.playerIndex != Constants.nullPlayerIndex && currSelectGrid != characterGrid)
                        {
                            //选中了敌方角色
                            Debug.Log("selected enemy characters");
                            SelectedCharacterInfo currentEnemy = GetCharacterInSelectedGrid(currSelectGrid);
                            currEnemy = characters[currentCharacter.playerIndex][currentCharacter.characterIndex].gameObject;
                            ChangeGameState(GameControlState.ShowAttackableArea);
                        }
                        else
                        {
                            //没选中我方或敌方角色 包括自己脚下
                            Debug.Log("selected no character");
                            List<Vector2Int> moveGrids = movementManager.GetMovePath(
                                currSelectGrid  //以后在这要提出来改一下 
                                );
                            if (moveGrids.Count > 0)
                            {
                                uiManager.ClearAllRange();
                                for (int i = 0; i < moveGrids.Count; i++)
                                {
                                    //Debug.Log("[" + i + "]" + moveGrids[i]);
                                    uiManager.AddRange(SignType.Move, moveGrids[i]);
                                }

                                if (selectedCharacter.gameObject == null)
                                {
                                    ChangeGameState(GameControlState.SelectCharacter);
                                    waitTick = 10;
                                    return;
                                }
                                AnimatorController animatorController = selectedCharacter.gameObject.GetComponent<AnimatorController>();
                                if (animatorController != null)
                                {
                                    lastPosition = selectedCharacter.gPos.grid;  // 把这个位置更新了
                                    animatorController.StartMove(moveGrids);
                                    ChangeGameState(GameControlState.CharacterMoving);
                                    waitTick = 10;
                                }
                            }
                            else
                            {
                                //留个以后错误提示的位置
                            }
                        }
                    }
                }
                break;
            case GameControlState.ShowAttackableArea:
                {
                    // 以选中的敌人为中心 按照我的武器最小最大范围 搜索出一个攻击范围
                    // 找这个攻击范围 和我可移动范围的交集 
                    // 显示交集内的格子 不显示其他格子

                List<Vector2Int> currAttackRange = selectedCharacter.gameObject.GetComponent<CharacterAttack>().GetAttackRange(currEnemy.GetComponent<GridPosition>().grid, mapGenerator.mapSize, Constants.TargetType_Foe);
                List<DijkstraMoveInfo> currMoveRange = movementManager.LogicMoveRange;

                List<Vector2Int> attackableArea = new List<Vector2Int>();

                //在这里找交集 然后显示
                for (int i = 0; i < currMoveRange.Count; i++)
                {
                    Vector2Int gridPos = currMoveRange[i].position;
                    if (currAttackRange.Contains(gridPos))
                    {
                        attackableArea.Add(gridPos);
                        Debug.Log("gridPos: " + gridPos);
                    }
                }
                if (attackableArea.Count > 0)
                {
                    uiManager.ClearAllRange();
                    uiManager.ShowAttackableMoveRange(attackableArea);
                }

                if (Input.GetMouseButton(0) && waitTick <= 0) 
                    {
                        // 之后要做一个状态链
                        // 用来知道状态之间的变化关系
                        // 是个数组 从0开始 中间可能改写
                        Vector2Int currSelectGrid = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition));
                        if (attackableArea.Contains(currSelectGrid))
                        {
                            List<Vector2Int> moveGrids = movementManager.GetMovePath(currSelectGrid);
                            if (moveGrids.Count > 0)
                            {
                                uiManager.ClearAllRange();
                                for (int i = 0; i < moveGrids.Count; i++)
                                {
                                    uiManager.AddRange(SignType.Move,moveGrids[i]);
                                }

                                AnimatorController animatorController = selectedCharacter.gameObject.GetComponent<AnimatorController>();
                                if (animatorController != null)
                                {
                                    //lastPosition = currSelectGrid;  // 把这个位置更新了
                                    animatorController.StartMove(moveGrids);
                                    ChangeGameState(GameControlState.CharacterMoving);
                                    statusChain = true;  // 我之后不能再选敌人
                                    waitTick = 10;
                                }
                            }
                            else
                            {
                                //留个以后错误提示的位置
                            }
                        }
                    }

                }



                break;
            case GameControlState.CharacterMoving:
                {
                    if (selectedCharacter.gameObject == null)
                    {
                        ChangeGameState(GameControlState.SelectCharacter);
                        waitTick = 10;
                        uiManager.ClearAllRange();
                        return;
                    }
                    MoveByPath moveByPath = selectedCharacter.gameObject.GetComponent<MoveByPath>();
                    if (moveByPath == null || moveByPath.IsMoving == false)  //这次移动移动完成
                    {
                        uiManager.ClearAllRange();
                        if (!statusChain)  // 我还没选敌人
                        {
                            msgDlgButtonInfos = GetMsgDlgButtonInfos();
                            //这个hasReachableEnemies的判定得改掉 用现在新的菜单列表去管理状态
                            //bool hasReachableEnemies = hasAttackableCharacters();  //检查周围还有没有可攻击的敌人或友方 
                            uiManager.ShowMsgDlg(msgDlgButtonInfos);  
                            //if (hasReachableEnemies)
                            //{
                            //     GameState.gameControlState = GameControlState.WeaponSelect;  //选择武器
                            // }
                            // else
                            // {
                            //     GameState.gameControlState = GameControlState.CharacterMoveDone;  //一个角色移动完成
                            // }
                            ChangeGameState(GameControlState.ShowCommandMenu);
                            waitTick = 10;
                        }
                        else  // 我之前已经选过敌人了
                        {
                            ChangeGameState(GameControlState.ConfirmWeapon);
                        }
                    }
                }
                break;
            case GameControlState.ShowCommandMenu:
                {

                }
                break;
            case GameControlState.WeaponSelect:
                {
                    //这里还没做武器选择 就直接跳转选择攻击对象状态
                    ChangeGameState(GameControlState.SelectAttackObject);
                    waitTick = 10;
                }
                break;
            case GameControlState.SelectAttackObject:
                {
                    // SetUIPointerIndex();  // 这个位置可能之后再改
                    if (Input.GetMouseButton(0) && waitTick <= 0)
                    {
                        Vector2Int currSelectGrid = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition));
                        SelectedCharacterInfo attackObjectCharacter = GetCharacterInSelectedGrid(currSelectGrid);
                        if (attackObjectCharacter.characterIndex == Constants.nullCharacterIndex)  // 没选中任何角色
                        {
                            ChangeGameState(GameControlState.CharacterMoveDone);
                        }
                        else if (attackObjectCharacter.characterIndex == 0)  // 选中了我方角色
                        {
                            ChangeGameState(GameControlState.ConfirmWeapon);  // 确认使用的攻击武器
                        }
                        else  // 选中了某个敌方角色
                        {
                            ChangeGameState(GameControlState.ConfirmWeapon);  // 确认使用的攻击武器
                        }
                    }                    
                }
                break;
            case GameControlState.ConfirmWeapon:
                {
                    // 还没做 先直接跳转攻击阶段了
                    // 之后这里要根据选中的是友方还是敌方 确认不同的武器
                    //Debug.Log("GameControlState.ConfirmWeapon");
                    ChangeGameState(GameControlState.Attack); 
                }
                break;
            case GameControlState.Attack:
                {
                    // 还没做 先直接跳转行动完成阶段了
                    ChangeGameState(GameControlState.CharacterMoveDone);
                }
                break;
            case GameControlState.CharacterMoveDone:  // 每个角色移动完成后 检查是不是所有角色都移动完成
                {
                    uiManager.ClearAllRange();
                    bool haveAllFinished = true;
                    for (int i = 0; i < characters[0].Count; i++)
                    {
                        if (characters[0][i].animator.IsMoveFinished(false) == false)
                        {
                            haveAllFinished = false;
                            break;
                        }
                    }
                    if (haveAllFinished)  // 所有我方单位都移动完了
                    {
                        ChangeGameState(GameControlState.EndTurn);  //回合结束
                    }
                    else
                    {
                        ChangeGameState(GameControlState.SelectCharacter);  //选人移动
                    }
                }
                break;
            case GameControlState.EnemyTurnStart:
                {

                }
                break;
            case GameControlState.EndTurn:
                {
                    currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;

                    ChangeGameState(GameControlState.NewTurn);
                }
                break;

        }
        if (waitTick > 0)
        {
            waitTick -= 1;
        }

    }

    /// <summary>
    /// 切换游戏状态
    /// <param name="state">要切换的那个状态</param>
    /// </summary>
    private void ChangeGameState(GameControlState state)
    {
        GameState.gameControlState = state;
    }

    /// <summary>
    /// 获取当前所有可执行的按钮信息列表
    /// </summary>
    private List<MsgDlgButtonInfo> GetMsgDlgButtonInfos()
    {
        List<MsgDlgButtonInfo> currentMsgDlgButtonInfos = new List<MsgDlgButtonInfo>();
        byte abilities = CheckAbilities(selectedCharacter);
        bool canExchangeItem = CheckExchangeItemAbility();  // 这个还没写
        bool canAccessBackup = CheckAccessBackupAbility();  // 这个也还没写

        if ((Constants.TargetType_Foe&abilities) == Constants.TargetType_Foe)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Attack", AttackCommand));
        }
        if ((Constants.TargetType_Ally&abilities) == Constants.TargetType_Ally)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Heal", HealCommand));
        }
        if (canExchangeItem)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("canExchangeItem", ExchangeCommand));
        }
        if (canAccessBackup)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("AccessBackup", BackupCommand));
        }

        // 以下是啥时候都要加的按钮
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Wait", WaitCommand));
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Inventory", InventoryCommand));
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Cancel", CancelCommand));  


        return currentMsgDlgButtonInfos;

    }

    /// <summary>
    /// 攻击 debug阶段结束这个character的本轮行动
    /// </summary>
    private void AttackCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// Heal debug阶段结束这个character的本轮行动
    /// </summary>
    private void HealCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 交换物品 debug阶段结束这个character的本轮行动
    /// </summary>
    private void ExchangeCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 运输队 debug阶段结束这个character的本轮行动
    /// </summary>
    private void BackupCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 运输队 debug阶段结束这个character的本轮行动
    /// </summary>
    private void InventoryCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 结束这个character的本轮行动
    /// </summary>
    private void WaitCommand()
    {
        ChangeGameState(GameControlState.CharacterMoveDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 返回上一个行动状态
    /// </summary>
    private void CancelCommand()
    {
        selectedCharacter.animator.NewTurn();  // 把animator状态重置一下
        GridPosition gPos = selectedCharacter.gameObject.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = lastPosition;
            gPos.SynchronizeGridPosition();
        }
        uiManager.HideMsgDlg();
        ChangeGameState(GameControlState.SelectCharacter);
    }

    // private void DebugLogEvent(string eventData)  // 测试时候用的函数 打个log
    // {
    //     Debug.Log(eventData);
    //     uiManager.HideMsgDlg();
    // }

    /// <summary>
    /// 检查周围是否有可攻击或治疗的对象
    /// </summary>
    private byte CheckAbilities(CharacterObject selectedCharacterObject)
    {
        List<Vector2Int> currMoveRange = new List<Vector2Int>();  // 移动完之后 只能以它自己脚下那一格为移动范围去搜索范围
        Vector2Int currPos = selectedCharacterObject.gameObject.GetComponent<GridPosition>().grid;  // 当前位置
        currMoveRange.Add(currPos);
        byte targets = 0b0;

        List<CoveredRange> currRange = selectedCharacterObject.gameObject.GetComponent<CharacterAttack>().GetWeaponRange(currPos, mapGenerator.mapSize);
        foreach (CoveredRange range in currRange)
        {
            SelectedCharacterInfo selectedCharacterInfo = GetCharacterInSelectedGrid(range.gridPos);
            if (selectedCharacterInfo.IsNull() == false)
            {
                byte relation = selectedCharacterObject.GetRelation(characters[selectedCharacterInfo.playerIndex][selectedCharacterInfo.characterIndex]);
                targets |= (byte)(relation & range.targetType);
            }          
        }

        return targets;
    }

    /// <summary>
    /// 检查周围是否有可交换物品的友军
    /// </summary>     
    private bool CheckExchangeItemAbility()
    {
        return false;  // debug用 一会删
    }

    /// <summary>
    /// 检查是否可以使用运输队
    /// </summary>       
    private bool CheckAccessBackupAbility()
    {
        return false;  // debug用 一会删
    }
         

    private List<Vector2Int> GetOccupiedGrids(SelectedCharacterInfo currentCharacter)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();
        for (int i = 0; i < characters.Length; i++)
        {
            for (int j = 0; j < characters[i].Count; j++)
            {
                if (i != currentCharacter.playerIndex)
                {
                    occupiedGrids.Add(characters[i][j].gPos.grid);
                }
            }
        }
        return occupiedGrids;
    }

    private List<Vector2Int> GetAllyGrids(SelectedCharacterInfo currentCharacter)
    {
        List<Vector2Int> allyGrids = new List<Vector2Int>();
        for (int i = 0; i < characters.Length; i++)
        {
            for (int j = 0; j < characters[i].Count; j++)
            {
                if (i == currentCharacter.playerIndex && j != currentCharacter.characterIndex)
                {
                    allyGrids.Add(characters[i][j].gPos.grid);
                }
            }
        }
        return allyGrids;
    }

    private void StartGame(int playerCount)
    {
        this.playerCount = playerCount;

        characters = new List<CharacterObject>[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            characters[i] = new List<CharacterObject>();
        }

        List<int> tempPlayers = new List<int>();  // temp players 
        tempPlayers.Add(5);
        tempPlayers.Add(8);
        CreateAllCharacters(tempPlayers);

        ChangeGameState(GameControlState.NewTurn);
    }

    //判断选中的位置是什么东西
    private SelectedCharacterInfo GetCharacterInSelectedGrid(Vector2Int gridPosition)
    {

        for (int currPlayerIndex = 0; currPlayerIndex < playerCount; currPlayerIndex++)
        {
            for (int currCharacterIndex = 0; currCharacterIndex < characters[currPlayerIndex].Count; currCharacterIndex++)
            {
                if (characters[currPlayerIndex][currCharacterIndex].gPos.grid == gridPosition)
                {
                    return new SelectedCharacterInfo(currPlayerIndex, currCharacterIndex);
                }
            }
        }

        return new SelectedCharacterInfo(Constants.nullPlayerIndex, Constants.nullCharacterIndex);

    }

    private SelectedMovePos GetSelectedMoveInfo(Vector2Int selectedGridPos)
    {
        for (int i = 1; i < playerCount; i++)
        {
            for (int j = 1; j < characters[i].Count; j++)
            {
                if (selectedGridPos == characters[i][j].gPos.grid)
                {
                    return SelectedMovePos.enemy;
                }
            }
        }

        for (int i = 0; i < characters[0].Count; i++)
        {
            if (selectedGridPos == characters[0][i].gPos.grid)
            {
                return SelectedMovePos.unmoveable;
            }
        }

        List<DijkstraMoveInfo> currentMoveRange = movementManager.LogicMoveRange;  // 总觉得这么写会爆炸
        for (int i = 0; i < currentMoveRange.Count; i++)
        {
            if (selectedGridPos == currentMoveRange[i].position)
            {
                return SelectedMovePos.moveable;
            }
        }

        return SelectedMovePos.unmoveable;

    }


    private List<Vector2Int> GenerateGrids(List<int> playersAndCount)
    {
        int playerCount = playersAndCount.Count;
        int totalCount = 0;
        List<Vector2Int> allGrids = new List<Vector2Int>();
        List<Vector2Int> allDrawGrids = new List<Vector2Int>();

        for (int i = 0; i <= playerCount - 1; i++)
        {
            totalCount += playersAndCount[i];
        }

        for (int i = 0; i < mapGenerator.mapSize.x; i++)
        {
            for (int j = 0; j < mapGenerator.mapSize.y; j++)
            {
                allGrids.Add(new Vector2Int(i, j));
            }
        }

        for (int i = 0; i < totalCount; i++)
        {
            int index = Random.Range(0, allGrids.Count);
            allDrawGrids.Add(allGrids[index]);
            allGrids.RemoveAt(index);
        }

        return allDrawGrids;

    }

    private void CreateAllCharacters(List<int> playersAndCount)
    {
        int playerCount = playersAndCount.Count;
        List<Vector2Int> allGrids = GenerateGrids(playersAndCount);
        List<string> paths = new List<string>();
        int index = 0;

        paths.Add("Prefabs/Characters/Rabbit_Blue");
        paths.Add("Prefabs/Characters/Rabbit_Red");

        for (int i = 0; i < playerCount; i++)
        {
            for (int j = 0; j < playersAndCount[i]; j++)
            {
                CreateCharacter(paths[i], i, allGrids[index]);
                index++;
            }
        }
    }

    private void CreateCharacter(string prefabPath, int playerIndex, Vector2Int gridPosition)
    {
        GameObject character = Instantiate<GameObject>(Resources.Load<GameObject>(prefabPath));

        if (character == null) return;

        character.transform.SetParent(transform);

        GridPosition gPos = character.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = gridPosition;
            gPos.SynchronizeGridPosition();
        }

        SlaveState slaveTo = character.GetComponent<SlaveState>();
        if (slaveTo)
        {
            slaveTo.masterPlayerIndex = playerIndex;
        }

        AnimatorController animator = character.GetComponent<AnimatorController>();

        CharacterAttack attack = character.GetComponent<CharacterAttack>();
        if (attack != null)
        {
            attack.AddWeapon(new WeaponObj(1, 1, 2, Constants.TargetType_Foe));
            attack.weaponCurIndex = 0;
            attack.AddWeapon(new WeaponObj(1, 1, 3, Constants.TargetType_Ally));
        }
        characters[playerIndex].Add(new CharacterObject(gPos, slaveTo, animator, attack));
    }


}
