using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a script with a static class so all the functions can be called anywhere in the project.
/// 
/// ATTATCHED TO: N/A
/// </summary>


public static class AnimMath
{
    public static float Map(float input, float min, float max, float min2, float max2)
    {
        return (input - min) * (max2 - min2) / (max - min) + min2;
    }

    public static float Lerp(float min, float max, float p, bool allowExtrapolation = true)
    {
        if (!allowExtrapolation)
        {
            if (p < 0) p = 0;
            if (p > 1) p = 1;
        }
        return (max - min) * p + min;
    }

    public static Vector3 Lerp(Vector3 min, Vector3 max, float p, bool allowExtrapolation = true)
    {
        if (!allowExtrapolation)
        {
            if (p < 0) p = 0;
            if (p > 1) p = 1;
        }
        return (max - min) * p + min;
    }

    public static Quaternion Lerp(Quaternion min, Quaternion max, float p, bool allowExtrapolation = true)
    {
        if (!allowExtrapolation)
        {
            if (p < 0) return max;
            if (p > 1) return min;
        }

        if (min == default(Quaternion)) return min;
        if (max == default(Quaternion)) return max;

        return Quaternion.Lerp(min, max, p);
    }

    public static float Slide(float current, float target, float percentLeftAfter1Second)
    {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }

    public static Vector3 Slide(Vector3 current, Vector3 target, float percentLeftAfter1Second)
    {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }

    public static Quaternion Slide(Quaternion current, Quaternion target, float percentLeftAfter1Second = .05f)
    {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }
}
