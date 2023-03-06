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
    public int mAttack;
    public int defense;
    public int mDefense;
    public int hp;
    public int hit;
    public int dodge;
    public int crit;
    public int speed;

    public CharacterStatus(int attack, int mAttack, int defense, int mDefense, int hp, int hit, int dodge, int crit, int speed)
    {
        this.attack = attack;
        this.mAttack = mAttack;
        this.defense = defense;
        this.mDefense = mDefense;
        this.hp = hp;
        this.hit = hit;
        this.dodge = dodge;
        this.crit = crit;
        this.speed = speed;
    }
    
    public static CharacterStatus operator +(CharacterStatus a, CharacterStatus b)
    {
        return new CharacterStatus(a.attack + b.attack, a.mAttack + b.mAttack, 
            a.defense + b.defense, a.mDefense + b.mDefense, a.hp + b.hp, a.hit + b.hit, 
            a.dodge + b.dodge, a.crit + b.crit, a.speed + b.speed);
    }
}
