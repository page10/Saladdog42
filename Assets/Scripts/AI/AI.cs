using System;
using System.Collections.Generic;
using UnityEngine;


    public class AI : MonoBehaviour
    {
        private CharacterObject character;
        private List<AIClip> _moveAiClips = new List<AIClip>();  // 移动相关的ai
        private List<AIClip> _attackAiClips = new List<AIClip>();  // 攻击相关的ai
        public List<AIClipData> moveAiClipsData;  
        public List<AIClipData> attackAiClipsData;

        private void Awake()
        {
            character = GetComponent<CharacterObject>();
        }

        private void Start()
        {
            for (int i = 0; i < moveAiClipsData.Count; i++)
            {
                AIClip aiClip = new AIClip();
                aiClip.Conditions = new List<AICondition>();
                aiClip.Actions = new List<AIAction>();
                for (int j = 0; j < moveAiClipsData[i].Conditions.Count; j++)
                {
                    aiClip.Conditions.Add(AiConditions.AIConditionsDict[moveAiClipsData[i].Conditions[j]]);
                }

                for (int j = 0; j < moveAiClipsData[i].Actions.Count; j++)
                {
                    aiClip.Actions.Add(AiActions.AIActionsDict[moveAiClipsData[i].Actions[j]]);
                }
                _moveAiClips.Add(aiClip);
            }

            for (int i = 0; i < attackAiClipsData.Count; i++)
            {
                AIClip aiClip = new AIClip();
                aiClip.Conditions = new List<AICondition>();
                aiClip.Actions = new List<AIAction>();
                for (int j = 0; j < attackAiClipsData[i].Conditions.Count; j++)
                {
                    aiClip.Conditions.Add(AiConditions.AIConditionsDict[attackAiClipsData[i].Conditions[j]]);
                }

                for (int j = 0; j < attackAiClipsData[i].Actions.Count; j++)
                {
                    aiClip.Actions.Add(AiActions.AIActionsDict[attackAiClipsData[i].Actions[j]]);
                }
                _attackAiClips.Add(aiClip);
            }
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

        public AIClip GetAvailableAttackAI(CharacterObject characterObj)
        {
            for (int i = 0; i < _attackAiClips.Count; i++)
            {
                bool meet = true;
                foreach (AICondition aiCondition in _attackAiClips[i].Conditions)
                {
                    if (!aiCondition(characterObj))
                    {
                        meet = false;
                        break;
                    };
                }

                if (meet)
                {
                    return _attackAiClips[i];
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

    [Serializable]
    public struct AIClipData  // 用来读填表数据
    {
        public List<string> Conditions;
        public List<string> Actions;
    }
