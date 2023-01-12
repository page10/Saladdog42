using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPosition : MonoBehaviour
{
    public Vector2Int charactertPosition;
    private void Awake() {
        charactertPosition = new Vector2Int((int)transform.position.x/2, (int)transform.position.y/2);
    }
}
