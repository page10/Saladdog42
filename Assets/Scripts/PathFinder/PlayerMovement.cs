using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public int MovePoints = 6;  //这玩意儿应该是要和MoveType有关联的
    public MoveType moveType = MoveType.Mount;  //骑兵
    private int[,] costMap;  //对应兵种的地形消耗map
    public Vector2Int NowCoordinate { get; set; }
    [SerializeField] private GameRangeItem moveRangePrefab;
    private List<GameRangeItem> moveRangeItems = new List<GameRangeItem>();
	[SerializeField] private Transform rangeItemParent;
    private List<Vector2Int> dijkstraRange;  //move range Vector2Int list created from Dijkstra 
    private Dijkstra Pathfinder = new Dijkstra();

    public void Init(Vector2Int coord)
    {
        NowCoordinate = coord;
        transform.position = new Vector3(NowCoordinate.x, NowCoordinate.y);
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
        CreateMoveRangePrefab(NowCoordinate);  
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
                Vector3 mousePos = Input.mousePosition;
                Vector2Int IntCoordinate = new Vector2Int((int)(mousePos.x / 2), (int)(mousePos.y / 2));
				//判定是否可以移动 如果在范围内 就可以移动 
                if (dijkstraRange.Contains(IntCoordinate))
                {
                    Move(IntCoordinate);
                }
			}
		}
    private void Move(Vector2Int coord)  //和init里干一样的事情 
    {
        NowCoordinate = coord;
        transform.position = new Vector3(NowCoordinate.x, NowCoordinate.y);
    }
    private void GetDijkstraRange()
    {

    }

}
