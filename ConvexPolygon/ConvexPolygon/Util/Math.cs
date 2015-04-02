
namespace Tonari
{
    public static class Math
    {
        public static readonly float BigEpsilon = 0.0009765625f;

        public static int Power(int cardinal, int power)
        {
	        if (power < 0) return 0;
	        if (power == 0) return 1;
	        if (power == 1) return cardinal;
	        return cardinal * Power(cardinal, --power);
        }
    }
}