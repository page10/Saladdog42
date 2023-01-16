using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MovementManager movementManager;
    private Camera currentCamera;

    private int currentPlayerIndex = 0;

    private int playerCount = 2;
    private List<GameObject>[] characters;  // all rabbits(characters)

    private void Awake()
    {
        currentCamera = GameObject.Find("Camera").GetComponent<Camera>();
        movementManager = GetComponent<MovementManager>();
        StartGame(2);
    }

    public int waitTick = 0;
    private void FixedUpdate()
    {
        switch (GameState.gameControlState)
        {
            case GameControlState.SelectCharacter:
                {
                    if (Input.GetMouseButton(0) && waitTick <= 0)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
                    {
                        //Debug.Log("mouseClicked");
                        movementManager.GetMoveRange(characters[0][0].GetComponent<CharacterMovement>());
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
                        }
                        GameState.gameControlState = GameControlState.CharacterMoving;
                        waitTick = 10;
                    }                    
                }
                break;
            case GameControlState.CharacterMoving:
                {
                    
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

        characters = new List<GameObject>[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            characters[i] = new List<GameObject>();
        }

        CreateCharacter("Prefabs/Player", 0, new Vector2Int(3, 3));
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

        characters[playerIndex].Add(character);
    }


}
