using System;

namespace Tand.Core.Benchmarks
{
    public readonly struct Rational
    {
        public Rational(int numerator, int denominator)
        {
            var gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            if (denominator < 0)
            {
                Numerator = -numerator;
                Denominator = -denominator;
            }
            else
            {
                Numerator = numerator;
                Denominator = denominator;
            }
        }

        public int Numerator { get; }
        public int Denominator { get; }

        private static int GCD(int a, int b) => (Math.Abs(a), Math.Abs(b)) switch
        {
            (_, 0) => a,
            _ => GCD(b, a % b)
        };
    }
}