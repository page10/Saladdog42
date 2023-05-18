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
    //todo 暴击改成计数制
    public string id;
    public int attack;
    public int mAttack;
    public int defense;
    public int mDefense;
    public int hp;
    public int crit;
    public int speed;

    public CharacterStatus(string id = "",int attack = 0, int mAttack = 0, int defense = 0, int mDefense = 0, int hp = 0, 
        int crit = 0, int speed = 0)
    {
        this.id = id;
        this.attack = attack;
        this.mAttack = mAttack;
        this.defense = defense;
        this.mDefense = mDefense;
        this.hp = hp;
        this.crit = crit;
        this.speed = speed;
    }
    
    public static CharacterStatus operator +(CharacterStatus a, CharacterStatus b)
    {
        return new CharacterStatus(a.id,a.attack + b.attack, a.mAttack + b.mAttack, 
            a.defense + b.defense, a.mDefense + b.mDefense, a.hp + b.hp,a.crit + b.crit, a.speed + b.speed);
    }
}

[Serializable]
public class LoadedCharacterStatus
{
    public CharacterStatus[] CharacterStatus;
}