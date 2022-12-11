using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 渲染所用到的RenderTerrainTile的“身份证”
/// 【注意】这只是渲染的地块，也就是13宫图的内容
/// 但是用统一的template我们就需要自制一个编辑器，比如让美术直接用unity作为地图编辑器
/// 所以我们可以要求每一个地形块（元素）都是一个Prefab，包括13宫图的每一块都是一个单独的Prefab
/// 这样虽然看起来我们同一个地形要设置13次一样的值，但是规范了许多
/// 当然设置值的麻烦，只是input层（编辑器问题），核心功能代码层不为编辑器层的问题买单
/// 值得一提的是，虽然这个东西可以看起来完全没有，但是我们还是给了一个“身份证”
/// 并不是为了平添麻烦，而是当后期需要追加一些数据信息的时候，这个“空”的class才会有价值
/// </summary>
public class RenderTerrainTile : MonoBehaviour
{
    
}