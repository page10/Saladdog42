using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour  //监听animator 根据它的状态和角色状态决定要做哪些动作
{
    private Animator animator;
    private MoveByPath moveByPath;  
    private GameObject mask;
    private bool startedMove = false;
    private bool finishedMove = false;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        moveByPath = GetComponent<MoveByPath>();

        //debug 
        mask = GetComponentInChildren<MovementMask>().gameObject;
    }

    private void Update() {
        if (startedMove && moveByPath != null) {
            if (moveByPath.IsMoving == false) {
                FinishMove();
            }
        }
    }

    public void StartMove(List<Vector2Int> path)
    {
        if (moveByPath)
        {
            moveByPath.StartMove(path);
        }
        startedMove = true;
    }

    public bool IsMoveFinished(bool needReset = true)
    {
        bool finished = finishedMove & startedMove;
        if (needReset)
        {
            startedMove = finishedMove = false;
            
        }
        return finished;
    }

    private void FinishMove()
    {
        finishedMove = true;
        mask.SetActive(true);
    }

    public void NewTurn()
    {
        mask.SetActive(false);
    }
}
