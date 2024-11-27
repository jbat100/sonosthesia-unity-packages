using Unity.Mathematics;

namespace Sonosthesia.Ease
{
	public static class Float4EaseTypeExtensions
	{
		public static float4 Evaluate(this EaseType easeType, float4 start, float4 end, float4 value)
		{
			return Float4EaseCurves.Ease(easeType, start, end, value);
		}
		
		public static float4 Evaluate(this EaseType easeType, float4 value)
		{
			return Float4EaseCurves.Ease(easeType, 0f, 1f, value);
		}
	}
	
    public static class Float4EaseCurves
    {
	    public static float4 Ease(EaseType type, float4 start, float4 end, float4 value) => type switch 
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

	    private static float4 linear(float4 start, float4 end, float4 value)
	    {
		    return math.lerp(start, end, value);
	    }
	    
	    private static float4 spring(float4 start, float4 end, float4 value)
	    {
		    value = math.clamp(value, 0, 1);
		    value = (math.sin(value * math.PI * (0.2f + 2.5f * value * value * value)) * math.pow(1f - value, 2.2f) +
		             value) * (1f + (1.2f * (1f - value)));
		    return start + (end - start) * value;
	    }

	    private static float4 easeInQuad(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * value * value + start;
	    }

	    private static float4 easeOutQuad(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return -end * value * (value - 2) + start;
	    }

	    private static float4 easeInOutQuad(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;

		    float4 valueMinus1 = value - 1;
		    
		    return math.select(end * 0.5f * value * value + start,
			    -end * 0.5f * (valueMinus1 * (valueMinus1 - 2) - 1) + start,
			    value < 1);
	    }

	    private static float4 easeInCubic(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * value * value * value + start;
	    }

	    private static float4 easeOutCubic(float4 start, float4 end, float4 value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value + 1) + start;
	    }

	    private static float4 easeInOutCubic(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;
		    float4 valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value + start,
			    end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 + 2) + start, 
			    value < 1);
	    }

	    private static float4 easeInQuart(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * value * value * value * value + start;
	    }

	    private static float4 easeOutQuart(float4 start, float4 end, float4 value)
	    {
		    value--;
		    end -= start;
		    return -end * (value * value * value * value - 1) + start;
	    }

	    private static float4 easeInOutQuart(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;
		    float4 valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value * value + start,
			    -end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 - 2) + start, 
			    value < 1);
	    }

	    private static float4 easeInQuint(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * value * value * value * value * value + start;
	    }

	    private static float4 easeOutQuint(float4 start, float4 end, float4 value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value * value * value + 1) + start;
	    }

	    private static float4 easeInOutQuint(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;
		    float4 valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value * value * value + start,
			    end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 + 2) + start,
			    value < 1);
	    }

	    private static float4 easeInSine(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return -end * math.cos(value * (math.PI * 0.5f)) + end + start;
	    }

	    private static float4 easeOutSine(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * math.sin(value * (math.PI * 0.5f)) + start;
	    }

	    private static float4 easeInOutSine(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return -end * 0.5f * (math.cos(math.PI * value) - 1) + start;
	    }

	    private static float4 easeInExpo(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * math.pow(2, 10 * (value - 1)) + start;
	    }

	    private static float4 easeOutExpo(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return end * (-math.pow(2, -10 * value) + 1) + start;
	    }

	    private static float4 easeInOutExpo(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;
		    return math.select(end * 0.5f * math.pow(2, 10 * (value - 1)) + start,
			    end * 0.5f * (-math.pow(2, -10 * (value - 1)) + 2) + start,
			    value < 1);
	    }

	    private static float4 easeInCirc(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    return -end * (math.sqrt(1 - value * value) - 1) + start;
	    }

	    private static float4 easeOutCirc(float4 start, float4 end, float4 value)
	    {
		    value--;
		    end -= start;
		    return end * math.sqrt(1 - value * value) + start;
	    }

	    private static float4 easeInOutCirc(float4 start, float4 end, float4 value)
	    {
		    value /= .5f;
		    end -= start;
		    float4 valueMinus2 = value - 2;
		    return math.select(-end * 0.5f * (math.sqrt(1 - value * value) - 1) + start,
			    end * 0.5f * (math.sqrt(1 - valueMinus2 * valueMinus2) + 1) + start,
			    value < 1);
	    }

	    private static float4 easeInBack(float4 start, float4 end, float4 value)
	    {
		    end -= start;
		    value /= 1;
		    float4 s = 1.70158f;
		    return end * (value) * value * ((s + 1) * value - s) + start;
	    }

	    private static float4 easeOutBack(float4 start, float4 end, float4 value)
	    {
		    float4 s = 1.70158f;
		    end -= start;
		    value = (value) - 1;
		    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	    }

	    private static float4 easeInOutBack(float4 start, float4 end, float4 value)
	    {
		    float4 s = 2.5949095f; // 1.70158f * 1.525f;
		    end -= start;
		    value /= .5f;
		    float4 valueMinus2 = value - 2;
		    return math.select(end * 0.5f * (value * value * (((s) + 1) * value - s)) + start,
			    end * 0.5f * ((valueMinus2) * valueMinus2 * (((s) + 1) * valueMinus2 + s) + 2) + start,
			    value < 1);
	    }
    }
}