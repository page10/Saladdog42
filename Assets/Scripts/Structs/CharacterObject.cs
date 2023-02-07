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
}