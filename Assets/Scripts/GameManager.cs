
using System;
using System.Net.Http.Headers;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

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
    
    //战斗相关
    private BattleManager battleManager;
    private SelectedCharacterInfo currSelectedEnemy;  // 当前选中的敌方角色
    private SelectedCharacterInfo attacker;  // 当前选中的我方角色
    private SelectedCharacterInfo defender;  // 当前选中的敌方角色
    private List<SelectedCharacterInfo> attackTeam;  // 可发动连携攻击的队伍
    

    // 之后要被重构掉的东西
    private bool haveSelectedEnemy = false;  // 状态链
    List<MsgDlgButtonInfo> msgDlgButtonInfos = new List<MsgDlgButtonInfo>();  
    private Vector2Int lastPosition;  // 上一个状态时候的character位置
    
    private GameData _character = new GameData();  // todo 不确定是不是对的


    private void Awake()
    {
        _character.Start();
        currentCamera = GameObject.Find("Camera").GetComponent<Camera>();
        movementManager = GetComponent<MovementManager>();
        mapGenerator = GetComponent<MapGenerator>();
        uiManager = GetComponent<UIManager>();
        battleManager = GetComponent<BattleManager>();
        StartGame(2);
        //GameState.gameControlState = GameControlState.SelectCharacter;
        
        //debug用 之后删了
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("attack", (p)=>{Debug.Log("attack");}, Array.Empty<object>()));
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("exchange", (p)=>{Debug.Log("exchange");}, Array.Empty<object>()));
        msgDlgButtonInfos.Add(new MsgDlgButtonInfo("skip", (p)=>{Debug.Log("skip");}, Array.Empty<object>()));
    }

    public int waitTick = 0;

    private void FixedUpdate()
    {
        
        switch (GameState.gameControlState)
        {
            //todo 先选人后选移动位置的那个流程 状态跳转 选武器漏写了
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
                    // 看一下选中的是谁
                    if (Input.GetMouseButton(0) && waitTick <= 0)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
                    {
                        Vector2Int selectedGridPos = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition)
                            );
                        SelectedCharacterInfo currentCharacter = GetCharacterInSelectedGrid(selectedGridPos);

                        if (currentCharacter.playerIndex == currentPlayerIndex)  
                        {
                            // 如果选中了我方角色
                            // 优先判定是不是可移动
                            // 如果可移动 则展开移动和攻击范围 并进入「ShowRange」
                            // 不可移动状态下 判定是否可攻击 
                            // 如果可攻击 进入攻击武器选择状态「WeaponSelect」（在这个状态里去展开攻击范围 现在先不展开）
                            // 如果不可攻击 则进入行动完成「CharacterActionDone」状态
                            // （可移动不可攻击的判定在「ShowRange」里面处理）
                            
                            // 把攻击者设置为我选中的这个人
                            attacker = currentCharacter;   
                            // 拿到我选中的这个人的characterObject 
                            selectedCharacter = characters[currentCharacter.playerIndex][currentCharacter.characterIndex];
                            
                            // 确认一下是不是在播动画 如果没在播动画才有事情发生
                            if (selectedCharacter.animator.IsMoveFinished(false))  // 走路的动画还在播
                            {
                                //Debug.Log("test");
                            }
                            else // 没在播走路动画
                            {
                                
                                if (!selectedCharacter.hasMoved)  // 还没移动
                                {
                                    // 展开移动和攻击范围 并进入「ShowRange」
                                    movementManager.GetMoveRange(
                                        selectedCharacter.gameObject.GetComponent<CharacterMovement>(),
                                        GetOccupiedGrids(currentCharacter), GetAllyGrids(currentCharacter));
                                    uiManager.ShowAllRange(selectedCharacter.gameObject.GetComponent<CharacterAttack>(),
                                        MovementManager.GetV2IntFromDijkstraRange(movementManager.LogicMoveRange),
                                        mapGenerator.mapSize, GetAllyGrids(currentCharacter));
                                    ChangeGameState(GameControlState.ShowRange);

                                    waitTick = 10;
                                }
                                else if (selectedCharacter.hasMoved && !selectedCharacter.hasAttacked)  // 没攻击 移动过了
                                {
                                    // 进入攻击武器选择状态「WeaponSelect」
                                    GameState.gameControlState = GameControlState.WeaponSelect;
                                    
                                }

                                else  // 移动过了 攻击过了
                                {
                                    //什么事情也不发生 之后也可以在这里做一个状态展示UI
                                }
                                //这里调用时候 最后一个参数传的是所有的自己单位 其实不太对 

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
                    // 在这个状态下 如果选中了敌方角色 就进入攻击准备 也就是weaponSelect状态
                    // 如果选中了我方角色 且自己有治疗武器 也进入weaponSelect状态 否则什么都不发生
                    // 如果选中了移动范围内格子 就执行角色移动
                    // 如果选中了攻击范围内格子 判定是不是有
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
                            defender = currentEnemy;  
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
                                    selectedCharacter.hasMoved = true;
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
                List<Vector2Int> currAttackRange = selectedCharacter.gameObject.GetComponent<CharacterAttack>()
                    .GetAttackRange(currEnemy.GetComponent<GridPosition>().grid, mapGenerator.mapSize, Constants.TargetType_Foe);
                List<DijkstraMoveInfo> currMoveRange = movementManager.LogicMoveRange;

                List<Vector2Int> attackableArea = new List<Vector2Int>();

                //在这里找交集 然后显示
                for (int i = 0; i < currMoveRange.Count; i++)
                {
                    Vector2Int gridPos = currMoveRange[i].position;
                    if (currAttackRange.Contains(gridPos))
                    {
                        attackableArea.Add(gridPos);
                        //Debug.Log("gridPos: " + gridPos);
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
                                uiManager.AddRange(SignType.Move, moveGrids[i]);
                            }

                            AnimatorController animatorController =
                                selectedCharacter.gameObject.GetComponent<AnimatorController>();
                            if (animatorController != null)
                            {
                                //lastPosition = currSelectGrid;  // 把这个位置更新了
                                animatorController.StartMove(moveGrids);
                                ChangeGameState(GameControlState.CharacterMoving);
                                haveSelectedEnemy = true; // 我之后不能再选敌人
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
                        if (!haveSelectedEnemy)  // 我还没选敌人
                        {
                            msgDlgButtonInfos = GetMsgDlgButtonInfos();

                            uiManager.ShowMsgDlg(msgDlgButtonInfos);
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
            // case GameControlState.WeaponSelect:
            //     {
            //         // 展开武器选择菜单
            //         // 跳转到根据对应武器展开攻击范围并选择攻击目标的「SelectAttackObject」状态
            //         // 其实感觉这个状态可以不要的 毕竟只是展开菜单就切换状态了 只是做了这件事情 没有等待玩家输入
            //         // 应该把SelectAttackObject改成这个
            //
            //         msgDlgButtonInfos = GetWeaponMsgDlgButtonInfos(selectedCharacter.attack);
            //
            //         uiManager.ShowMsgDlg(msgDlgButtonInfos);
            //
            //
            //         
            //         ChangeGameState(GameControlState.SelectAttackObject);
            //         waitTick = 10;
            //     }
                // break;
            case GameControlState.WeaponSelect:
                {
                    
                    // 展开武器选择菜单
                    msgDlgButtonInfos = GetWeaponMsgDlgButtonInfos(selectedCharacter.attack);

                    uiManager.ShowMsgDlg(msgDlgButtonInfos);
                    
                    CharacterAttack characterAttack = selectedCharacter.gameObject.GetComponent<CharacterAttack>();

                    List<Vector2Int> currAttackRange = characterAttack.GetAttackRange(
                        selectedCharacter.gPos.grid, mapGenerator.mapSize,
                        characterAttack.Weapons[characterAttack.weaponCurIndex].target
                    );
                    
                    if (currAttackRange.Count > 0)
                    {
                        uiManager.ClearAllRange();
                        uiManager.ShowAttackableMoveRange(currAttackRange);
                    }


                    
                    if (Input.GetMouseButton(0) && waitTick <= 0)
                    {
                        Vector2Int currSelectGrid = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition));
                        
                        if (currAttackRange.Contains(currSelectGrid))
                        {
                            defender = GetCharacterInSelectedGrid(currSelectGrid);
                            // 这里是不是有必要根据当前持有的武器限定可点击对象呢？目前限定了 但是感觉有点僵硬
                            if (defender.characterIndex == Constants.nullCharacterIndex)  // 没选中任何角色
                            {

                            }
                            else if (defender.playerIndex == attacker.playerIndex &&
                                     (characterAttack.Weapons[characterAttack.weaponCurIndex].target |
                                      Constants.TargetType_Ally) == Constants.TargetType_Ally) // 选中了我方角色
                            {
                                CalBattlePreview();
                            }
                            else if (defender.playerIndex != attacker.playerIndex &&
                                     (characterAttack.Weapons[characterAttack.weaponCurIndex].target |
                                      Constants.TargetType_Foe) == Constants.TargetType_Foe) // 选中了某个敌方角色
                            {
                                CalBattlePreview();
                            }
                        
                        }
                        else
                        {
                            //留个以后错误提示的位置
                        }
                    }                    
                }
                break;
            case GameControlState.ConfirmWeapon:
                {
                    //UI 就是对应显示两边图片 攻击 会不会打死
                    // 这里和ShowCommandMenu是类似的
                    
                    RefreshAttackRange();  // 刷新攻击范围 

                }
                break;
            case GameControlState.Attack:
                {
                    // todo 总觉得这个状态也可以干掉呢 因为也就是做完一件事就跳转了 不存在循环呢

                    haveSelectedEnemy = false;  // 这个现在应该也要改掉了
                    Attack();
                }
                break;
            case GameControlState.PlayBattleAnimation:
                {
                    if (battleManager.IsPlayingAnimation == false)
                    {
                        ChangeGameState(GameControlState.CharacterActionDone);
                    }
                }
                break;
            case GameControlState.CharacterActionDone:  // 它的语义是 这回合行动结束
                {
                    
                    uiManager.ClearAllRange();
                    selectedCharacter.animator.FinishAction();
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
        Debug.Log(state);
        if (state == GameControlState.SelectCharacter)  // 死亡时移除
        {
            selectedCharacter = null;  // 回合开始 把我方和敌方的选中角色都清空
            currEnemy = null;

            foreach (List<CharacterObject> charas in characters)
            {
                int index = 0;
                while (index < charas.Count)
                {
                    if(charas[index].CanBeDestroyed)
                    {
                        Destroy(charas[index].gameObject);
                        charas.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
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
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Attack", AttackCommand, Array.Empty<object>()));
        }
        if ((Constants.TargetType_Ally&abilities) == Constants.TargetType_Ally)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Heal", HealCommand, Array.Empty<object>()));
        }
        if (canExchangeItem)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("canExchangeItem", ExchangeCommand, Array.Empty<object>()));
        }
        if (canAccessBackup)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("AccessBackup", BackupCommand, Array.Empty<object>()));
        }

        // 以下是啥时候都要加的按钮
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Wait", WaitCommand, Array.Empty<object>()));
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Inventory", InventoryCommand, Array.Empty<object>()));
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Cancel", CancelCommand, Array.Empty<object>()));  


        return currentMsgDlgButtonInfos;

    }

    /// <summary>
    /// 选择武器菜单页面
    /// </summary>
    /// <returns></returns>
    private List<MsgDlgButtonInfo> GetWeaponMsgDlgButtonInfos(CharacterAttack characterAttack)
    {
        // todo 筛选攻击范围内的我方和敌方 如果有才对应显示武器按钮
        List<MsgDlgButtonInfo> currentMsgDlgButtonInfos = new List<MsgDlgButtonInfo>();
        for (int i = 0; i < characterAttack.Weapons.Count; i++)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo(characterAttack.Weapons[i].weaponName, WeaponCommand, new object[] { i }));
        }
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo ("Cancel", CancelCommand, Array.Empty<object>()));  
        return currentMsgDlgButtonInfos;
    }
    
    /// <summary>
    /// 战斗预览菜单页面 选择武器部分
    /// </summary>
    /// <returns></returns>
    private List<MsgDlgButtonInfo> GetWeaponBattlePreviewInfos(CharacterAttack characterAttack)
    {
        // todo 筛选攻击范围内的我方和敌方 如果有才对应显示武器按钮
        List<MsgDlgButtonInfo> currentMsgDlgButtonInfos = new List<MsgDlgButtonInfo>();
        for (int i = 0; i < characterAttack.Weapons.Count; i++)
        {
            currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo(characterAttack.Weapons[i].weaponName, WeaponPreviewCommand,
                new object[] { i }));
        }

        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo("Cancel", CancelCommand, Array.Empty<object>()));
        currentMsgDlgButtonInfos.Add(new MsgDlgButtonInfo("AttackConfirm", AttackConfirmCommand, Array.Empty<object>()));
        return currentMsgDlgButtonInfos;
    }
    
    /// <summary>
    /// 用于选择武器界面的武器选择按钮
    /// </summary>
    /// <param name="args"></param>
    private void WeaponCommand(params object[] args)
    {
        selectedCharacter.attack.weaponCurIndex = (int)args[0];
        uiManager.HideMsgDlg();
    }
    
    /// <summary>
    /// 用于攻击预览界面的武器选择按钮
    /// </summary>
    /// <param name="args"></param>
    private void WeaponPreviewCommand(params object[] args)
    {
        selectedCharacter.attack.weaponCurIndex = (int)args[0];
        // todo 这里要重算一遍攻击 先这么写不一定对
        //uiManager.HideMsgDlg();
        CalBattlePreview();
    }
    
    /// <summary>
    /// 点这个按钮切到攻击执行
    /// </summary>
    private void AttackConfirmCommand(object[] args)
    {
        uiManager.HideMsgDlg();
        uiManager.ClearAllRange();
        uiManager.HideBattlePreviewPanel();
        // 移动完成之后的选择攻击目标
        GameState.gameControlState = GameControlState.Attack;

    }

    /// <summary>
    /// 在攻击状态里操作
    /// </summary>
    private void Attack()
    {
        BattleInputInfo battleInputInfo = new BattleInputInfo();
        battleInputInfo.attacker = characters[attacker.playerIndex][attacker.characterIndex];
        battleInputInfo.defender = characters[defender.playerIndex][defender.characterIndex];
        battleInputInfo.attackerWeapon = characters[attacker.playerIndex][attacker.characterIndex].attack.Weapons[0];
        battleInputInfo.defenderWeapon = characters[defender.playerIndex][defender.characterIndex].attack.Weapons[0];
        battleInputInfo.attackerPos = new Vector2Int(
            characters[attacker.playerIndex][attacker.characterIndex].gPos.grid.x,
            characters[attacker.playerIndex][attacker.characterIndex].gPos.grid.y);
        battleInputInfo.defenderPos = new Vector2Int(
            characters[defender.playerIndex][defender.characterIndex].gPos.grid.x,
            characters[defender.playerIndex][defender.characterIndex].gPos.grid.y);
        battleInputInfo.attackerTerrainStatus = mapGenerator.Map
        [
            battleInputInfo.attackerPos.x,
            battleInputInfo.attackerPos.y
        ].percentageModifier;
        battleInputInfo.defenderTerrainStatus = mapGenerator.Map
        [
            battleInputInfo.defenderPos.x,
            battleInputInfo.defenderPos.y
        ].percentageModifier;
        battleInputInfo.isSameSide = attacker.playerIndex == defender.playerIndex;
        
        battleManager.StartBattle(battleInputInfo);
        battleManager.StartBattleAnim();
        ChangeGameState(GameControlState.PlayBattleAnimation);
    }
    
    /// <summary>
    /// 点这个按钮切到攻击准备状态 在攻击状态里调攻击函数
    /// </summary>
    private void AttackCommand(object[] args)
    {
        uiManager.HideMsgDlg();
        // 移动完成之后的选择攻击目标
        GameState.gameControlState = GameControlState.WeaponSelect;

    }

    /// <summary>
    /// Heal debug阶段结束这个character的本轮行动
    /// </summary>
    private void HealCommand(object[] args)
    {
        ChangeGameState(GameControlState.CharacterActionDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 交换物品 debug阶段结束这个character的本轮行动
    /// </summary>
    private void ExchangeCommand(object[] args)
    {
        ChangeGameState(GameControlState.CharacterActionDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 运输队 debug阶段结束这个character的本轮行动
    /// </summary>
    private void BackupCommand(object[] args)
    {
        ChangeGameState(GameControlState.CharacterActionDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 运输队 debug阶段结束这个character的本轮行动
    /// </summary>
    private void InventoryCommand(object[] args)
    {
        ChangeGameState(GameControlState.CharacterActionDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 结束这个character的本轮行动
    /// </summary>
    private void WaitCommand(object[] args)
    {
        ChangeGameState(GameControlState.CharacterActionDone);
        uiManager.HideMsgDlg();
    }

    /// <summary>
    /// 返回上一个行动状态
    /// </summary>
    private void CancelCommand(object[] args)
    {
        selectedCharacter.animator.NewTurn();  // 把animator状态重置一下
        GridPosition gPos = selectedCharacter.gameObject.GetComponent<GridPosition>();
        if (gPos != null)
        {
            gPos.grid = lastPosition;
            gPos.SynchronizeGridPosition();
        }
        uiManager.HideMsgDlg();
        uiManager.HideBattlePreviewPanel();
        uiManager.ClearAllRange();
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
            attack.AddWeapon(new WeaponObj(1, 1, 2, Constants.TargetType_Foe, WeaponType.DoubleAttack, "targetFoe", 20, "iconPath"));
            attack.weaponCurIndex = 0;
            //todo 治疗相关的战斗逻辑没写 是治疗武器的话 数值怎么计算 血量上限的计算
            attack.AddWeapon(new WeaponObj(1, 1, 1, Constants.TargetType_Ally, WeaponType.NormalAttack, "targetAlly", 20, "iconPath"));
        }

        CharacterObject characterObject = character.GetComponent<CharacterObject>();
        characterObject.characterName = _character.characterNameModel.names[Random.Range(0, _character.characterNameModel.names.Count)];  // todo 不一定是对的
        characters[playerIndex].Add(characterObject);
    }

    /// <summary>
    /// 切武器时候刷新攻击范围
    /// </summary>
    private void RefreshAttackRange()
    {
        CharacterAttack characterAttack = selectedCharacter.gameObject.GetComponent<CharacterAttack>();

        List<Vector2Int> currAttackRange = characterAttack.GetAttackRange(
            selectedCharacter.gPos.grid, mapGenerator.mapSize,
            characterAttack.Weapons[characterAttack.weaponCurIndex].target
        );
                    
        if (currAttackRange.Count > 0)
        {
            uiManager.ClearAllRange();
            uiManager.ShowAttackableMoveRange(currAttackRange);
        }
    }

    /// <summary>
    /// 计算战斗预览 用于显示
    /// </summary>
    private void CalBattlePreview()
    {
        BattleInputInfo battleInputInfo = new BattleInputInfo();
        battleInputInfo.attacker = characters[attacker.playerIndex][attacker.characterIndex];
        battleInputInfo.defender = characters[defender.playerIndex][defender.characterIndex];
        battleInputInfo.attackerWeapon = characters[attacker.playerIndex][attacker.characterIndex].attack.Weapons[0];  
        battleInputInfo.defenderWeapon = characters[defender.playerIndex][defender.characterIndex].attack.Weapons[0];
        battleInputInfo.attackerPos = new Vector2Int(
            characters[attacker.playerIndex][attacker.characterIndex].gPos.grid.x,
            characters[attacker.playerIndex][attacker.characterIndex].gPos.grid.y);
        battleInputInfo.defenderPos = new Vector2Int(
            characters[defender.playerIndex][defender.characterIndex].gPos.grid.x,
            characters[defender.playerIndex][defender.characterIndex].gPos.grid.y);
        battleInputInfo.attackerTerrainStatus = mapGenerator.Map
        [
            battleInputInfo.attackerPos.x,
            battleInputInfo.attackerPos.y
        ].percentageModifier;
        battleInputInfo.defenderTerrainStatus = mapGenerator.Map
        [
            battleInputInfo.defenderPos.x,
            battleInputInfo.defenderPos.y
        ].percentageModifier;
        battleInputInfo.isSameSide = attacker.playerIndex == defender.playerIndex;
        
        battleManager.StartBattle(battleInputInfo);
        battleManager.StartBattle(battleInputInfo);  // todo 这里 和 下面有一段一样的丑陋代码 一会提出来

        msgDlgButtonInfos = GetWeaponBattlePreviewInfos(selectedCharacter.attack);  // 武器选择功能
        uiManager.ShowMsgDlg(msgDlgButtonInfos);
                                
        uiManager.ShowBattlePreviewPanel(battleManager.SingleBattleInfo);  // 攻击预览
        ChangeGameState(GameControlState.ConfirmWeapon); // 确认使用的攻击武器
        
    }


}
