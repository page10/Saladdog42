using System;
using System.Collections.Generic;
using UnityEngine;


    public class AI : MonoBehaviour
    {
        private CharacterObject cha;
        private List<AIClip> _clips;
        

        private void Awake()
        {
            cha = GetComponent<CharacterObject>();
        }

        public void DoAI()
        {
            for (int i = 0; i < _clips.Count; i++)
            {
                bool meet = true;
                foreach (AICondition aiCondition in _clips[i].Conditions)
                {
                    if (!aiCondition(cha))
                    {
                        meet = false;
                        break;
                    };
                }

                if (meet)
                {
                    for (int j = 0; j < _clips[i].Actions.Count; j++)
                    {
                        _clips[i].Actions[j](cha);
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

    public delegate bool AICondition(CharacterObject characterObj);

    public delegate void AIAction(CharacterObject characterObj);
