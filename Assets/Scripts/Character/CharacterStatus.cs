using System;
/// <summary>
/// all character properties 
/// including character properties change due to terrain,
/// a character's basic properties, or equipment properties on character
/// use struct for easier modification
/// use int when calculating
/// </summary>
[Serializable]
public struct CharacterStatus
{
    public int attack;
    public int defense;
    public int mDefense;
    public int dodge;

    public CharacterStatus(int attack, int defense, int mDefense, int dodge)
    {
        this.attack = attack;
        this.defense = defense;
        this.mDefense = mDefense;
        this.dodge = dodge;
    }
}
