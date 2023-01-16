using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    private GridPosition _movingCharacter;
    private List<GameObject> moveRange = new List<GameObject>();
    private List<DijkstraMoveInfo> _logicMoveRange = new List<DijkstraMoveInfo>();

    private void Awake()
    {
        _mapGenerator = GetComponent<MapGenerator>();
    }

    public void GetMoveRange(CharacterMovement characterMove)
    {
        if (!_mapGenerator || !characterMove) return;

        _movingCharacter = characterMove.GetComponent<GridPosition>();

        int width = _mapGenerator.Map.GetLength(0);
        int height = _mapGenerator.Map.GetLength(1);
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costMap[x, y] = _mapGenerator.Map[x, y].Cost(characterMove.movePower.moveType);
            }
        }
        characterMove.SetCostMap(costMap);

        GridPosition position = characterMove.GetComponent<GridPosition>();
        Vector2Int pos = position ? position.grid : Vector2Int.zero;
        _logicMoveRange = characterMove.GetDijkstraRange(pos);
        ShowMoveRange();
    }

    private void ShowMoveRange()
    {
        ClearMoveRange();

        for (int i = 0; i < _logicMoveRange.Count; i++)
        {
            AddMoveRange(_logicMoveRange[i].position);
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

    public Vector2Int GetGridPosition(Vector3 position3)
    {
        Debug.Log("position3: " + position3);
        Vector2Int tempVec2Int = new Vector2Int(
            Mathf.FloorToInt((position3.x - Constants.tileSize / 4.00f) / Constants.tileSize),
            Mathf.FloorToInt((position3.y - Constants.tileSize / 4.00f) / Constants.tileSize));
        Debug.Log("tempVec2Int: " + tempVec2Int);

        return new Vector2Int(
            Mathf.FloorToInt((position3.x - Constants.tileSize / 4.00f) / Constants.tileSize),
            Mathf.FloorToInt((position3.y - Constants.tileSize / 4.00f) / Constants.tileSize)
        );
    }

    private int IndexInLogicMoveRange(Vector2Int grid)
    {
        for (int i = 0; i < _logicMoveRange.Count; i++)
        {
            if (_logicMoveRange[i].position == grid) return i;
        }
        return -1;
    }

    public List<Vector2Int> GetMovePath(Vector2Int targetGrid)
    {
        Vector2Int startGrid = _movingCharacter.grid;
        List<Vector2Int> res = new List<Vector2Int>();
        int targetGridIndex = IndexInLogicMoveRange(targetGrid);
        if (targetGridIndex < 0)
        {
            res.Add(startGrid);
            return res;
        }

        int currentIndex = targetGridIndex;
        while (true)
        {
            if (
                _logicMoveRange[currentIndex].position == _logicMoveRange[currentIndex].from ||
                _logicMoveRange[currentIndex].position == startGrid
            )
            {
                res.Add(startGrid);
                break;
            }
            else
            {
                res.Add(_logicMoveRange[currentIndex].position);
                currentIndex = IndexInLogicMoveRange(_logicMoveRange[currentIndex].from);
                if (currentIndex < 0) break;
            }
        }
        res.Reverse();
        return res;
    }



}
