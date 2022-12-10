using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainTile : MonoBehaviour 
{
    
    public int LandMovementConsume { get; set; }  // movement consumption of terrain for land units
    public int AirMovementConsume { get; set; }  // movement consumption of terrain for airborne units
    public int RidingMovementConsume { get; set; }   // movement consumption of terrain for riding units

    public float DefenseModifier { get; set; }   // defense modifier, percentage 
    public float AttackModifier { get; set; }   // attack modifier, percentage
    public float MDefenseModifier { get; set; }   // magic defense modifier, percentage
    public float DodgeModifier { get; set; }   // dodge modifier, percentage

}
