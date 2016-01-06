using UnityEngine;

public static class MathHelper
{
    public static float Remap(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
    {
        return (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
    }

    public static float Remap01(this float value, float max)
    {
        return value / max;
    }

    public static Vector3 SquareInterpolateTo(this Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return (((1 - t) * (1 - t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
    }

    public static Vector3 QuadraticSplinePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        t = Mathf.Clamp01(t);
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term 
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term
        return p;
    }
}
