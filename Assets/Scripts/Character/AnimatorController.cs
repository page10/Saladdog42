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
    //private GameObject mask;
    private bool startedMove = false;  
    public bool finishedMove = false;
    private SpriteRenderer spriteRenderer;
    

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        moveByPath = GetComponent<MoveByPath>();

        //debug 
        //mask = GetComponentInChildren<MovementMask>().gameObject;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() {  //监听某些状态 
        if (startedMove && moveByPath != null) {
            if (moveByPath.IsMoving == false) {
                FinishMove();  //「移动完了」是个概括 所以是个函数 一般人类说话里的概括就是函数 
            }
            else
            {
                if (animator)
                {
                    animator.SetFloat("DirectionX", moveByPath.MovingDirection.x);
                    animator.SetFloat("DirectionY", moveByPath.MovingDirection.y);
                }
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
        if (animator)
        {
            animator.SetBool( "Moving", true);
        }
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
        //Debug.Log("finishedMove");
        finishedMove = true;
        //mask.SetActive(true);
        if (animator)
        {
            animator.SetBool( "Moving", false);
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", -1);
        }

    }

    public void FinishMovement()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.yellow;
        }
    }

    /// <summary>
    /// 这一回合主动动作做完了
    /// 蒙上一层灰色蒙版
    /// </summary>
    public void FinishAction()
    {
        //Debug.Log("finishedAction");
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.gray;
        }
    }
    

    public void NewTurn()
    {
        //mask.SetActive(false);
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.white;
        }
        startedMove = finishedMove = false;
        if (animator)
        {
            animator.SetBool( "Moving", false);
            animator.SetFloat("DirectionX", 0);
            animator.SetFloat("DirectionY", -1);
        }

    }

    public void PlayAnimation(string animationName)
    {
        if (animator)
        {
            animator.SetTrigger(animationName);
        }
    }

    public void RemoveCharacter()
    {
        if (spriteRenderer) spriteRenderer.gameObject.SetActive(false);
    }
    
    public void SetFaceDirection(Vector2Int direction)
    {
        if (animator)
        {
            animator.SetFloat("DirectionX", direction.x);
            animator.SetFloat("DirectionY", direction.y);
        }
    }


    //写component时候分析 这是不是它该干的活
    //如果不是 就暴露一个接口 
    //查询类 暴露出去给别人看 功能类 能干点什么 暴露出去让别人用 tick类 内部循环 不断做一些事情 不会给别人的
}
