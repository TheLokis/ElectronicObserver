using System;

public static class MathExtension
{
    public static int ToInt(this object obj)
    {
        if (int.TryParse(obj.ToString(), out int res) == true)
        {
            return res;
        }

        return 0;
    }

    public static int Clamp(this int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public static float Clamp(this float value, float min, float max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public static double Clamp(this double value, double min, double max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

}
