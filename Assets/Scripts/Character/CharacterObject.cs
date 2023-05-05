using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterObject : MonoBehaviour
{
    public GridPosition gPos;
    public SlaveState slaveTo;
    public AnimatorController animator;
    public CharacterAttack attack;
    public CharacterStatus Status = new CharacterStatus();

    //todo 角色的名字，创建的时候可以随机生成一个
    public string characterName = "";
    
    public int hp;
    public bool CanBeDestroyed { get; set; } = false;
    

    private void Awake()
    {
        hp = Status.hp;  // 不知道hp在这里初始化对不对呢
        gPos = GetComponent<GridPosition>();
        slaveTo = GetComponent<SlaveState>();
        animator = GetComponentInChildren<AnimatorController>();
        attack = GetComponent<CharacterAttack>();
        // debug 之后这里要传入参数生成不同种类的
        Status = GetInitStatusByType(MoveType.Land);
    }
    
    public bool IsEnemy(CharacterObject target)
    {
        // 这部分的逻辑之后需要根据设计情况改
        return (target.slaveTo.masterPlayerIndex != slaveTo.masterPlayerIndex);
    }

    public bool IsAlly(CharacterObject target)
    {
        // 这部分的逻辑之后需要根据设计情况改
        return (target.slaveTo.masterPlayerIndex == slaveTo.masterPlayerIndex);
    }

    public byte GetRelation(CharacterObject target)
    {
        byte res = 0;
        if (IsEnemy(target))
        {
            res |= Constants.TargetType_Foe;
        }
        else if (IsAlly(target))
        {
            res |= Constants.TargetType_Ally;
        }
        if (target.gameObject == this.gameObject)
        {
            res |= Constants.TargetType_Self;
        }

        return res;
    }

    private CharacterStatus GetInitStatusByType(MoveType type)
    {
        // todo: 这里之后要写读表
        CharacterStatus res = new CharacterStatus(9, 9, 3, 9, 30, 100, 10, 90, 80);
        return res;
    }
}
