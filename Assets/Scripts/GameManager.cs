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

    private void Awake()
    {
        currentCamera = GameObject.Find("Camera").GetComponent<Camera>();
        movementManager = GetComponent<MovementManager>();
        mapGenerator = GetComponent<MapGenerator>();
        StartGame(2);
    }

    public int waitTick = 0;
    private void FixedUpdate()
    {
        switch (GameState.gameControlState)
        {
            case GameControlState.NewTurn:
                {
                    movementManager.ClearMoveRange();
                    //重置可移动角色的移动状态    
                    //这里直接把所有角色的可移动状态重置吧 反正玩家也操作不了敌人队伍
                    for (int i = 0; i < playerCount; i++)
                    {
                        for (int j = 0; j < characters[i].Count; j++)
                        {
                            characters[i][j].animator.NewTurn();  
                        }
                    }
                
                }
                break;
            case GameControlState.SelectCharacter:
                {                    
                    if (Input.GetMouseButton(0) && waitTick <= 0)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
                    {
                        //Debug.Log("mouseClicked");
                        selectedCharacter = characters[0][0].gameObject;
                        movementManager.GetMoveRange(selectedCharacter.GetComponent<CharacterMovement>());
                        GameState.gameControlState = GameControlState.ShowMoveRange;
                        waitTick = 10;
                    }
                }
                break;
            case GameControlState.ShowMoveRange:
                {
                    if (Input.GetMouseButton(0) && waitTick <= 0)
                    {
                        List<Vector2Int> moveGrids = movementManager.GetMovePath(
                            movementManager.GetGridPosition(
                                currentCamera.ScreenToWorldPoint(Input.mousePosition)
                            )
                        );
                        if (moveGrids.Count > 1)
                        {
                            movementManager.ClearMoveRange();
                            for (int i = 0; i < moveGrids.Count; i++)
                            {
                                Debug.Log("[" + i + "]" + moveGrids[i]);
                                movementManager.AddMoveRange(moveGrids[i]);
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
                        
                    }
                }
                break;
            case GameControlState.CharacterMoving:
                {
                    if (selectedCharacter == null)
                    {
                        GameState.gameControlState = GameControlState.SelectCharacter;
                        waitTick = 10;
                        movementManager.ClearMoveRange();
                        return;
                    }
                    MoveByPath moveByPath = selectedCharacter.GetComponent<MoveByPath>();
                    if (moveByPath == null || moveByPath.IsMoving == false)
                    {
                        GameState.gameControlState = GameControlState.SelectCharacter;
                        movementManager.ClearMoveRange();
                        waitTick = 10;
                    }
                }
                break;

        }
        if (waitTick > 0)
        {
            waitTick -= 1;
        }

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

    private List<Vector2Int> GenerateGrids(List<int> playersAndCount)
    {
        int playerCount = playersAndCount.Count;
        int totalCount = 0;
        List<Vector2Int> allGrids = new List<Vector2Int>();
        List<Vector2Int> allDrawGrids = new List<Vector2Int>();

        for(int i = 0; i <= playerCount - 1; i++)
        {
            totalCount += playersAndCount[i];
        }

        for (int i = 0; i < mapGenerator.mapSize.x; i++)
        {
            for (int j = 0; j < mapGenerator.mapSize.y; j++)
            {
                allGrids.Add(new Vector2Int(i,j));
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

        characters[playerIndex].Add(new CharacterObject(gPos, slaveTo, animator));

    }


}
