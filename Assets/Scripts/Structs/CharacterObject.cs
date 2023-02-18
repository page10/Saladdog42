using UnityEngine;
public struct CharacterObject
{
    public GridPosition gPos;
    public SlaveState slaveTo;
    public AnimatorController animator;
    public CharacterAttack attack;

    public GameObject gameObject {
        get =>
            gPos != null ? gPos.gameObject:
            slaveTo != null ? slaveTo.gameObject:
            animator != null ? animator.gameObject:
            null;
        
    }

    public CharacterObject(GridPosition gPos, SlaveState slaveTo, AnimatorController animator, CharacterAttack attack)
    {
        this.gPos = gPos;
        this.slaveTo = slaveTo;
        this.animator = animator;
        this.attack = attack;
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
}