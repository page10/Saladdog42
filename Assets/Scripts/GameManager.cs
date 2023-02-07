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
    private GameObject selectedCharacter;  // selected character
    private MapGenerator mapGenerator;
    private UIManager uiManager;


    private void Awake()
    {
        currentCamera = GameObject.Find("Camera").GetComponent<Camera>();
        movementManager = GetComponent<MovementManager>();
        mapGenerator = GetComponent<MapGenerator>();
        uiManager = GetComponent<UIManager>();
        StartGame(2);
        //GameState.gameControlState = GameControlState.SelectCharacter;
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
                        GameState.gameControlState = GameControlState.EndTurn;
                        return;
                    }
                    uiManager.ClearMoveRange();
                    uiManager.ClearAttackRange();
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
                        GameState.gameControlState = GameControlState.SelectCharacter;
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
                            selectedCharacter = characters[currentCharacter.playerIndex][currentCharacter.characterIndex].gameObject;
                            if (characters[currentPlayerIndex][currentCharacter.characterIndex].animator.IsMoveFinished(false) == true)  // 移动完了
                            {
                                Debug.Log("test");
                            }
                            else // 还没移动
                            {
                                movementManager.GetMoveRange(selectedCharacter.GetComponent<CharacterMovement>(), GetOccupiedGrids(currentCharacter), GetAllyGrids(currentCharacter));
                                uiManager.ShowMoveRange(movementManager.LogicMoveRange);
                                uiManager.ShowAttackRange(selectedCharacter.GetComponent<CharacterAttack>(), MovementManager.GetV2IntFromDijkstraRange(movementManager.LogicMoveRange), mapGenerator.mapSize);  //拿到攻击范围V2Int List
                                GameState.gameControlState = GameControlState.ShowRange;
                                waitTick = 10;
                            }

                        }
                        else if (currentCharacter.playerIndex == Constants.nullPlayerIndex)  // 选到了空格子
                        {

                        }
                        else  // 选中了敌方角色
                        {
                            
                        }

                        //Debug.Log("mouseClicked");
                        //selectedCharacter = characters[0][0].gameObject;  //修改这个 根据鼠标位置选择   
                        //movementManager.GetMoveRange(selectedCharacter.GetComponent<CharacterMovement>());


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

                        Vector2Int characterGrid = selectedCharacter.GetComponent<GridPosition>().grid;  // 自己那格
                        Debug.Log("currSelectGrid , characterGrid" + currSelectGrid + "  " + characterGrid);
                        if (currentCharacter.playerIndex == 0 && currSelectGrid != characterGrid)
                        {
                            //选中了我方角色 除了自己
                            Debug.Log("selected ally characters");

                        }
                        else if (currentCharacter.playerIndex != Constants.nullPlayerIndex && currSelectGrid != characterGrid)
                        {
                            //选中了敌方角色
                            Debug.Log("selected enemy characters");
                            // 在这里新建一个状态 在这个状态里做以下事情
                            // 以选中的敌人为中心 按照我的武器最小最大范围 搜索出一个攻击范围
                            // 找这个攻击范围 和我可移动范围的交集 
                            // 显示交集内的格子 不显示其他格子
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
                                uiManager.ClearMoveRange();
                                uiManager.ClearAttackRange();
                                for (int i = 0; i < moveGrids.Count; i++)
                                {
                                    //Debug.Log("[" + i + "]" + moveGrids[i]);
                                    uiManager.AddMoveRange(moveGrids[i]);
                                }

                                if (selectedCharacter == null)
                                {
                                    GameState.gameControlState = GameControlState.SelectCharacter;
                                    waitTick = 10;
                                    return;
                                }
                                AnimatorController animatorController = selectedCharacter.GetComponent<AnimatorController>();
                                if (animatorController != null)
                                {
                                    animatorController.StartMove(moveGrids);
                                    GameState.gameControlState = GameControlState.CharacterMoving;
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
                    if (selectedCharacter == null)
                    {
                        GameState.gameControlState = GameControlState.SelectCharacter;
                        waitTick = 10;
                        uiManager.ClearMoveRange();
                        uiManager.ClearAttackRange();
                        return;
                    }
                    MoveByPath moveByPath = selectedCharacter.GetComponent<MoveByPath>();
                    if (moveByPath == null || moveByPath.IsMoving == false)  //这次移动移动完成
                    {
                        uiManager.ClearMoveRange();
                        uiManager.ClearAttackRange();
                        List<CharacterObject> reachableEnemies = GetReachableEnemies();  //检查周围还有没有可攻击的敌人
                        if (reachableEnemies.Count > 0)
                        {
                            GameState.gameControlState = GameControlState.WeaponSelect;  //选择武器
                        }
                        else
                        {
                            GameState.gameControlState = GameControlState.CharacterMoveDone;  //一个角色移动完成
                        }
                        waitTick = 10;
                    }


                }
                break;
            case GameControlState.WeaponSelect:
                {
                    //这里还没做武器选择 就直接跳转选择攻击对象状态
                    GameState.gameControlState = GameControlState.SelectAttackObject;
                    waitTick = 10;
                }
                break;
            case GameControlState.SelectAttackObject:
                {
                    if (Input.GetMouseButton(0) && waitTick <= 0)
                    {
                        Vector2Int currSelectGrid = movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition));
                        SelectedCharacterInfo attackObjectCharacter = GetCharacterInSelectedGrid(currSelectGrid);
                        if (attackObjectCharacter.characterIndex == Constants.nullCharacterIndex)  // 没选中任何角色
                        {
                            GameState.gameControlState = GameControlState.CharacterMoveDone;
                        }
                        else  // 选中了某个敌方角色
                        {
                            GameState.gameControlState = GameControlState.ConfirmWeapon;  // 确认使用的攻击武器
                        }
                    }                    
                }
                break;
            case GameControlState.ConfirmWeapon:
                {
                    // 还没做 先直接跳转攻击阶段了
                    GameState.gameControlState = GameControlState.Attack;
                }
                break;
            case GameControlState.Attack:
                {
                    // 还没做 先直接跳转行动完成阶段了
                    GameState.gameControlState = GameControlState.CharacterMoveDone;
                }
                break;
            case GameControlState.CharacterMoveDone:  // 每个角色移动完成后 检查是不是所有角色都移动完成
                {
                    uiManager.ClearMoveRange();
                    uiManager.ClearAttackRange();
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
                        GameState.gameControlState = GameControlState.EndTurn;  //回合结束
                    }
                    else
                    {
                        GameState.gameControlState = GameControlState.SelectCharacter;  //选人移动
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
                    GameState.gameControlState = GameControlState.NewTurn;
                }
                break;

        }
        if (waitTick > 0)
        {
            waitTick -= 1;
        }

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

        GameState.gameControlState = GameControlState.NewTurn;
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
        }
        characters[playerIndex].Add(new CharacterObject(gPos, slaveTo, animator, attack));
    }

    /// <summary>
    /// 获得移动之后攻击范围内的所有敌人
    /// </summary>
    private List<CharacterObject> GetReachableEnemies()
    {
        List<CharacterObject> reachableEnemies = new List<CharacterObject>();
        List<Vector2Int> currMoveRange = new List<Vector2Int>();  // 移动完之后 只能以它自己脚下那一格为移动范围去搜索攻击范围
        Vector2Int currPos = selectedCharacter.GetComponent<GridPosition>().grid;  // 当前位置
        currMoveRange.Add(currPos);
        uiManager.ShowAttackRange(selectedCharacter.GetComponent<CharacterAttack>(), currMoveRange, mapGenerator.mapSize);  //总觉得不该在这调用
        List<Vector2Int> currAttackRange = selectedCharacter.GetComponent<CharacterAttack>().GetAttackRange(currPos, mapGenerator.mapSize, true);

        for (int i = 1; i < playerCount; i++)
        {
            for (int j = 0; j < characters[i].Count; j++)
            {
                if (currAttackRange.Contains(characters[i][j].gPos.grid))
                {
                    reachableEnemies.Add(characters[i][j]);
                }
            }
        }
        return reachableEnemies;
    }

}
