using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SignType
{
    Move,  // 移动范围显示
    Attack,  // 攻击范围显示
    AttackableMove,  // 对于选中目标可以攻击的移动范围显示
    Heal,  // 治疗范围显示
    AttackHeal,  // 对于选中目标可以攻击并治疗的移动范围显示

}

