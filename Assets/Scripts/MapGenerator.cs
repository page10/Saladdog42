using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Texture2D[] waterTextures;  // 先只弄水 等加森林的时候 在这里加一个list 加的时候加一个判定看用哪个list
    public RectTransform tile_Template;
    public RectTransform tile_Grass;
    public TerrainTile[,] tileMap = new TerrainTile[Constants.maxTilesX, Constants.maxTilesY];
    TerrainType[,] typeMap = new TerrainType[Constants.maxTilesX, Constants.maxTilesY];  // 这个是不是可以不要？
    Dictionary<Vector2Int, RectTransform> presentWaterTiles = new Dictionary<Vector2Int, RectTransform>();
    public int TileSize
    {
        get { return Constants.tileSize; }
    }
      enum TileParts
    {
        LU,
        RU,
        LD,
        RD,
        count
    };

    void Start()
    {
        GenerateMapTypes();
        typeMap[2, 4] = typeMap[3, 3] = TerrainType.Grassland;  // 插一块草感受一下

        for(int x = 0; x < Constants.maxTilesX; x++)
        {
            for (int y = 0; y < Constants.maxTilesY; y++)
            {
                if (typeMap[x, y] == TerrainType.Water)
                {
                    BuildWaterTerrain(x, y);
                }
                else if (typeMap[x, y] == TerrainType.Grassland)
                {
                    BuildGrassTerrain(x, y);
                }
                
            }
        }
    }
    private void GenerateMapTypes()  // Generate different types of tiles 
    {
        for (int x = 2; x < Constants.maxTilesX - 2; x++)
        {
            for (int y = 2; y < Constants.maxTilesY - 2; y++)
            {
                typeMap[x, y] =  TerrainType.Water;
            }
        }
        
    }
    private void BuildGrassTerrain(int x, int y) 
    {
        var newTile = Instantiate(tile_Grass.gameObject).GetComponent<RectTransform>();           
        newTile.SetParent(tile_Grass.parent);
        newTile.anchoredPosition = new Vector2(x * TileSize, y * TileSize);
    }

    private void BuildWaterTerrain(int x, int y) 
    {
        var newTile = Instantiate(tile_Template.gameObject).GetComponent<RectTransform>();           
        newTile.SetParent(tile_Template.parent);
        newTile.anchoredPosition = new Vector2(x * TileSize, y * TileSize);
        Vector2Int tileIndex = new Vector2Int(x, y);
        presentWaterTiles.Add(tileIndex, newTile);
        RefreshWaterTexture();

    }
    
    private void RefreshWaterTexture()
    {
       foreach (var tile in presentWaterTiles)
        {
            for (int i = 0; i < (int)TileParts.count; i++)
            {
                TileParts part = (TileParts)i;
                tile.Value.Find(part.ToString()).GetComponent<UnityEngine.UI.RawImage>().texture
                    = waterTextures[GetExpectElementIndex(tile.Key,part)];
            }
        }
    }

    int GetExpectElementIndex(Vector2Int TileIndex, TileParts part)
    {
        var NeighbourFilled = new System.Func<Vector2Int,bool >((NeighbourOffset) =>
        {
            return presentWaterTiles.ContainsKey(TileIndex + NeighbourOffset);
        });

        if(part==TileParts.LU)
        {
            bool up = NeighbourFilled(new Vector2Int(0, 1));
            bool left = NeighbourFilled(new Vector2Int(-1, 0));
            bool up_left = NeighbourFilled(new Vector2Int(-1, 1));
            if (up && left)//上和左都有
            {
                if (up_left) //左上也有
                    return 19;
                else                                        //左上没有
                    return 5;
            }
            else if (!(up || left))//上和左都没有
                return 9;
            else    //左和上有一者有
            {
                if (up)         //上
                    return 17;
                else            //左
                    return 11;
            }
        }
        if (part == TileParts.RU)
        {
            bool up = NeighbourFilled(new Vector2Int(0, 1));
            bool right = NeighbourFilled(new Vector2Int(1, 0));
            bool up_right = NeighbourFilled(new Vector2Int(1, 1));
            if (up && right)//上和右都有
            {
                if (up_right) //右上也有
                    return 18;
                else                                        //右上没有
                    return 6;
            }
            else if (!(up || right))//上和右都没有
                return 12;
            else    //右和上有一者有
            {
                if (up)         //上
                    return 20;
                else            //右
                    return 10;
            }
        }
        if (part == TileParts.LD)
        {
            bool down = NeighbourFilled(new Vector2Int(0, -1));
            bool left = NeighbourFilled(new Vector2Int(-1, 0));
            bool down_left = NeighbourFilled(new Vector2Int(-1, -1));
            if (down && left)//下和左都有
            {
                if (down_left) //左下也有
                    return 15;
                else                                        //左下没有
                    return 7;
            }
            else if (!(down || left))//下和左都没有
                return 21;
            else    //左和下有一者有
            {
                if (down)         //下
                    return 13;
                else            //左
                    return 23;
            }
        }
        if (part == TileParts.RD)
        {
            bool down = NeighbourFilled(new Vector2Int(0, -1));
            bool right = NeighbourFilled(new Vector2Int(1, 0));
            bool down_right = NeighbourFilled(new Vector2Int(1, -1));
            if (down && right)//下和右都有
            {
                if (down_right) //右下也有
                    return 14;
                else                                        //右下没有
                    return 8;
            }
            else if (!(down || right))//下和右都没有
                return 24;
            else    //右和下有一者有
            {
                if (down)         //下
                    return 16;
                else            //右
                    return 22;
            }
        }

        return 0;
    }
}
