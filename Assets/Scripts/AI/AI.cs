using System;
using System.Collections.Generic;
using UnityEngine;


    public class AI : MonoBehaviour
    {
        private CharacterObject character;
        private List<AIClip> _clips;
        

        private void Awake()
        {
            character = GetComponent<CharacterObject>();
        }

        public void DoAI()
        {
            for (int i = 0; i < _clips.Count; i++)
            {
                bool meet = true;
                foreach (AICondition aiCondition in _clips[i].Conditions)
                {
                    if (!aiCondition(character))
                    {
                        meet = false;
                        break;
                    };
                }

                if (meet)
                {
                    for (int j = 0; j < _clips[i].Actions.Count; j++)
                    {
                        _clips[i].Actions[j](character);
                    }
                }
            }
        }
        // public AICondition hasAttackableEnemy = (characterObj) =>
        // {
        //     List<CharacterObject> attackableEnemies = characterObj.attack.GetAttackableEnemies();
        //     // 总觉得之前有写过判定攻击范围内是不是有敌人的方法呢……
        //     return attackableEnemies.Count > 0;
        // };
        
    }

    public struct AIClip
    {
        public List<AICondition> Conditions;
        public List<AIAction> Actions;
    }
// todo 先写死一套ai逻辑绑在敌人身上 大概就是 如果攻击范围内有敌人 就移动并攻击血量最少的敌人 否则待机

    public delegate bool AICondition(CharacterObject characterObj);

    public delegate void AIAction(CharacterObject characterObj);


