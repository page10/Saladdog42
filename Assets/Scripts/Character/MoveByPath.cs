using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByPath : MonoBehaviour
{
    private bool isMoving = false;  // initialize 
    public bool IsMoving { get => isMoving; }
    public float moveSpeed = 400.00f;
    private List<PositionAndGrid> pathNodes = new List<PositionAndGrid>();  //只需要被传 不一定需要被别人知道


    public void StartMove(List<Vector2Int> path)
    {
        if (path == null) return;
        pathNodes.Clear();

        Vector2Int direction = Vector2Int.zero;
        if (path.Count > 1)
        {
            direction = path[1] - path[0];
        }
        else{
            return; 
        }

        for (int i = 1; i < path.Count; i++)  // 自己那格不用加进移动路径里去
        {
            Vector2Int currentDirection = i >= path.Count - 1 ? direction : new Vector2Int(path[i + 1].x - path[i].x, path[i + 1].y - path[i].y);
            if (((currentDirection.x == 0 && direction.x == 0) || (currentDirection.y == 0 && direction.y == 0)) && i < path.Count - 1)  continue;  //不把一条线上的加进路径节点里
            pathNodes.Add(
                new PositionAndGrid(
                    new Vector2(path[i].x * Constants.tileSize, path[i].y * Constants.tileSize),
                    path[i]
                )
            );
            direction = currentDirection;
        }
        Debug.Log("pathNodes length : " + pathNodes.Count);

        isMoving = true;

    }

    private void Update()
    {  // 渲染功能 所以写在update里 和逻辑没啥关系 只是表现
        if (isMoving == false) return;

        if (pathNodes.Count <= 0)
        {
            isMoving = false;
            return;
        }

        float thisTickMoveSpeed = Time.deltaTime * moveSpeed;
//        Debug.Log("thisTickMoveSpeed : " + thisTickMoveSpeed);
//        Debug.Log("deltaTime: " + Time.deltaTime);
        if (Mathf.Abs(transform.position.x - pathNodes[0].position.x) + Mathf.Abs(transform.position.y - pathNodes[0].position.y) <= thisTickMoveSpeed)
        {
            transform.position = new Vector3(pathNodes[0].position.x, pathNodes[0].position.y);
            GridPosition gridPosition = GetComponent<GridPosition>();
            if (gridPosition != null)
            {
                gridPosition.grid = pathNodes[0].grid;
            }
            pathNodes.RemoveAt(0);
            return;
        }
        int xDir = Mathf.Abs(transform.position.x - pathNodes[0].position.x) < thisTickMoveSpeed? 0 : 
        (pathNodes[0].position.x > transform.position.x? 1: -1);
        int yDir = Mathf.Abs(transform.position.y - pathNodes[0].position.y) < thisTickMoveSpeed? 0 : 
        (pathNodes[0].position.y > transform.position.y? 1: -1);

        transform.position += new Vector3(
            xDir * thisTickMoveSpeed,
            yDir * thisTickMoveSpeed
        );

    }
}

public struct PositionAndGrid
{
    public Vector2 position;
    public Vector2Int grid;

    public PositionAndGrid(Vector2 position, Vector2Int grid)
    {
        this.position = position;
        this.grid = grid;
    }
}
