using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static float tileSize = 2f;  // meters, in the game world
    public static int nullPlayerIndex = -1; 
    public static int nullCharacterIndex = -1;
    public static int nullWeaponIndex = -1;
    public static int nullCommandIndex = -1;
    public static byte TargetType_Self = 1;
    public static byte TargetType_Ally = 1 << 1;
    public static byte TargetType_Foe = 1 << 2;

    public static int weaponSlots = 3;
}
