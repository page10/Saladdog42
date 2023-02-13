using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 按钮event和文字的struct
/// </summary>
public struct MsgDlgButtonInfo
{
    public string commandText;
    public CommandEvent commandEvent;

    public MsgDlgButtonInfo(string commandText, CommandEvent commandEvent)
    {
        this.commandText = commandText;
        this.commandEvent = commandEvent;
    }
    
}

public delegate void CommandEvent();