using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 抽象一件事情 可以给其他很多东西 是做一个component的动机
// 通过别的状态 控制animator播放什么东西 是专门做这个事情的 
// 把依赖的东西都private调用进来

public class AnimatorController : MonoBehaviour  //监听animator 根据它的状态和角色状态决定要做哪些动作
{
    private Animator animator;
    private MoveByPath moveByPath;  
    private GameObject mask;
    private bool startedMove = false;  
    public bool finishedMove = false;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        moveByPath = GetComponent<MoveByPath>();

        //debug 
        mask = GetComponentInChildren<MovementMask>().gameObject;
    }

    private void Update() {  //监听某些状态 
        if (startedMove && moveByPath != null) {
            if (moveByPath.IsMoving == false) {
                FinishMove();  //「移动完了」是个概括 所以是个函数 一般人类说话里的概括就是函数 
            }
            //Time.deltaTime 是两帧update之间的时间间隔
            //Fixeddeltatime 是两个逻辑帧之间的时间间隔 按照tick来作为最小单位（int）
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

    public bool IsMoveFinished(bool needReset = true)  //动画是不是播放完了 
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
        startedMove = finishedMove = false;

    }

    //写component时候分析 这是不是它该干的活
    //如果不是 就暴露一个接口 
    //查询类 暴露出去给别人看 功能类 能干点什么 暴露出去让别人用 tick类 内部循环 不断做一些事情 不会给别人的
}
