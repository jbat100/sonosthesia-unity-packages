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

	    private readonly EaseType _easeType;
	    private readonly Func<T, T, float, T> _lerpingFunction;
	    
	    public EaseCurve(EaseType easeType, CurveEndpoint<T> start, CurveEndpoint<T> end, Func<T, T, float, T> lerpingFunction)
	    {
		    _start = start;
		    _end = end;
		    _easeType = easeType;
		    _lerpingFunction = lerpingFunction;
	    }

	    protected void Evaluate(float t, CurveEndpoint<T> lower, CurveEndpoint<T> upper, out T value)
	    {
		    if (_lerpingFunction == null)
		    {
			    value = upper.Value;
			    return;
		    }
		    double ease = (t - lower.Time) / (upper.Time - lower.Time);
		    double lerp = _easeType.Evaluate(0, 1, ease);
		    value = _lerpingFunction(lower.Value, upper.Value, (float)lerp);
	    }

	    public virtual void Evaluate(float t, out T value)
	    {
		    Evaluate(t, _start, _end, out value);
	    }
    }
    
    public class PulseCurve<T> : EaseCurve<T> where T: struct
    {
	    private readonly CurveEndpoint<T> _middle;
	    public CurveEndpoint<T> Middle => _middle;
	    
	    public PulseCurve(EaseType easeType, CurveEndpoint<T> start, CurveEndpoint<T> middle, CurveEndpoint<T> end, Func<T, T, float, T> lerpingFunction) 
		    : base(easeType, start, end, lerpingFunction)
	    {
		    _middle = middle;
	    }

	    public override void Evaluate(float t, out T value)
	    {
		    if (t >= Start.Time && t < Middle.Time)
		    {
			    Evaluate(t, Start, Middle, out value);
		    }
		    else
		    {
			    Evaluate(t, Middle, End, out value);
		    }
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
        easeInBack,
        easeOutBack,
        easeInOutBack
    }
}