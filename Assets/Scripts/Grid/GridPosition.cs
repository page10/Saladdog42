using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    public Vector2Int grid;
    public void SynchronizeGridPosition()
    {
        gameObject.transform.position = new Vector3(
            grid.x * Constants.tileSize,
            grid.y * Constants.tileSize
        );
    }
    private void Start() {
        SynchronizeGridPosition();
    }
}
