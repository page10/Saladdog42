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
    
    public string characterName = "";
    
    public CharacterResource CharacterResource = new CharacterResource();  // todo hp修改要用这个
    public bool CanBeDestroyed { get; set; } = false;
    
    // 移动和攻击分开算用的标签 
    public bool hasMoved = false;
    public bool hasAttacked = false;

    public AI characterAi;
    

    private void Awake()
    {
        CharacterResource.hp = Status.hp;  // 不知道hp在这里初始化对不对呢
        gPos = GetComponent<GridPosition>();
        slaveTo = GetComponent<SlaveState>();
        animator = GetComponentInChildren<AnimatorController>();
        attack = GetComponent<CharacterAttack>();
        // debug 之后这里要传入参数生成不同种类的
        Status = GetInitStatusByType(MoveType.Land);
        characterAi = GetComponent<AI>();  // 用的时候判断一下有没有 我方就是没有挂这个ai的
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
        CharacterStatus res = new CharacterStatus("emptyID",9, 9, 3, 9,  10, 90, 80);
        return res;
    }

    /// <summary>
    /// 移动动画是否播放完毕了
    /// </summary>
    /// <returns></returns>
    public bool IsMovingAnimDone()
    {
        Debug.Log(">>>>>Check Moving done");
        MoveByPath moveByPath = gameObject.GetComponent<MoveByPath>();
        return (moveByPath == null || moveByPath.IsMoving == false); //这次移动移动完成
    }
}
