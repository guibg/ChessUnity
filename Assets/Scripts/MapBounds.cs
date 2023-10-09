using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapBounds{
    public static Vector2 min;
    public static Vector2 max;
    
    public static Vector3 GetPositionInsideMapBounds(Vector3 position)
    {
        if (position.x < min.x) position.x = min.x;
        if (position.x > max.x) position.x = max.x;
        if (position.y < min.y) position.y = min.y;
        if (position.y > max.y) position.y = max.y;
        return position;
    }
    
    public static bool isPositionOutsideBoard(Vector2Int position)
    {
        if (position.x < 0) return true;
        if (position.x > 7) return true;
        if (position.y < 0) return true;
        if (position.y > 7) return true;
        return false;
    }
}
