using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这是逻辑地形块的“身份证”
/// 现在Prefab中的Tile_Template，是一个过于抽象的东西，他是对的，我们可以基于他来创建，并给它赋值
/// 这在正常的游戏开发环境下都是正确的逻辑
/// 但是我们现在用了Unity，所以我们得定义每一个地形都是一个Tile_Template，这样地编同学就可以通过这个来编辑地图
/// </summary>
public class TerrainTile : MonoBehaviour 
{
    //这些策划定义的内容应当放在一个结构体或者数组中，以便扩展
    // public int LandMovementConsume { get; set; }  // movement consumption of terrain for land units
    // public int AirMovementConsume { get; set; }  // movement consumption of terrain for airborne units
    // public int RidingMovementConsume { get; set; }   // movement consumption of terrain for riding units
    // public float DefenseModifier { get; set; }   // defense modifier, percentage 
    // public float AttackModifier { get; set; }   // attack modifier, percentage
    // public float MDefenseModifier { get; set; }   // magic defense modifier, percentage
    // public float DodgeModifier { get; set; }   // dodge modifier, percentage
    
    /// <summary>
    /// 地形移动用字典即可，可以约定没有的值=最大值
    /// 在游戏领域，这叫Cost
    /// </summary>
    [Tooltip("地形的移动力消耗，没有设置的地形就是不可过地形")]
    public MovementInfo[] costs; 

    /// <summary>
    /// 角色属性百分比变化器
    /// 如果还需要直接改写的，那么这里应该有2条这个结构，就如int HpPlus和 float HpTimes一样道理
    /// 只是float在这里得按照约定来int HpTimes，仅仅人类概念上是float而已。
    /// </summary>
    [Tooltip("地形上的角色属性百分比变化（+多少%），没有设置的就是0变化")]
    public CharacterStatus percentageModifer;

    /// <summary>
    /// 这个单元格的地图信息是什么，也就是他是草地还是水
    /// </summary>
    [Tooltip("这是什么地形")] public TerrainType terrain;

    /// <summary>
    /// 4个角落的贴图引用，便于管理
    /// </summary>
    private RenderTerrainTile[,] _subTiles = new RenderTerrainTile[2,2];

    /// <summary>
    /// 设置每一个子块，也就是4个角落的贴图
    /// </summary>
    /// <param name="part">4个角落用Scale13的哪一个</param>
    public void SetSubTile(Scale13[,] part)
    {
        //这个函数里的数组2x2的2都是写死的数据，是因为这个自然规则就是2，无法改变这个值，不然应当定义变量或常量
        
        //异常处理，传进来的不是一个2x2的，就return了
        if (part.GetLength(0) != 2 || part.GetLength(1) != 2) return;
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //丢掉老的
                if (_subTiles[i, j] != null) Destroy(_subTiles[i, j].gameObject);
                
                //创建新的
                GameObject go = Instantiate(terrain.PartPrefab(part[i, j]));
                if (go)
                {
                    _subTiles[i, j] = go.GetComponent<RenderTerrainTile>();
                    go.transform.SetParent(transform);
                    go.transform.localPosition = new Vector3(
                        i * Constants.tileSize * 0.500f,
                        (1 - j) * Constants.tileSize * 0.500f       //unity和全世界相反
                    );
                }
            }
        }
    }
}


/// <summary>
/// 移动类型
/// 游戏中角色的移动类型和地形的移动类型在这里定义
/// 这里是策划设计玩法的时候决定的，有哪些异动类型
/// </summary>
public enum MovementType
{
    Land,       //地面
    Air,        //空中
    Mount,  //骑马（游戏领域用mount不是ride）
}

/// <summary>
/// 我们需要一个移动类型和移动力的结构
/// 它用来记录地形的消耗，兵种的移动力等信息
/// </summary>
[Serializable]
public struct MovementInfo
{
    public MovementType movementType;
    public int value;

    public MovementInfo(MovementType movementType, int value)
    {
        this.movementType = movementType;
        this.value = value;
    }
}

/// <summary>
/// 角色的属性数据结构
/// 所有的角色属性的地方，包括地形改变、角色的基础属性、装备的角色属性等都是这个
/// 具体内容要由策划来设计，这个游戏中角色需要的属性，数值和系统一起的
/// 角色属性等游戏运算属性必须是int，float虽然比较流行（很偷懒）但是外行做法
/// 在需要用到float的地方，比如百分比的地方，这里需要精确到第几位：
/// 比如10000代表1%（约定），那么4320代表0.432%，这个约定在游戏内应该统一
/// 之所以启用结构体，是因为这个内容策划很容易进行改动，并且抛接口给策划，应该给的是这个结构体
/// </summary>
[Serializable]
public struct CharacterStatus
{
    public int attack;
    public int defense;
    public int mDefense;
    public int dodge;

    public CharacterStatus(int attack, int defense, int mDefense, int dodge)
    {
        this.attack = attack;
        this.defense = defense;
        this.mDefense = mDefense;
        this.dodge = dodge;
    }
}

/// <summary>
/// 这个数据实际上是一个KeyValue数据，允许我们给预设的地形Prefab在某个地图内进行命名
/// 毕竟地图A的“水”和地图B的“水”未必一样对吧，包括美术和逻辑都可能不一样
/// 因此当我们把一个MpaGenerator看做是一个地图（关卡）的时候，那么这个关卡所使用的各种信息
/// 就可以在这个结构定义，当然，严格的说，这是一个中间结构，为了人和计算机搭桥梁的
/// </summary>
[Serializable]
public struct NamedTerrainTile
{
    [Tooltip("地图块的名称")] public string name;

    [Tooltip("地图块")] public TerrainTile terrainTile;
}