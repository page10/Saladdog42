using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiConditions
{
    /// <summary>
    /// 判定攻击范围内是不是有敌人
    /// </summary>
    /// <param name="characterObj">攻击执行者</param>
    /// <returns>攻击范围内是不是有敌人</returns>
    public static bool hasAttackableEnemy(CharacterObject characterObj)
    {
        return (GameState.GetAttackableCharacters(characterObj)|Constants.TargetType_Foe) != 0;
        

    }
}