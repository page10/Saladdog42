using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    //private GridPosition _movingCharacter;

    
    //private List<DijkstraMoveInfo> _logicMoveRange = new List<DijkstraMoveInfo>();
    //public List<DijkstraMoveInfo> LogicMoveRange { get => _logicMoveRange; }

    private void Awake()
    {
        _mapGenerator = GetComponent<MapGenerator>();
    }

    ///<summary>
    ///获得移动范围 
    ///<param name="characterMove" >  角色移动信息 </param>
    ///<param name="occupiedGrids"> 是被其他单位占了的位置 </param>
    ///<param name="allyGrids"> 是被友军单位占了的位置 </param>
    ///</summary>
    public List<DijkstraMoveInfo> GetMoveRange(CharacterMovement characterMove, List<Vector2Int> occupiedGrids, List<Vector2Int> allyGrids)  
    {
        if (!_mapGenerator || !characterMove) return new List<DijkstraMoveInfo>();

        int width = _mapGenerator.Map.GetLength(0);
        int height = _mapGenerator.Map.GetLength(1);
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costMap[x, y] = occupiedGrids.Contains(new Vector2Int(x, y))?
                            int.MaxValue :
                            _mapGenerator.Map[x, y].Cost(characterMove.movePower.moveType);
            }
        }
        characterMove.SetCostMap(costMap);

        GridPosition position = characterMove.GetComponent<GridPosition>();
        Vector2Int pos = position ? position.grid : Vector2Int.zero;
        return characterMove.GetDijkstraRange(pos);
    }
    
    /// <summary>
    /// 获得一个角色可以移动到的所有单元格
    /// </summary>
    /// <param name="character">角色</param>
    /// <param name="occupiedGrids">已经被他的敌人占领的格子</param>
    /// <param name="allyGrids">友军所在的格子</param>
    public List<DijkstraMoveInfo> GetMoveRange(CharacterObject character, List<Vector2Int> occupiedGrids, List<Vector2Int> allyGrids)
    {
        CharacterMovement characterMove = character.gameObject.GetComponent<CharacterMovement>();
        List<DijkstraMoveInfo> res = new List<DijkstraMoveInfo>();
        if (!_mapGenerator || !characterMove) return res;

        //_movingCharacter = character.gameObject.GetComponent<GridPosition>();

        int width = _mapGenerator.Map.GetLength(0);
        int height = _mapGenerator.Map.GetLength(1);
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int thisG = new Vector2Int(x, y);
                costMap[x, y] = occupiedGrids.Contains(thisG) || allyGrids.Contains(thisG) ?
                    int.MaxValue :
                    _mapGenerator.Map[x, y].Cost(characterMove.movePower.moveType);
            }
        }
        characterMove.SetCostMap(costMap);

        GridPosition position = character.gameObject.GetComponent<GridPosition>();
        Vector2Int pos = position ? position.grid : Vector2Int.zero;
        return characterMove.GetDijkstraRange(pos);
        
    }

    

    public Vector2Int GetGridPosition(Vector3 position3)
    {
        //Debug.Log("position3: " + position3);
        Vector2Int tempVec2Int = new Vector2Int(
            Mathf.FloorToInt((position3.x + Constants.tileSize / 4.00f) / Constants.tileSize),
            Mathf.FloorToInt((position3.y + Constants.tileSize / 4.00f) / Constants.tileSize));
        //Debug.Log("tempVec2Int: " + tempVec2Int);

        return new Vector2Int(
            Mathf.FloorToInt((position3.x + Constants.tileSize / 4.00f) / Constants.tileSize),
            Mathf.FloorToInt((position3.y + Constants.tileSize / 4.00f) / Constants.tileSize)
        );
    }

    // private int IndexInLogicMoveRange(Vector2Int grid)
    // {
    //     for (int i = 0; i < _logicMoveRange.Count; i++)
    //     {
    //         if (_logicMoveRange[i].position == grid) return i;
    //     }
    //     return -1;
    // }

    public List<Vector2Int> GetMovePath(CharacterObject characterObject, Vector2Int targetGrid,
        List<Vector2Int> occupiedGrids, List<Vector2Int> allyGrids)
    {
        GridPosition gp = characterObject.GetComponent<GridPosition>();
        Vector2Int startGrid = gp ? gp.grid : targetGrid;
        List<Vector2Int> res = new List<Vector2Int>();
        List<DijkstraMoveInfo> logicMoveRange = GetMoveRange(characterObject, occupiedGrids, allyGrids);

        int IndexInLogicMoveRange(Vector2Int g) 
        {
            int idx = -1;
            for (int i = 0; i < logicMoveRange.Count; i++)
            {
                if (logicMoveRange[i].position.x == g.x && logicMoveRange[i].position.y == g.y)
                {
                    idx = i;
                    break;
                }
            }

            return idx;
        }


        int targetGridIndex = IndexInLogicMoveRange(targetGrid);
        if (targetGridIndex < 0)
        {
            res.Add(startGrid);  //根据这个判定是不是点到可移动范围之外去了 但不可以走 还有其他的可能性 得加个判断 是哪种情况
            return res;
        }

        int currentIndex = targetGridIndex;
        while (true)  //根据dijkstra里各点的from 获得从起点到终点的路径
        {
            if (
                logicMoveRange[currentIndex].position == logicMoveRange[currentIndex].from ||
                logicMoveRange[currentIndex].position == startGrid
            )
            {
                res.Add(startGrid);
                break;
            }
            else
            {
                res.Add(logicMoveRange[currentIndex].position);
                currentIndex = IndexInLogicMoveRange(logicMoveRange[currentIndex].from);
                if (currentIndex < 0) break;
            }
        }
        res.Reverse();
        return res;
    }

    /// <summary>
    /// 根据DijkstraMoveInfo的List给出V2Int的List
    /// </summary>
    public static List<Vector2Int> GetV2IntFromDijkstraRange(List<DijkstraMoveInfo> dijkstraRange)
    {
        if (dijkstraRange.Count == 0) return new List<Vector2Int>();

        List<Vector2Int> resRange = new List<Vector2Int>();
        for (int i = 0; i < dijkstraRange.Count; i++)
        {
            resRange.Add(dijkstraRange[i].position);
        }

        return resRange;

    }

}
