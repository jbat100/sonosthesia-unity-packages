using Unity.Mathematics;

// Adapted from :
// https://github.com/PixelWizards/iTween
// Copyright (c) 2011 - 2018 Bob Berkebile (pixelplacment) 

namespace Sonosthesia.Ease
{
	public static class FloatEaseTypeExtensions
	{
		public static float Evaluate(this EaseType easeType, float start, float end, float value)
		{
			return FloatEaseCurves.Ease(easeType, start, end, value);
		}
		
		public static float Evaluate(this EaseType easeType, float value)
		{
			return FloatEaseCurves.Ease(easeType, 0f, 1f, value);
		}
	}
	
    public static class FloatEaseCurves
    {
	    public static float Ease(EaseType type, float start, float end, float value) => type switch 
	    {
		    EaseType.easeInQuad => easeInQuad(start, end, value),
		    EaseType.easeOutQuad => easeOutQuad(start, end, value),
		    EaseType.easeInOutQuad => easeInOutQuad(start, end, value),
		    EaseType.easeInCubic => easeInCubic(start, end, value),
		    EaseType.easeOutCubic => easeOutCubic(start, end, value),
		    EaseType.easeInOutCubic => easeInOutCubic(start, end, value),
		    EaseType.easeInQuart => easeInQuart(start, end, value),
		    EaseType.easeOutQuart => easeOutQuart(start, end, value),
		    EaseType.easeInOutQuart => easeInOutQuart(start, end, value),
		    EaseType.easeInQuint => easeInQuint(start, end, value),
		    EaseType.easeOutQuint => easeOutQuint(start, end, value),
		    EaseType.easeInOutQuint => easeInOutQuint(start, end, value),
		    EaseType.easeInSine => easeInSine(start, end, value),
		    EaseType.easeOutSine => easeOutSine(start, end, value),
		    EaseType.easeInOutSine => easeInOutSine(start, end, value),
		    EaseType.easeInExpo => easeInExpo(start, end, value),
		    EaseType.easeOutExpo => easeOutExpo(start, end, value),
		    EaseType.easeInOutExpo => easeInOutExpo(start, end, value),
		    EaseType.easeInCirc => easeInCirc(start, end, value),
		    EaseType.easeOutCirc => easeOutCirc(start, end, value),
		    EaseType.easeInOutCirc => easeInOutCirc(start, end, value),
		    EaseType.spring => spring(start, end, value),
		    EaseType.easeInBack => easeInBack(start, end, value),
		    EaseType.easeOutBack => easeOutBack(start, end, value),
		    EaseType.easeInOutBack => easeInOutBack(start, end, value),
		    _ => linear(start, end, value),
	    };

	    private static float linear(float start, float end, float value)
	    {
		    return math.lerp(start, end, value);
	    }

	    private static float clerp(float start, float end, float value)
	    {
		    float min = 0.0f;
		    float max = 360.0f;
		    float half = math.abs((max - min) * 0.5f);
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
		    value = math.clamp(value, 0, 1);
		    value = (math.sin(value * math.PI * (0.2f + 2.5f * value * value * value)) * math.pow(1f - value, 2.2f) +
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
		    return -end * math.cos(value * (math.PI * 0.5f)) + end + start;
	    }

	    private static float easeOutSine(float start, float end, float value)
	    {
		    end -= start;
		    return end * math.sin(value * (math.PI * 0.5f)) + start;
	    }

	    private static float easeInOutSine(float start, float end, float value)
	    {
		    end -= start;
		    return -end * 0.5f * (math.cos(math.PI * value) - 1) + start;
	    }

	    private static float easeInExpo(float start, float end, float value)
	    {
		    end -= start;
		    return end * math.pow(2, 10 * (value - 1)) + start;
	    }

	    private static float easeOutExpo(float start, float end, float value)
	    {
		    end -= start;
		    return end * (-math.pow(2, -10 * value) + 1) + start;
	    }

	    private static float easeInOutExpo(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return end * 0.5f * math.pow(2, 10 * (value - 1)) + start;
		    value--;
		    return end * 0.5f * (-math.pow(2, -10 * value) + 2) + start;
	    }

	    private static float easeInCirc(float start, float end, float value)
	    {
		    end -= start;
		    return -end * (math.sqrt(1 - value * value) - 1) + start;
	    }

	    private static float easeOutCirc(float start, float end, float value)
	    {
		    value--;
		    end -= start;
		    return end * math.sqrt(1 - value * value) + start;
	    }

	    private static float easeInOutCirc(float start, float end, float value)
	    {
		    value /= .5f;
		    end -= start;
		    if (value < 1) return -end * 0.5f * (math.sqrt(1 - value * value) - 1) + start;
		    value -= 2;
		    return end * 0.5f * (math.sqrt(1 - value * value) + 1) + start;
	    }

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
		    s = period / (2 * math.PI) * math.asin(0);
		    return (amplitude * math.pow(2, -10 * value) * math.sin((value * 1 - s) * (2 * math.PI) / period));
	    }
    }
}