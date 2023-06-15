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
                    //做个no null判断
                    if (AiConditions.AIConditionsDict.ContainsKey(moveAiClipsData[i].Conditions[j]))
                        aiClip.Conditions.Add(AiConditions.AIConditionsDict[moveAiClipsData[i].Conditions[j]]);
                }

                for (int j = 0; j < moveAiClipsData[i].Actions.Count; j++)
                {
                    //做个no null判断
                    if (AiActions.AIActionsDict.ContainsKey(moveAiClipsData[i].Actions[j]))
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
                    //做个no null判断
                    if (AiConditions.AIConditionsDict.ContainsKey(moveAiClipsData[i].Conditions[j]))
                        aiClip.Conditions.Add(AiConditions.AIConditionsDict[attackAiClipsData[i].Conditions[j]]);
                }

                for (int j = 0; j < attackAiClipsData[i].Actions.Count; j++)
                {
                    //做个no null判断
                    if (AiActions.AIActionsDict.ContainsKey(moveAiClipsData[i].Actions[j]))
                        aiClip.Actions.Add(AiActions.AIActionsDict[attackAiClipsData[i].Actions[j]]);
                }
                _attackAiClips.Add(aiClip);
            }
        }

        /// <summary>
        /// 判定各个aiClip的条件，找到并返回第一个可用的aiclip
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
        /// 生成aiNodeData
        /// 应该类似于GenerateBattleAnimEvent()生成动画data时候做的事情
        /// </summary>
        /// <param name="aiClip">要执行的那个aiClip</param>
        /// <param name="parentNodeData">执行到这条之前的aiNodeData</param>
        public AiNodeData GenAiNode(AIClip aiClip, AiNodeData parentNodeData)
        {
            if (aiClip.Actions == null) return parentNodeData;  // 如果没有可以执行的action就直接跳过
            for (int j = 0; j < aiClip.Actions.Count; j++)
            {
                AiNodeData newAiNodeData = aiClip.Actions[j](character);  // 逐条执行aiClip中的actions 里面是个AiAction这个delegate 也就是调用了AiActions里面的方法
                if (parentNodeData != null)
                    parentNodeData.NextEventDatas.Add(newAiNodeData);  // 把新的aiNodeData加到这个aiNodeData的nextEventDatas里面 不确定对不对
            }

            return parentNodeData;
        }

        
    }

    public struct AIClip
    {
        public List<AICondition> Conditions;
        public List<AIAction> Actions;
        
    }
// 先写死一套ai逻辑绑在敌人身上 大概就是 如果攻击范围内有敌人 就移动并攻击血量最少的敌人 否则待机

    public delegate bool AICondition(in CharacterObject characterObj);

    public delegate AiNodeData AIAction(in CharacterObject characterObj);

    [Serializable]
    public struct AIClipData  // 用来读填表数据
    {
        public List<string> Conditions;
        public List<string> Actions;
    }
