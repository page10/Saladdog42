using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiConditions
{
    public static Dictionary<string, AICondition> AIConditionsDict = new Dictionary<string, AICondition>()
    {
        // 每加一个脚本就要在这里注册
        {"Always", Always},
        {"hasAttackableEnemy",hasAttackableEnemy},
        {"hasAttackableAlly",hasAttackableAlly}
    };

    /// <summary>
    /// 总是通过的
    /// </summary>
    /// <param name="characterObj"></param>
    /// <returns></returns>
    public static bool Always(in CharacterObject characterObj)
    {
        return true;
    }
    
    /// <summary>
    /// 判定攻击范围内是不是有敌人
    /// </summary>
    /// <param name="characterObj">攻击执行者</param>
    /// <returns>攻击范围内是不是有敌人</returns>
    public static bool hasAttackableEnemy(in CharacterObject characterObj)
    {
        return false;   //todo test
        return (GameState.GetAttackableCharacters(characterObj) & Constants.TargetType_Foe) != 0;
    }
    
    /// <summary>
    /// 判定治疗范围内是否有可以治疗的队友
    /// </summary>
    /// <param name="characterObj"></param>
    /// <returns></returns>
    public static bool hasAttackableAlly(in CharacterObject characterObj)
    {
        return false;   //todo test
        return (GameState.GetAttackableCharacters(characterObj) & Constants.TargetType_Ally) != 0;
    }
}