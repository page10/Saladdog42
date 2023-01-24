using UnityEngine;
public struct CharacterObject
{
    public GridPosition gPos;
    public SlaveState slaveTo;
    public AnimatorController animator;

    public GameObject gameObject {
        get =>
            gPos != null ? gPos.gameObject:
            slaveTo != null ? slaveTo.gameObject:
            animator != null ? animator.gameObject:
            null;
        
    }

    public CharacterObject(GridPosition gPos, SlaveState slaveTo, AnimatorController animator)
    {
        this.gPos = gPos;
        this.slaveTo = slaveTo;
        this.animator = animator;
    }
}