using UnityEngine;

public static class Constants 
{
     public static ushort maxTilesX = 20;
     public static ushort maxTilesY = 10;
     //public static ushort tileSize = 48;  //之后这里换了图记得改

     /// <summary>
     /// 地图单元格我们按照1米x1米来
     /// 值得一提的是，他是2D的，但不能放在Canvas下，这在Unity是不对的
     /// Unity的Canvas下放的是UI，所以大多是2D的，但不是2D的都放在Canvas下
     /// 作为游戏的元素，地图单元格、角色等都得放在游戏世界下，所以用米作为单位
     /// </summary>
     public static float tileSize = 1f;      
}
