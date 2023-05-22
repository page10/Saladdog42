using System;
using System.Collections.Generic;
using UnityEngine;


    public class AI : MonoBehaviour
    {
        private CharacterObject character;
        private List<AIClip> _moveAiClips;  // 移动相关的ai
        private List<AIClip> _attackAiClips;  // 攻击相关的ai

        private void Awake()
        {
            character = GetComponent<CharacterObject>();
        }

        /// <summary>
        /// 找到并返回第一个可用的aiclips
        /// </summary>
        public AIClip GetAvailableMoveAI(CharacterObject characterObj)
        {
            for (int i = 0; i < _moveAiClips.Count; i++)
            {
                bool meet = true;
                foreach (AICondition aiCondition in _moveAiClips[i].Conditions)
                {
                    if (!aiCondition(characterObj))
                    {
                        meet = false;
                        break;
                    };
                }

                if (meet)
                {
                    return _moveAiClips[i];
                }
            }

            return new AIClip();
        }
        
        /// <summary>
        /// 执行aiClip中的actions
        /// </summary>
        /// <param name="aiClip">要执行的那个aiClip</param>
        public void ExecuteAI(AIClip aiClip)
        {
            for (int j = 0; j < aiClip.Actions.Count; j++)
            {
                aiClip.Actions[j](character);
            }
        }

        public void DoAI()
        {
            for (int i = 0; i < _moveAiClips.Count; i++)
            {
                bool meet = true;
                foreach (AICondition aiCondition in _moveAiClips[i].Conditions)
                {
                    if (!aiCondition(character))
                    {
                        meet = false;
                        break;
                    };
                }

                if (meet)
                {
                    for (int j = 0; j < _moveAiClips[i].Actions.Count; j++)
                    {
                        _moveAiClips[i].Actions[j](character);
                    }
                }
            }
        }

        
    }

    public struct AIClip
    {
        public List<AICondition> Conditions;
        public List<AIAction> Actions;
    }
// todo 先写死一套ai逻辑绑在敌人身上 大概就是 如果攻击范围内有敌人 就移动并攻击血量最少的敌人 否则待机

    public delegate bool AICondition(CharacterObject characterObj);

    public delegate void AIAction(CharacterObject characterObj);


