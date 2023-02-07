using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectedCharacterInfo
{
    public int playerIndex;  // 是哪一个阵营的
    public int characterIndex;  // 是character list里的第几个

    public SelectedCharacterInfo(int playerIndex, int characterIndex)
    {
        this.playerIndex = playerIndex;
        this.characterIndex = characterIndex;
    }
}
