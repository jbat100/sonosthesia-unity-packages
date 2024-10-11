using Unity.Mathematics;

// Adapted from :
// https://github.com/PixelWizards/iTween
// Copyright (c) 2011 - 2018 Bob Berkebile (pixelplacment) 

namespace Sonosthesia.Ease
{
	public static class DoubleEaseTypeExtensions
	{
		public static double Evaluate(this EaseType easeType, double start, double end, double value)
		{
			return DoubleEaseCurves.Ease(easeType, start, end, value);
		}
		
		public static double Evaluate(this EaseType easeType, double value)
		{
			return DoubleEaseCurves.Ease(easeType, 0f, 1f, value);
		}
	}
	
    public static class DoubleEaseCurves
    {
	    public static double Ease(EaseType type, double start, double end, double value) => type switch 
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
		    EaseType.easeInBounce => easeInBounce(start, end, value),
		    EaseType.easeOutBounce => easeOutBounce(start, end, value),
		    EaseType.easeInOutBounce => easeInOutBounce(start, end, value),
		    EaseType.easeInBack => easeInBack(start, end, value),
		    EaseType.easeOutBack => easeOutBack(start, end, value),
		    EaseType.easeInOutBack => easeInOutBack(start, end, value),
		    EaseType.easeInElastic => easeInElastic(start, end, value),
		    EaseType.easeOutElastic => easeOutElastic(start, end, value),
		    EaseType.easeInOutElastic => easeInOutElastic(start, end, value),
		    _ => linear(start, end, value),
	    };

	    private static double linear(double start, double end, double value)
	    {
		    return math.lerp(start, end, value);
	    }

	    private static double clerp(double start, double end, double value)
	    {
		    double min = 0.0;
		    double max = 360.0;
		    double half = math.abs((max - min) * 0.5);
		    double retval = 0.0;
		    double diff = 0.0;
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

	    private static double spring(double start, double end, double value)
	    {
		    value = math.clamp(value, 0, 1);
		    value = (math.sin(value * math.PI * (0.2 + 2.5 * value * value * value)) * math.pow(1 - value, 2.2) +
		             value) * (1 + (1.2 * (1 - value)));
		    return start + (end - start) * value;
	    }

	    private static double easeInQuad(double start, double end, double value)
	    {
		    end -= start;
		    return end * value * value + start;
	    }

	    private static double easeOutQuad(double start, double end, double value)
	    {
		    end -= start;
		    return -end * value * (value - 2) + start;
	    }

	    private static double easeInOutQuad(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return end * 0.5 * value * value + start;
		    value--;
		    return -end * 0.5 * (value * (value - 2) - 1) + start;
	    }

	    private static double easeInCubic(double start, double end, double value)
	    {
		    end -= start;
		    return end * value * value * value + start;
	    }

	    private static double easeOutCubic(double start, double end, double value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value + 1) + start;
	    }

	    private static double easeInOutCubic(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return end * 0.5 * value * value * value + start;
		    value -= 2;
		    return end * 0.5 * (value * value * value + 2) + start;
	    }

	    private static double easeInQuart(double start, double end, double value)
	    {
		    end -= start;
		    return end * value * value * value * value + start;
	    }

	    private static double easeOutQuart(double start, double end, double value)
	    {
		    value--;
		    end -= start;
		    return -end * (value * value * value * value - 1) + start;
	    }

	    private static double easeInOutQuart(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return end * 0.5 * value * value * value * value + start;
		    value -= 2;
		    return -end * 0.5 * (value * value * value * value - 2) + start;
	    }

	    private static double easeInQuint(double start, double end, double value)
	    {
		    end -= start;
		    return end * value * value * value * value * value + start;
	    }

	    private static double easeOutQuint(double start, double end, double value)
	    {
		    value--;
		    end -= start;
		    return end * (value * value * value * value * value + 1) + start;
	    }

	    private static double easeInOutQuint(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return end * 0.5 * value * value * value * value * value + start;
		    value -= 2;
		    return end * 0.5 * (value * value * value * value * value + 2) + start;
	    }

	    private static double easeInSine(double start, double end, double value)
	    {
		    end -= start;
		    return -end * math.cos(value * (math.PI * 0.5)) + end + start;
	    }

	    private static double easeOutSine(double start, double end, double value)
	    {
		    end -= start;
		    return end * math.sin(value * (math.PI * 0.5)) + start;
	    }

	    private static double easeInOutSine(double start, double end, double value)
	    {
		    end -= start;
		    return -end * 0.5 * (math.cos(math.PI * value) - 1) + start;
	    }

	    private static double easeInExpo(double start, double end, double value)
	    {
		    end -= start;
		    return end * math.pow(2, 10 * (value - 1)) + start;
	    }

	    private static double easeOutExpo(double start, double end, double value)
	    {
		    end -= start;
		    return end * (-math.pow(2, -10 * value) + 1) + start;
	    }

	    private static double easeInOutExpo(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return end * 0.5 * math.pow(2, 10 * (value - 1)) + start;
		    value--;
		    return end * 0.5 * (-math.pow(2, -10 * value) + 2) + start;
	    }

	    private static double easeInCirc(double start, double end, double value)
	    {
		    end -= start;
		    return -end * (math.sqrt(1 - value * value) - 1) + start;
	    }

	    private static double easeOutCirc(double start, double end, double value)
	    {
		    value--;
		    end -= start;
		    return end * math.sqrt(1 - value * value) + start;
	    }

	    private static double easeInOutCirc(double start, double end, double value)
	    {
		    value /= .5;
		    end -= start;
		    if (value < 1) return -end * 0.5 * (math.sqrt(1 - value * value) - 1) + start;
		    value -= 2;
		    return end * 0.5 * (math.sqrt(1 - value * value) + 1) + start;
	    }

	    /* GFX47 MOD START */
	    private static double easeInBounce(double start, double end, double value)
	    {
		    end -= start;
		    double d = 1;
		    return end - easeOutBounce(0, end, d - value) + start;
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    //private static double bounce(double start, double end, double value){
	    private static double easeOutBounce(double start, double end, double value)
	    {
		    value /= 1;
		    end -= start;
		    if (value < (1 / 2.75))
		    {
			    return end * (7.5625 * value * value) + start;
		    }
		    else if (value < (2 / 2.75))
		    {
			    value -= (1.5 / 2.75);
			    return end * (7.5625 * (value) * value + .75) + start;
		    }
		    else if (value < (2.5 / 2.75))
		    {
			    value -= (2.25 / 2.75);
			    return end * (7.5625 * (value) * value + .9375) + start;
		    }
		    else
		    {
			    value -= (2.625 / 2.75);
			    return end * (7.5625 * (value) * value + .984375) + start;
		    }
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    private static double easeInOutBounce(double start, double end, double value)
	    {
		    end -= start;
		    double d = 1;
		    if (value < d * 0.5) return easeInBounce(0, end, value * 2) * 0.5 + start;
		    else return easeOutBounce(0, end, value * 2 - d) * 0.5 + end * 0.5 + start;
	    }
	    /* GFX47 MOD END */

	    private static double easeInBack(double start, double end, double value)
	    {
		    end -= start;
		    value /= 1;
		    double s = 1.70158;
		    return end * (value) * value * ((s + 1) * value - s) + start;
	    }

	    private static double easeOutBack(double start, double end, double value)
	    {
		    double s = 1.70158;
		    end -= start;
		    value = (value) - 1;
		    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	    }

	    private static double easeInOutBack(double start, double end, double value)
	    {
		    double s = 1.70158;
		    end -= start;
		    value /= .5;
		    if ((value) < 1)
		    {
			    s *= (1.525);
			    return end * 0.5 * (value * value * (((s) + 1) * value - s)) + start;
		    }

		    value -= 2;
		    s *= (1.525);
		    return end * 0.5 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	    }

	    private static double punch(double amplitude, double value)
	    {
		    double s = 9;
		    if (value == 0)
		    {
			    return 0;
		    }
		    else if (value == 1)
		    {
			    return 0;
		    }

		    double period = 1 * 0.3;
		    s = period / (2 * math.PI) * math.asin(0);
		    return (amplitude * math.pow(2, -10 * value) * math.sin((value * 1 - s) * (2 * math.PI) / period));
	    }

	    /* GFX47 MOD START */
	    private static double easeInElastic(double start, double end, double value)
	    {
		    end -= start;

		    double d = 1;
		    double p = d * .3;
		    double s = 0;
		    double a = 0;

		    if (value == 0) return start;

		    if ((value /= d) == 1) return start + end;

		    if (a == 0 || a < math.abs(end))
		    {
			    a = end;
			    s = p / 4;
		    }
		    else
		    {
			    s = p / (2 * math.PI) * math.asin(end / a);
		    }

		    return -(a * math.pow(2, 10 * (value -= 1)) * math.sin((value * d - s) * (2 * math.PI) / p)) + start;
	    }
	    /* GFX47 MOD END */

	    /* GFX47 MOD START */
	    //private static double elastic(double start, double end, double value){
	    private static double easeOutElastic(double start, double end, double value)
	    {
		    /* GFX47 MOD END */
		    //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		    end -= start;

		    double d = 1;
		    double p = d * .3;
		    double s = 0;
		    double a = 0;

		    if (value == 0) return start;

		    if ((value /= d) == 1) return start + end;

		    if (a == 0 || a < math.abs(end))
		    {
			    a = end;
			    s = p * 0.25;
		    }
		    else
		    {
			    s = p / (2 * math.PI) * math.asin(end / a);
		    }

		    return (a * math.pow(2, -10 * value) * math.sin((value * d - s) * (2 * math.PI) / p) + end + start);
	    }

	    /* GFX47 MOD START */
	    private static double easeInOutElastic(double start, double end, double value)
	    {
		    end -= start;

		    double d = 1;
		    double p = d * .3;
		    double s = 0;
		    double a = 0;

		    if (value == 0) return start;

		    if ((value /= d * 0.5) == 2) return start + end;

		    if (a == 0 || a < math.abs(end))
		    {
			    a = end;
			    s = p / 4;
		    }
		    else
		    {
			    s = p / (2 * math.PI) * math.asin(end / a);
		    }

		    if (value < 1)
			    return -0.5 * (a * math.pow(2, 10 * (value -= 1)) * math.sin((value * d - s) * (2 * math.PI) / p)) +
			           start;
		    return a * math.pow(2, -10 * (value -= 1)) * math.sin((value * d - s) * (2 * math.PI) / p) * 0.5 + end +
		           start;
	    }
	    /* GFX47 MOD END */

    }
}