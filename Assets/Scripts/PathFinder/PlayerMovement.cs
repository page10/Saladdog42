using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    //这个脚本 能做的应该是 调用dijkstra 根据dijkstra生成的范围 执行移动 

    //account, role, character, player 
    //player 控制 role 里面的 character
    //命名中在这种地方少出现player 应该是character
    public int MovePoints = 6;  //这玩意儿应该是要和MoveType有关联的 不过先这么写吧
    public MoveType moveType = MoveType.Mount;  //骑兵
    //private int[,] costMap;  //对应兵种的地形消耗map 
    //这个不应该在这个脚本里 
    // 应该交给地图来管理 
    // 比如地图改变了 不应该改变player身上的数据

    //private Vector2Int nowCoordinate;  //用来存当前单元格坐标
    //这个也应该在地图里控制 地图里有个数组控制了各个角色的坐标 
    //这个应该是依赖于地图里的坐标的

    //[SerializeField] private GameRangeItem moveRangePrefab;  //用来显示可移动范围的东西
    //private List<GameRangeItem> moveRangeItems = new List<GameRangeItem>();  //可移动范围
    //这俩都在GameManager里面 或者专门的绘制脚本 
    //专门的绘制格子的系统
    // 不应该由characterMove来做 顶多提供访问的数据给它
	//[SerializeField] private Transform rangeItemParent;  

    private List<Vector2Int> _dijkstraRange;  //Dijkstra生成的移动范围 能get不能set
    private Dijkstra _pathFinder = new Dijkstra();  //用来寻路的东西
    // private 变量命名最好是_pathFinder 

    // public void Init(Vector2Int coord)  
    // {
        // nowCoordinate = coord;
        // transform.position = new Vector3(nowCoordinate.x, nowCoordinate.y);
        // _pathFinder.map = costMap;
    // }
    // private void CreateMoveRangePrefab(Vector2Int coord)  //不用了
    // {
        // var moveRange = Instantiate(moveRangePrefab, rangeItemParent, false);
		// moveRange.Coordinate = coord;
		// moveRange.Visible = false;
		// moveRangeItems.Add(moveRange);
    // }
    // private void SetRangeItems()  // 
    // {
        // CreateMoveRangePrefab(nowCoordinate);  
        // foreach (Vector2Int moveable in _dijkstraRange)  // 这里 Dijkstra生成的范围
        // {
        //     CreateMoveRangePrefab(moveable); 
        // }
    // }
    // private void HideRanges()
	// 	{
			// foreach (var item in moveRangeItems)
			// {
			// 	item.Visible = false;
			// }
		// }
    // private void Update()
	// 	{            
	// 		if (Input.GetMouseButtonDown(0))  //input不能发生在update里发生 有专门的inputManager 语法不对 而且这个程序里不需要Update了
	// 		{
                // GetDijkstraRange();
                // Vector3 mousePos = Input.mousePosition;
                // Vector2Int IntCoordinate = new Vector2Int((int)(mousePos.x / 2), (int)(mousePos.y / 2));
				// //判定是否可以移动 如果在范围内 就可以移动 
                // if (_dijkstraRange.Contains(IntCoordinate))
                // {
                //     Move(IntCoordinate);
                // }
		// 	}
		// }
    // private void Move(Vector2Int coord)  //移动
    // {
    //     nowCoordinate = coord;
    //     transform.position = new Vector3(nowCoordinate.x, nowCoordinate.y);
    // }
    public void GetDijkstraRange(Vector2Int pos)  //拿范围
    {
        _dijkstraRange.Clear();

        List<DijkstraMoveInfo> dijkstraReturn = _pathFinder.GetCanMoveGrids(MovePoints, pos);
        foreach (var moveable in dijkstraReturn)
        {
            _dijkstraRange.Add(moveable.position);
        }
    }
    public void SetCostMap(int[,] map)  //这个是让别人来改动这个costMap的
    {
        _pathFinder.map = map;
    }
 // 那个绘制 要单独加个component绘制范围prefab 
 // 现在做到点一下能展开移动范围 点一下收起来就行 
}
