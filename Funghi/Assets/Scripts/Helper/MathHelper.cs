
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
}
