using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public int MovePoints = 6;  //这玩意儿应该是要和MoveType有关联的 不过先这么写吧
    public MoveType moveType = MoveType.Mount;  //骑兵
    private int[,] costMap;  //对应兵种的地形消耗map
    private Vector2Int nowCoordinate;  //用来存当前位置
    [SerializeField] private GameRangeItem moveRangePrefab;  //用来显示可移动范围的东西
    private List<GameRangeItem> moveRangeItems = new List<GameRangeItem>();  //可移动范围
	[SerializeField] private Transform rangeItemParent;  
    private List<Vector2Int> dijkstraRange;  //Dijkstra生成的移动范围
    private Dijkstra Pathfinder = new Dijkstra();  //用来寻路的东西

    public void Init(Vector2Int coord)  
    {
        nowCoordinate = coord;
        transform.position = new Vector3(nowCoordinate.x, nowCoordinate.y);
        Pathfinder.map = costMap;
    }
    private void CreateMoveRangePrefab(Vector2Int coord)  
    {
        var moveRange = Instantiate(moveRangePrefab, rangeItemParent, false);
		moveRange.Coordinate = coord;
		moveRange.Visible = false;
		moveRangeItems.Add(moveRange);
    }
    private void SetRangeItems()
    {
        CreateMoveRangePrefab(nowCoordinate);  
        foreach (Vector2Int moveable in dijkstraRange)  // 这里 Dijkstra生成的范围
        {
            CreateMoveRangePrefab(moveable); 
        }
    }
    private void HideRanges()
		{
			foreach (var item in moveRangeItems)
			{
				item.Visible = false;
			}
		}
    private void Update()
		{            
			if (Input.GetMouseButtonDown(0))
			{
                GetDijkstraRange();
                Vector3 mousePos = Input.mousePosition;
                Vector2Int IntCoordinate = new Vector2Int((int)(mousePos.x / 2), (int)(mousePos.y / 2));
				//判定是否可以移动 如果在范围内 就可以移动 
                if (dijkstraRange.Contains(IntCoordinate))
                {
                    Move(IntCoordinate);
                }
			}
		}
    private void Move(Vector2Int coord)  //移动
    {
        nowCoordinate = coord;
        transform.position = new Vector3(nowCoordinate.x, nowCoordinate.y);
    }
    private void GetDijkstraRange()  //拿范围
    {
        Pathfinder.map = costMap;
        var dijkstraReturn = Pathfinder.GetCanMoveGrids(MovePoints, nowCoordinate);
        foreach (var moveable in dijkstraReturn)
        {
            dijkstraRange.Add(moveable.position);
        }
    }

}
