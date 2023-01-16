using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MovementManager movementManager;


    private int currentPlayerIndex = 0;

    private int playerCount = 2;
    private List<GameObject>[] characters;  // all rabbits(characters)

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        StartGame(2);
    }

    public bool clicked = false;
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && clicked == false)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
        {
            Debug.Log("mouseClicked");
            clicked = true;
            movementManager.GetMoveRange(characters[0][0].GetComponent<CharacterMovement>());
        }
        else
        {
            clicked = false;
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
