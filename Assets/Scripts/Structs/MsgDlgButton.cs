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
    public object[] parameters;

    public MsgDlgButtonInfo(string commandText, CommandEvent commandEvent, object[] parameters)
    {
        this.commandText = commandText;
        this.commandEvent = commandEvent;
        this.parameters = parameters;
    }

}

public delegate void CommandEvent(object[] parameters);