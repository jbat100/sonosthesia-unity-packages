using System;
using UnityEngine;

namespace Sonosthesia.Ease
{
    // Adapted from :
    // https://github.com/PixelWizards/iTween
    // Copyright (c) 2011 - 2018 Bob Berkebile (pixelplacment) 
    
    public enum EaseType 
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
    }

    public static class EaseTypeExtensions
    {
	    public static float Evaluate(this EaseType easeType, float value)
	    {
		    Func<float, float, float, float> func = easeType.EasingFunction();
		    if (func != null)
		    {
			    return func(0, 1, value);
		    }
		    return 0;
	    }
	    
	    public static float Evaluate(this EaseType easeType, float start, float end, float value)
	    {
		    Func<float, float, float, float> func = easeType.EasingFunction();
		    if (func != null)
		    {
			    return func(start, end, value);
		    }
		    return 0;
	    }

	    public static Func<float, float, float, float> EasingFunction(this EaseType easeType)
	    {
		    return EaseCurves.GetEasingFunction(easeType);
	    }
    }

    public static class EaseCurves
    {
	    public static Func<float, float, float, float> GetEasingFunction(EaseType type) => type switch 
	    {
		    EaseType.easeInQuad => easeInQuad,
		    EaseType.easeOutQuad => easeOutQuad,
		    EaseType.easeInOutQuad => easeInOutQuad,
		    EaseType.easeInCubic => easeInCubic,
		    EaseType.easeOutCubic => easeOutCubic,
		    EaseType.easeInOutCubic => easeInOutCubic,
		    EaseType.easeInQuart => easeInQuart,
		    EaseType.easeOutQuart => easeOutQuart,
		    EaseType.easeInOutQuart => easeInOutQuart,
		    EaseType.easeInQuint => easeInQuint,
		    EaseType.easeOutQuint => easeOutQuint,
		    EaseType.easeInOutQuint => easeInOutQuint,
		    EaseType.easeInSine => easeInSine,
		    EaseType.easeOutSine => easeOutSine,
		    EaseType.easeInOutSine => easeInOutSine,
		    EaseType.easeInExpo => easeInExpo,
		    EaseType.easeOutExpo => easeOutExpo,
		    EaseType.easeInOutExpo => easeInOutExpo,
		    EaseType.easeInCirc => easeInCirc,
		    EaseType.easeOutCirc => easeOutCirc,
		    EaseType.easeInOutCirc => easeInOutCirc,
		    EaseType.linear => linear,
		    EaseType.spring => spring,
		    EaseType.easeInBounce => easeInBounce,
		    EaseType.easeOutBounce => easeOutBounce,
		    EaseType.easeInOutBounce => easeInOutBounce,
		    EaseType.easeInBack => easeInBack,
		    EaseType.easeOutBack => easeOutBack,
		    EaseType.easeInOutBack => easeInOutBack,
		    EaseType.easeInElastic => easeInElastic,
		    EaseType.easeOutElastic => easeOutElastic,
		    EaseType.easeInOutElastic => easeInOutElastic,
		    _ => null
	    };

	    private static float linear(float start, float end, float value)
	    {
		    return Mathf.Lerp(start, end, value);
	    }

	    private static float clerp(float start, float end, float value)
	    {
		    float min = 0.0f;
		    float max = 360.0f;
		    float half = Mathf.Abs((max - min) * 0.5f);
		    float retval = 0.0f;
		    float diff = 0.0f;
		    if ((end - start) < -half)
		    {
			    diff = ((max - start) + end) * value;
			    retval = start + diff;
		    }
		    else if ((end - start) > half)
		    {
			    diff = -((max - end) + start) * value;
			    retval = start + diff;
		    }
		    else retval = start + (end - start) * value;

		    return retval;
	    }

	    private static float spring(float start, float end, float value)
	    {
		    value = Mathf.Clamp01(value);
		    value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) +
		             value) * (1f + (1.2f * (1f - value)));
		    return start + (end - start) * value;
	    }

	    private static float easeInQuad(float start, float end, float value)
	    {
		    end -= start;
		    return end * value * value + start;
	    }

	    private static float easeOutQuad(float start, float end, float value)
	    {
		    end -= start;
		    return -end * value * (value - 2) + start;
	    }

	    private static float easeInOutQuad(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * value * value + start;
		    value--;
		    return -end * 0.5f * (value * (value - 2) - 1) + start;
	    }

	    private static float easeInCubic(float start, float end, float value)
	    {
		    end -= start;
		    return end * value * value * value + start;
	    }

	    private static float easeOutCubic(float start, float end, float value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value + 1) + start;
	    }

	    private static float easeInOutCubic(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * value * value * value + start;
		    value -= 2;
		    return end * 0.5f * (value * value * value + 2) + start;
	    }

	    private static float easeInQuart(float start, float end, float value)
	    {
		    end -= start;
		    return end * value * value * value * value + start;
	    }

	    private static float easeOutQuart(float start, float end, float value)
	    {
		    value--;
		    end -= start;
		    return -end * (value * value * value * value - 1) + start;
	    }

	    private static float easeInOutQuart(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * value * value * value * value + start;
		    value -= 2;
		    return -end * 0.5f * (value * value * value * value - 2) + start;
	    }

	    private static float easeInQuint(float start, float end, float value)
	    {
		    end -= start;
		    return end * value * value * value * value * value + start;
	    }

	    private static float easeOutQuint(float start, float end, float value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value * value * value + 1) + start;
	    }

	    private static float easeInOutQuint(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * value * value * value * value * value + start;
		    value -= 2;
		    return end * 0.5f * (value * value * value * value * value + 2) + start;
	    }

	    private static float easeInSine(float start, float end, float value)
	    {
		    end -= start;
		    return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	    }

	    private static float easeOutSine(float start, float end, float value)
	    {
		    end -= start;
		    return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
	    }

	    private static float easeInOutSine(float start, float end, float value)
	    {
		    end -= start;
		    return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	    }

	    private static float easeInExpo(float start, float end, float value)
	    {
		    end -= start;
		    return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	    }

	    private static float easeOutExpo(float start, float end, float value)
	    {
		    end -= start;
		    return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
	    }

	    private static float easeInOutExpo(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		    value--;
		    return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	    }

	    private static float easeInCirc(float start, float end, float value)
	    {
		    end -= start;
		    return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	    }

	    private static float easeOutCirc(float start, float end, float value)
	    {
		    value--;
		    end -= start;
		    return end * Mathf.Sqrt(1 - value * value) + start;
	    }

	    private static float easeInOutCirc(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		    value -= 2;
		    return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	    }

	    /* GFX47 MOD START */
	    private static float easeInBounce(float start, float end, float value)
	    {
		    end -= start;
		    float d = 1f;
		    return end - easeOutBounce(0, end, d - value) + start;
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    //private static float bounce(float start, float end, float value){
	    private static float easeOutBounce(float start, float end, float value)
	    {
		    value /= 1f;
		    end -= start;
		    if (value < (1 / 2.75f))
		    {
			    return end * (7.5625f * value * value) + start;
		    }
		    else if (value < (2 / 2.75f))
		    {
			    value -= (1.5f / 2.75f);
			    return end * (7.5625f * (value) * value + .75f) + start;
		    }
		    else if (value < (2.5 / 2.75))
		    {
			    value -= (2.25f / 2.75f);
			    return end * (7.5625f * (value) * value + .9375f) + start;
		    }
		    else
		    {
			    value -= (2.625f / 2.75f);
			    return end * (7.5625f * (value) * value + .984375f) + start;
		    }
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    private static float easeInOutBounce(float start, float end, float value)
	    {
		    end -= start;
		    float d = 1f;
		    if (value < d * 0.5f) return easeInBounce(0, end, value * 2) * 0.5f + start;
		    else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
	    }
	    /* GFX47 MOD END */

	    private static float easeInBack(float start, float end, float value)
	    {
		    end -= start;
		    value /= 1;
		    float s = 1.70158f;
		    return end * (value) * value * ((s + 1) * value - s) + start;
	    }

	    private static float easeOutBack(float start, float end, float value)
	    {
		    float s = 1.70158f;
		    end -= start;
		    value = (value) - 1;
		    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	    }

	    private static float easeInOutBack(float start, float end, float value)
	    {
		    float s = 1.70158f;
		    end -= start;
		    value /= .5f;
		    if ((value) < 1)
		    {
			    s *= (1.525f);
			    return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		    }

		    value -= 2;
		    s *= (1.525f);
		    return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	    }

	    private static float punch(float amplitude, float value)
	    {
		    float s = 9;
		    if (value == 0)
		    {
			    return 0;
		    }
		    else if (value == 1)
		    {
			    return 0;
		    }

		    float period = 1 * 0.3f;
		    s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		    return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
	    }

	    /* GFX47 MOD START */
	    private static float easeInElastic(float start, float end, float value)
	    {
		    end -= start;

		    float d = 1f;
		    float p = d * .3f;
		    float s = 0;
		    float a = 0;

		    if (value == 0) return start;

		    if ((value /= d) == 1) return start + end;

		    if (a == 0f || a < Mathf.Abs(end))
		    {
			    a = end;
			    s = p / 4;
		    }
		    else
		    {
			    s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		    }

		    return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    //private static float elastic(float start, float end, float value){
	    private static float easeOutElastic(float start, float end, float value)
	    {
		    /* GFX47 MOD END */
		    //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		    end -= start;

		    float d = 1f;
		    float p = d * .3f;
		    float s = 0;
		    float a = 0;

		    if (value == 0) return start;

		    if ((value /= d) == 1) return start + end;

		    if (a == 0f || a < Mathf.Abs(end))
		    {
			    a = end;
			    s = p * 0.25f;
		    }
		    else
		    {
			    s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		    }

		    return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	    }

	    /* GFX47 MOD START */
	    private static float easeInOutElastic(float start, float end, float value)
	    {
		    end -= start;

		    float d = 1f;
		    float p = d * .3f;
		    float s = 0;
		    float a = 0;

		    if (value == 0) return start;

		    if ((value /= d * 0.5f) == 2) return start + end;

		    if (a == 0f || a < Mathf.Abs(end))
		    {
			    a = end;
			    s = p / 4;
		    }
		    else
		    {
			    s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		    }

		    if (value < 1)
			    return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) +
			           start;
		    return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end +
		           start;
	    }
	    /* GFX47 MOD END */

    }
}