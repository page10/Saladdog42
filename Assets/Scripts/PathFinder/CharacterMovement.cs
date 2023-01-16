using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public MovementInfo movePower = new MovementInfo(MoveType.Mount, 10);
    private Dijkstra _pathFinder = new Dijkstra();  //用来寻路的东西

    public List<DijkstraMoveInfo> GetDijkstraRange(Vector2Int pos)  //拿范围
    {
        return _pathFinder.GetCanMoveGrids(movePower.moveCost, pos);
    }
    //public bool clicked = false;
    // private void FixedUpdate()
    // {
    //     if (Input.GetMouseButton(0) && clicked == false)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
    //     {
    //         Debug.Log("mouseClicked");
    //         clicked = true;
    //         Vector3 mousePos = Input.mousePosition;
    //         Debug.Log("mouseClicked: " + mousePos);
    //         Vector2Int IntCoordinate = new Vector2Int((int)(mousePos.x / 2), (int)(mousePos.y / 2));
    //         //判定是否可以移动 如果在范围内 就可以移动 
    //         if (_dijkstraRange.Contains(IntCoordinate))
    //         {
    //             Debug.Log("in range");
    //             Move(IntCoordinate);
    //         }
    //     }
    //     else
    //     {
    //         clicked = false;
    //     }
    // }
    // private void Move(Vector2Int coord)  //移动
    // {
    //     Vector3 mousePos = Input.mousePosition;
    //     Vector2Int IntCoordinate = new Vector2Int((int)(mousePos.x / 2), (int)(mousePos.y / 2));
    //     if (_dijkstraRange.Contains(IntCoordinate))
    //     {
    //         Debug.Log("move");
    //         transform.position = new Vector3(coord.x, coord.y);
    //     }
        
    // }

    public void SetCostMap(int[,] map)  //这个是让别人来改动这个costMap的
    {
        _pathFinder.map = map;
    }

}
