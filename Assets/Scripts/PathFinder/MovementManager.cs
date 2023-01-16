using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    private CharacterPosition _position;
    private List<GameObject> moveRange = new List<GameObject>();

private void Awake() {
    _mapGenerator = GetComponent<MapGenerator>();
}
    
    public void GetMoveRange(CharacterMovement characterMove)
    {
        if (!_mapGenerator || !characterMove) return;

        int width = _mapGenerator.Map.GetLength(0);
        int height = _mapGenerator.Map.GetLength(1);
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costMap[x, y] = _mapGenerator.Map[x, y].Cost(characterMove.moveType);
            }
        }
        characterMove.SetCostMap(costMap);

        GridPosition position = characterMove.GetComponent<GridPosition>();
        Vector2Int pos = position ? position.grid : Vector2Int.zero;
        ShowMoveRange(characterMove.GetDijkstraRange(pos));
    }

    private void ShowMoveRange(List<Vector2Int> range)
    {
        if (moveRange.Count > 0)
        {
            foreach (GameObject gameRange in moveRange)
            {
                Destroy(gameRange);
            }
            moveRange.Clear();
        }

        for (int i = 0; i < range.Count; i++)
        {
            GameObject grid = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/MoveRange"));
            if (grid == null) return;

            grid.transform.SetParent(transform);

            GridPosition gPos = grid.GetComponent<GridPosition>();
            if (gPos != null)
            {
                gPos.grid = range[i];
                gPos.SynchronizeGridPosition();
            }

            moveRange.Add(grid);
        }

    }


    // public bool clicked = false;
    // private void FixedUpdate() {
    //     if (Input.GetMouseButton(0) && clicked == false)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
    // 		{
    //             Debug.Log("mouseClicked");  //还得写一个鼠标点击位置选中角色的东西
    //             clicked = true;
    //             //GetMoveRange();
    // 		}
    //         else
    //         {
    //             clicked = false;
    //         }
    // }
}
