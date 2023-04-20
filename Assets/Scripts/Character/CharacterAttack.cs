using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{

    public int weaponCurIndex { get; set; }
    private List<WeaponObj> weapons = new List<WeaponObj>();
    public  List<WeaponObj> Weapons {get => weapons; set => weapons = value == null ? value : new List<WeaponObj>(); }

    /// <summary>
    /// 添加武器 并且返回是否添加成功
    /// </summary>
    public bool AddWeapon(WeaponObj weapon)
    {
        if (weapons.Count < Constants.weaponSlots)
        {
            weapons.Add(weapon);
            return true;
        }
        else return false;        
    }

    /// <summary>
    /// 获得所有武器攻击范围的最小值
    /// <param name="target"></param> 武器生效方  
    /// </summary>
    public int GetWeaponMinRange(byte target)
    {
        if (weapons.Count == 0) return Constants.nullWeaponIndex;
        int minRange = 0;
        for (int i = 0; i < weapons.Count; i++)
        {
            if (minRange > weapons[i].minRange && target == weapons[i].target)
            {
                minRange = weapons[i].minRange;
            }
        }
        return minRange;
    }

    /// <summary>
    /// 获得所有武器攻击范围的最大值
    /// <param name="target"></param> 武器生效方  
    /// </summary>
    public int GetWeaponMaxRange(byte target)
    {
        if (weapons.Count == 0) return Constants.nullWeaponIndex;
        int maxRange = 0;
        for (int i = 0; i < weapons.Count; i++)
        {
            if (maxRange < weapons[i].maxRange && target == weapons[i].target)
            {
                maxRange = weapons[i].maxRange;
            }
        }
        return maxRange;
    }

    /// <summary>
    /// 获得所有的可触及范围格子
    /// <param name="gridPos">检查的范围的中心</param> 
    /// <param name="mapSize">地图宽高 避免得到地图外格子</param> 
    /// <param name="target">触及对象的类型</param> 
    /// </summary>
    public List<Vector2Int> GetAttackRange(Vector2Int gridPos, Vector2Int mapSize, byte target)  
    {
        List<Vector2Int> atkRange = new List<Vector2Int>();
        
        int curMinRange = GetWeaponMinRange(target);  // 对敌方使用的武器的最小范围
        int curMaxRange = GetWeaponMaxRange(target);  // 对敌方使用的武器的最大范围

        for (int i = -curMaxRange; i <= curMaxRange; i++)
        {
            for (int j = -curMaxRange; j <= curMaxRange; j++)
            {
                Vector2Int addGrid = new Vector2Int();
                addGrid.x = gridPos.x + i;
                addGrid.y = gridPos.y + j;

                if (addGrid.x < mapSize.x && 
                    addGrid.y < mapSize.y && 
                    addGrid.x >= 0 && addGrid.y >= 0 &&
                    (Mathf.Abs(i) + Mathf.Abs(j)) <= curMaxRange &&  
                    (Mathf.Abs(i) + Mathf.Abs(j)) >= curMinRange 
                    )  
                    {
                        atkRange.Add(addGrid);
                    }
            }
        }
        return atkRange;
    }

    /// <summary>
    /// 获得各武器的攻击或治疗范围
    /// <param name="gridPos">检查的范围的中心</param> 
    /// <param name="mapSize">地图宽高 避免得到地图外格子</param> 
    /// </summary>
    public List<CoveredRange> GetWeaponRange(Vector2Int gridPos, Vector2Int mapSize)
    {
        List<CoveredRange> weaponRange = new List<CoveredRange>();

        int WeaponRangeIndex(Vector2Int checkPos){
            for(int _i = 0; _i < weaponRange.Count; _i++)
            {
                if (weaponRange[_i].gridPos == checkPos) return _i;
            }
            return -1;
        }

        foreach (WeaponObj weapon in weapons)
        {
            for (int i = -weapon.maxRange; i <= weapon.maxRange; i++)
            {
                for (int j = -weapon.maxRange; j <= weapon.maxRange; j++)
                {
                    Vector2Int addGrid = new Vector2Int();
                    addGrid.x = gridPos.x + i;
                    addGrid.y = gridPos.y + j;

                    if (addGrid.x < mapSize.x &&
                        addGrid.y < mapSize.y &&
                        addGrid.x >= 0 && addGrid.y >= 0 &&
                        (Mathf.Abs(i) + Mathf.Abs(j)) <= weapon.maxRange &&
                        (Mathf.Abs(i) + Mathf.Abs(j)) >= weapon.minRange
                       )
                    {
                        int index = WeaponRangeIndex(addGrid);
                        if (index >= 0)
                        {
                            weaponRange[index] = CoveredRange.MixedType(weaponRange[index], weapon.target);
                        }
                        else
                        {
                            weaponRange.Add(new CoveredRange(weapon.target, addGrid));
                        }
                    }
                }
            }
        }


        return weaponRange;
    }

}
