using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiActions
{
    /// <summary>
    /// 移动到攻击范围内最近的敌人
    /// </summary>
    /// <param name="characterObj">移动执行者</param>
    public static void moveToNearestEnemy(CharacterObject characterObj)
    {
        Debug.Log("move to nearest enemy!");
    }

    /// <summary>
    /// 攻击敌人
    /// </summary>
    /// <param name="characterObj">攻击执行者</param>
    public static void attackEnemy(CharacterObject characterObj)
    {
        Debug.Log("attack enemy!");
    }
}