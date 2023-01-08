using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DijkstraMoveInfo{
    //当前的格子
    public Vector2Int position;

    //来自哪个格子
    public Vector2Int from;

    //剩余移动力
    public int cost;

    public DijkstraMoveInfo(Vector2Int position, Vector2Int from, int cost){
        this.position = position;
        this.from = from;
        this.cost = cost;
    }

    public static DijkstraMoveInfo Invalid(){
        return new DijkstraMoveInfo(Vector2Int.one * -1, Vector2Int.one * -1, -1);
    }

    public bool IsValid(){
        return cost >= 0;
    }

    public static bool operator ==(DijkstraMoveInfo left, DijkstraMoveInfo right){
        return left.position == right.position;
    }
    public static bool operator !=(DijkstraMoveInfo left, DijkstraMoveInfo right){
        return left.position != right.position;
    }
}

public class Dijkstra 
{
    //地图移动力信息
    public int[,] map;

    private List<DijkstraMoveInfo> open = new List<DijkstraMoveInfo>();
    private List<DijkstraMoveInfo> close = new List<DijkstraMoveInfo>();

    public Dijkstra(){}

    /// <summary>
    /// 获得可以移动的单元格
    /// <param name="cost">移动力多少</param>
    /// <param name="startGrid">寻路起点坐标</param>
    /// </summary>
    /// <returns>所有可以达到的单元格</returns>
    public List<DijkstraMoveInfo> GetCanMoveGrids(int cost, Vector2Int startGrid){
        if (!GridValid(startGrid)){
            return new List<DijkstraMoveInfo>();
        }
        
        close.Clear();
        open.Clear();
        close.Add(new DijkstraMoveInfo(startGrid, startGrid, cost));

        if (cost <= 0){
            return close;
        }

        open.Add(new DijkstraMoveInfo(startGrid, startGrid, cost));

        while(open.Count > 0){
            int restCost = open[0].cost;
            for (int i = -1; i <= 1; i++){
                for (int j = -1; j <= 1; j++){
                    int gX = open[0].position.x + i;
                    int gY = open[0].position.y + j;
                    if (Mathf.Abs(i) + Mathf.Abs(j) == 1 && GridValid(gX, gY) && restCost >= map[gX, gY]){
                        DijkstraMoveInfo inClose = GetInfoInClose(gX, gY);
                        int costWhileMoved = restCost - map[gX, gY];
                        DijkstraMoveInfo thisMoveInfo = 
                            new DijkstraMoveInfo(new Vector2Int(gX, gY), open[0].position, costWhileMoved);
                        if (inClose.IsValid()){
                            if (inClose.cost < costWhileMoved){
                                close.Remove(inClose);
                                close.Add(thisMoveInfo);
                                open.Add(thisMoveInfo);
                            }
                        }else{
                            close.Add(thisMoveInfo);
                            open.Add(thisMoveInfo);
                        }
                      
                    }
                }
            }
            open.RemoveAt(0);
        }

        return close;
    }

    public bool GridValid(Vector2Int grid){
        return !(grid.x < 0 || grid.y < 0 || grid.x >= map.GetLength(0) || grid.y >= map.GetLength(1));
    }

    public bool GridValid(int gridX, int gridY){
        return !(gridX < 0 || gridY < 0 || gridX >= map.GetLength(0) || gridY >= map.GetLength(1));
    }

    // 根据Pos来拿到Close列表中对应的内容
    private DijkstraMoveInfo GetInfoInClose(Vector2Int pos){
        for (int i = 0; i < close.Count; i++){
            if (close[i].position == pos){
                return close[i];
            }
        }
        return DijkstraMoveInfo.Invalid();
    }
    // 根据Pos来拿到Close列表中对应的内容
    private DijkstraMoveInfo GetInfoInClose(int gX, int gY){
        for (int i = 0; i < close.Count; i++){
            if (close[i].position.x == gX && close[i].position.y == gY){
                return close[i];
            }
        }
        return DijkstraMoveInfo.Invalid();
    }
}
