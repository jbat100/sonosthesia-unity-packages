using Unity.Mathematics;

namespace Sonosthesia.Ease
{
	public static class Double4EaseTypeExtensions
	{
		public static double4  Evaluate(this EaseType easeType, double4  start, double4  end, double4  value)
		{
			return Double4EaseCurves.Ease(easeType, start, end, value);
		}
		
		public static double4  Evaluate(this EaseType easeType, double4  value)
		{
			return Double4EaseCurves.Ease(easeType, 0f, 1f, value);
		}
	}
	
    public static class Double4EaseCurves
    {
	    public static double4  Ease(EaseType type, double4  start, double4  end, double4  value) => type switch 
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

	    private static double4  linear(double4  start, double4  end, double4  value)
	    {
		    return math.lerp(start, end, value);
	    }
	    
	    private static double4  spring(double4  start, double4  end, double4  value)
	    {
		    value = math.clamp(value, 0, 1);
		    value = (math.sin(value * math.PI * (0.2f + 2.5f * value * value * value)) * math.pow(1f - value, 2.2f) +
		             value) * (1f + (1.2f * (1f - value)));
		    return start + (end - start) * value;
	    }

	    private static double4  easeInQuad(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * value * value + start;
	    }

	    private static double4  easeOutQuad(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return -end * value * (value - 2) + start;
	    }

	    private static double4  easeInOutQuad(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;

		    double4  valueMinus1 = value - 1;
		    
		    return math.select(end * 0.5f * value * value + start,
			    -end * 0.5f * (valueMinus1 * (valueMinus1 - 2) - 1) + start,
			    value < 1);
	    }

	    private static double4  easeInCubic(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * value * value * value + start;
	    }

	    private static double4  easeOutCubic(double4  start, double4  end, double4  value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value + 1) + start;
	    }

	    private static double4  easeInOutCubic(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;
		    double4  valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value + start,
			    end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 + 2) + start, 
			    value < 1);
	    }

	    private static double4  easeInQuart(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * value * value * value * value + start;
	    }

	    private static double4  easeOutQuart(double4  start, double4  end, double4  value)
	    {
		    value--;
		    end -= start;
		    return -end * (value * value * value * value - 1) + start;
	    }

	    private static double4  easeInOutQuart(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;
		    double4  valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value * value + start,
			    -end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 - 2) + start, 
			    value < 1);
	    }

	    private static double4  easeInQuint(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * value * value * value * value * value + start;
	    }

	    private static double4  easeOutQuint(double4  start, double4  end, double4  value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value * value * value + 1) + start;
	    }

	    private static double4  easeInOutQuint(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;
		    double4  valueMinus2 = value - 2;
		    return math.select(end * 0.5f * value * value * value * value * value + start,
			    end * 0.5f * (valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 * valueMinus2 + 2) + start,
			    value < 1);
	    }

	    private static double4  easeInSine(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return -end * math.cos(value * (math.PI * 0.5f)) + end + start;
	    }

	    private static double4  easeOutSine(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * math.sin(value * (math.PI * 0.5f)) + start;
	    }

	    private static double4  easeInOutSine(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return -end * 0.5f * (math.cos(math.PI * value) - 1) + start;
	    }

	    private static double4  easeInExpo(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * math.pow(2, 10 * (value - 1)) + start;
	    }

	    private static double4  easeOutExpo(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return end * (-math.pow(2, -10 * value) + 1) + start;
	    }

	    private static double4  easeInOutExpo(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;
		    return math.select(end * 0.5f * math.pow(2, 10 * (value - 1)) + start,
			    end * 0.5f * (-math.pow(2, -10 * (value - 1)) + 2) + start,
			    value < 1);
	    }

	    private static double4  easeInCirc(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    return -end * (math.sqrt(1 - value * value) - 1) + start;
	    }

	    private static double4  easeOutCirc(double4  start, double4  end, double4  value)
	    {
		    value--;
		    end -= start;
		    return end * math.sqrt(1 - value * value) + start;
	    }

	    private static double4  easeInOutCirc(double4  start, double4  end, double4  value)
	    {
		    value /= .5f;
		    end -= start;
		    double4  valueMinus2 = value - 2;
		    return math.select(-end * 0.5f * (math.sqrt(1 - value * value) - 1) + start,
			    end * 0.5f * (math.sqrt(1 - valueMinus2 * valueMinus2) + 1) + start,
			    value < 1);
	    }

	    private static double4  easeInBack(double4  start, double4  end, double4  value)
	    {
		    end -= start;
		    value /= 1;
		    double4  s = 1.70158f;
		    return end * (value) * value * ((s + 1) * value - s) + start;
	    }

	    private static double4  easeOutBack(double4  start, double4  end, double4  value)
	    {
		    double4  s = 1.70158f;
		    end -= start;
		    value = (value) - 1;
		    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	    }

	    private static double4  easeInOutBack(double4  start, double4  end, double4  value)
	    {
		    double4  s = 2.5949095f; // 1.70158f * 1.525f;
		    end -= start;
		    value /= .5f;
		    double4  valueMinus2 = value - 2;
		    return math.select(end * 0.5f * (value * value * (((s) + 1) * value - s)) + start,
			    end * 0.5f * ((valueMinus2) * valueMinus2 * (((s) + 1) * valueMinus2 + s) + 2) + start,
			    value < 1);
	    }
    }
}