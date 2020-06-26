using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIAnimations
{
    public static Action<float, float> SampleAnimation()
    {
        return (time, deltaTime) => { };
    }

    public static Action<float, float> Floating(Transform obj, float distance, float endTime, float startTime = 0f)
    {
        return (time, deltaTime) =>
        {
            if (!(time >= startTime) || !(time < endTime)) return;
            obj.localPosition += (deltaTime) * distance / (endTime - startTime) * Vector3.up;
        };
    }

    /// <summary>
    /// 사이즈 변화 애니메이션.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="baseSize"></param>
    /// <param name="maxSize"></param>
    /// <param name="magnificationTime"></param>
    /// <param name="endTime"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    public static Action<float, float> SizeHighlighting(Transform obj, Vector3 baseSize, Vector3 maxSize,
        float magnificationTime, float endTime, float startTime = 0f)
    {
        return (time, deltaTime) =>
        {
            if (!(time >= startTime) || !(time < endTime)) return;
            if (time - startTime < magnificationTime)
                obj.localScale += (deltaTime) / (magnificationTime) * (maxSize - baseSize);
            else
                obj.localScale += (deltaTime) / (endTime - startTime - magnificationTime) *
                                  (baseSize - maxSize);
        };
    }

    public static Action<float, float> Shaking(Transform obj, float distance, int shakingCount, float endTime,
        float startTime = 0f)
    {
        return (time, deltaTime) =>
        {
            if (!(time >= startTime) || !(time < endTime)) return;

            var speed = 4 * distance * shakingCount / (endTime - startTime);
            if ((((time - startTime) * speed / distance) + 1) % 4 <= 2)
                obj.localPosition += (deltaTime) * speed * Vector3.left;
            else
                obj.localPosition += (deltaTime) * -speed * Vector3.left;
        };
    }

    public static Action<float, float> Dissolve(List<Image> obj, float endTime, float startTime = 0f)
    {
        return (time, deltaTime) =>
        {
            if (!(time >= startTime) || !(time < endTime)) return;
            for (var i = 0; i < obj.Count; i++)
            {
                var color = obj[i].color;
                color.a += -(deltaTime) / (endTime - startTime);
                obj[i].color = color;
            }
        };
    }
}