using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    private CharacterPosition _position;
    private void GetMoveRange(CharacterMovement charMove)
    {
        if (!_mapGenerator || !charMove) return;

        int width =_mapGenerator.Map.GetLength(0);
        int height = _mapGenerator.Map.GetLength(1);
        int[,] costMap = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costMap[x, y] = _mapGenerator.Map[x, y].Cost(charMove.moveType);
            }
        }
        charMove.SetCostMap(costMap);
        Vector2Int pos = _position.charactertPosition;
        charMove.GetDijkstraRange(pos);
    }

    public bool clicked = false;
    private void FixedUpdate() {
        if (Input.GetMouseButton(0) && clicked == false)  //input不能发生在CharacterMovement里面 之后有专门的inputManager 
			{
                Debug.Log("mouseClicked");  //还得写一个鼠标点击位置选中角色的东西
                clicked = true;
                //GetMoveRange();
			}
            else
            {
                clicked = false;
            }
    }
}
