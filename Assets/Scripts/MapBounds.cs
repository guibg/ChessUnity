using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapBounds
{
    public static Vector2 min;
    public static Vector2 max;
    
    public static Vector3 GetPositionInsideMapBounds(Vector3 position)
    {
        if (position.x < MapBounds.min.x) position.x = MapBounds.min.x;
        if (position.x > MapBounds.max.x) position.x = MapBounds.max.x;
        if (position.y < MapBounds.min.y) position.y = MapBounds.min.y;
        if (position.y > MapBounds.max.y) position.y = MapBounds.max.y;
        return position;
    }
}
