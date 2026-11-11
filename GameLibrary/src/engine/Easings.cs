namespace GameLibrary;

// This class is a hodgepodge of my easing functions and ones taken from online, expect no consistent structure here.
public static class Easings
{
	// My easings
	public static float FullSine(float t)
	{
		return MathF.Sin(t * MathF.PI * 2);
	}

	public static float HalfSine(float t)
	{
		return MathF.Sin(t * MathF.PI);
	}

	public static float QuarterSine(float t)
	{
		return MathF.Sin(t * MathF.PI / 2);
	}

	public static float Ballistic(float t)
	{
		return HalfSine(t); // Good enough
	}

	public static float Bounce(float t)
	{
		return 1 - OutBounce(t);
	}

	// Bounce but it starts from 0 and travels up to 1 before falling and bouncing.
	public static float FullBounce(float t)
	{
		// negative intersection point: -0.363636...
		return Bounce(Map(t, 0, 1, -0.363636f, 1));
	}
	
	public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
	{
		return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
	}
	
	// Easings from https://gist.githubusercontent.com/Kryzarel/bba64622057f21a1d6d44879f9cd7bd4/raw/83c21b579e5d261433cb7bab57533e40d4b1ff23/EasingFunctions.cs
	
	// Made with the help of this great post: https://joshondesign.com/2013/03/01/improvedEasingEquations

	// --------------------------------- Other Related Links --------------------------------------------------------------------
	// Original equations, bad formulation:	https://github.com/danro/jquery-easing/blob/master/jquery.easing.js
	// A few equations, very simplified:	https://gist.github.com/gre/1650294
	// Easings.net equations, simplified:	https://github.com/ai/easings.net/blob/master/src/easings/easingsFunctions.ts

	public static double Linear(double t) => t;

	public static double InQuad(double t) => t * t;
	public static double OutQuad(double t) => 1 - InQuad(1 - t);
	public static double InOutQuad(double t)
	{
		if (t < 0.5) return InQuad(t * 2) / 2;
		return 1 - InQuad((1 - t) * 2) / 2;
	}

	public static double InCubic(double t) => t * t * t;
	public static double OutCubic(double t) => 1 - InCubic(1 - t);
	public static double InOutCubic(double t)
	{
		if (t < 0.5) return InCubic(t * 2) / 2;
		return 1 - InCubic((1 - t) * 2) / 2;
	}

	public static double InQuart(double t) => t * t * t * t;
	public static double OutQuart(double t) => 1 - InQuart(1 - t);
	public static double InOutQuart(double t)
	{
		if (t < 0.5) return InQuart(t * 2) / 2;
		return 1 - InQuart((1 - t) * 2) / 2;
	}

	public static double InQuint(double t) => t * t * t * t * t;
	public static double OutQuint(double t) => 1 - InQuint(1 - t);
	public static double InOutQuint(double t)
	{
		if (t < 0.5) return InQuint(t * 2) / 2;
		return 1 - InQuint((1 - t) * 2) / 2;
	}

	public static double InSine(double t) => 1 - (double)Math.Cos(t * Math.PI / 2);
	public static double OutSine(double t) => (double)Math.Sin(t * Math.PI / 2);
	public static double InOutSine(double t) => (double)(Math.Cos(t * Math.PI) - 1) / -2;

	public static double InExpo(double t) => (double)Math.Pow(2, 10 * (t - 1));
	public static double OutExpo(double t) => 1 - InExpo(1 - t);
	public static double InOutExpo(double t)
	{
		if (t < 0.5) return InExpo(t * 2) / 2;
		return 1 - InExpo((1 - t) * 2) / 2;
	}

	public static double InCirc(double t) => -((double)Math.Sqrt(1 - t * t) - 1);
	public static double OutCirc(double t) => 1 - InCirc(1 - t);
	public static double InOutCirc(double t)
	{
		if (t < 0.5) return InCirc(t * 2) / 2;
		return 1 - InCirc((1 - t) * 2) / 2;
	}

	public static double InElastic(double t) => 1 - OutElastic(1 - t);
	public static double OutElastic(double t)
	{
		double p = 0.3f;
		return (double)Math.Pow(2, -10 * t) * (double)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
	}
	public static double InOutElastic(double t)
	{
		if (t < 0.5) return InElastic(t * 2) / 2;
		return 1 - InElastic((1 - t) * 2) / 2;
	}

	public static double InBack(double t)
	{
		double s = 1.70158f;
		return t * t * ((s + 1) * t - s);
	}
	public static double OutBack(double t) => 1 - InBack(1 - t);
	public static double InOutBack(double t)
	{
		if (t < 0.5) return InBack(t * 2) / 2;
		return 1 - InBack((1 - t) * 2) / 2;
	}

	public static double InBounce(double t) => 1 - OutBounce(1 - t);
	public static double OutBounce(double t)
	{
		double div = 2.75f;
		double mult = 7.5625f;

		if (t < 1 / div)
		{
			return mult * t * t;
		}
		else if (t < 2 / div)
		{
			t -= 1.5f / div;
			return mult * t * t + 0.75f;
		}
		else if (t < 2.5 / div)
		{
			t -= 2.25f / div;
			return mult * t * t + 0.9375f;
		}
		else
		{
			t -= 2.625f / div;
			return mult * t * t + 0.984375f;
		}
	}
	public static double InOutBounce(double t)
	{
		if (t < 0.5) return InBounce(t * 2) / 2;
		return 1 - InBounce((1 - t) * 2) / 2;
	}
	
	
	// Floats ----------------------------------------
	public static float Linear(float t) => t;

	public static float InQuad(float t) => t * t;
	public static float OutQuad(float t) => 1 - InQuad(1 - t);
	public static float InOutQuad(float t)
	{
		if (t < 0.5) return InQuad(t * 2) / 2;
		return 1 - InQuad((1 - t) * 2) / 2;
	}

	public static float InCubic(float t) => t * t * t;
	public static float OutCubic(float t) => 1 - InCubic(1 - t);
	public static float InOutCubic(float t)
	{
		if (t < 0.5) return InCubic(t * 2) / 2;
		return 1 - InCubic((1 - t) * 2) / 2;
	}

	public static float InQuart(float t) => t * t * t * t;
	public static float OutQuart(float t) => 1 - InQuart(1 - t);
	public static float InOutQuart(float t)
	{
		if (t < 0.5) return InQuart(t * 2) / 2;
		return 1 - InQuart((1 - t) * 2) / 2;
	}

	public static float InQuint(float t) => t * t * t * t * t;
	public static float OutQuint(float t) => 1 - InQuint(1 - t);
	public static float InOutQuint(float t)
	{
		if (t < 0.5) return InQuint(t * 2) / 2;
		return 1 - InQuint((1 - t) * 2) / 2;
	}

	public static float InSine(float t) => 1 - (float)Math.Cos(t * Math.PI / 2);
	public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
	public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;

	public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
	public static float OutExpo(float t) => 1 - InExpo(1 - t);
	public static float InOutExpo(float t)
	{
		if (t < 0.5) return InExpo(t * 2) / 2;
		return 1 - InExpo((1 - t) * 2) / 2;
	}

	public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
	public static float OutCirc(float t) => 1 - InCirc(1 - t);
	public static float InOutCirc(float t)
	{
		if (t < 0.5) return InCirc(t * 2) / 2;
		return 1 - InCirc((1 - t) * 2) / 2;
	}

	public static float InElastic(float t) => 1 - OutElastic(1 - t);
	public static float OutElastic(float t)
	{
		float p = 0.3f;
		return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
	}
	public static float InOutElastic(float t)
	{
		if (t < 0.5) return InElastic(t * 2) / 2;
		return 1 - InElastic((1 - t) * 2) / 2;
	}

	public static float InBack(float t)
	{
		float s = 1.70158f;
		return t * t * ((s + 1) * t - s);
	}
	public static float OutBack(float t) => 1 - InBack(1 - t);
	public static float InOutBack(float t)
	{
		if (t < 0.5) return InBack(t * 2) / 2;
		return 1 - InBack((1 - t) * 2) / 2;
	}

	public static float InBounce(float t) => 1 - OutBounce(1 - t);
	public static float OutBounce(float t)
	{
		float div = 2.75f;
		float mult = 7.5625f;

		if (t < 1 / div)
		{
			return mult * t * t;
		}
		else if (t < 2 / div)
		{
			t -= 1.5f / div;
			return mult * t * t + 0.75f;
		}
		else if (t < 2.5 / div)
		{
			t -= 2.25f / div;
			return mult * t * t + 0.9375f;
		}
		else
		{
			t -= 2.625f / div;
			return mult * t * t + 0.984375f;
		}
	}
	public static float InOutBounce(float t)
	{
		if (t < 0.5) return InBounce(t * 2) / 2;
		return 1 - InBounce((1 - t) * 2) / 2;
	}
}
