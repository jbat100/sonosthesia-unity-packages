using System;

namespace Sonosthesia.Ease
{
	public interface ICurve<T> where T: struct
    {
	    void Evaluate(float t, out T value);
	    
	    CurveEndpoint<T> Start { get; }
	    CurveEndpoint<T> End { get; }
    }

    public readonly struct CurveEndpoint<T> where T: struct
    {
	    public readonly float Time;
	    public readonly T Value;

	    public CurveEndpoint(float time, T value)
	    {
		    Time = time;
		    Value = value;
	    }
    }
    
    public class EaseCurve<T> : ICurve<T> where T: struct
    {
	    private readonly CurveEndpoint<T> _start;
	    public CurveEndpoint<T> Start => _start;

	    private readonly CurveEndpoint<T> _end;
	    public CurveEndpoint<T> End => _end;

	    private readonly Func<double, double, double, double> _easingFunction;
	    private readonly Func<T, T, float, T> _lerpingFunction;
	    
	    public EaseCurve(EaseType easeType, CurveEndpoint<T> start, CurveEndpoint<T> end, Func<T, T, float, T> lerpingFunction)
	    {
		    _start = start;
		    _end = end;
		    _easingFunction = DoubleEaseCurves.GetEasingFunction(easeType);
		    _lerpingFunction = lerpingFunction;
	    }

	    public void Evaluate(float t, out T value)
	    {
		    if (_lerpingFunction == null)
		    {
			    value = End.Value;
			    return;
		    }
		    double ease = (t - _start.Time) / (_end.Time - _start.Time);
		    double lerp = _easingFunction(0, 1, ease);
		    value = _lerpingFunction(_start.Value, _end.Value, (float)lerp);
	    }
    }
    
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


    
}