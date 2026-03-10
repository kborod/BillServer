using System;

public readonly struct Fixed64Dub : IComparable<Fixed64Dub>, IEquatable<Fixed64Dub>
{
    const int FRACTIONAL_BITS = 32;
    const long ONE = 1L << FRACTIONAL_BITS;
    const long FRACTION_MASK = ONE - 1;

    public readonly long Raw;

    public Fixed64Dub(long raw)
    {
        Raw = raw;
    }

    // ---------------- constants ----------------

    public static readonly Fixed64Dub Zero = new Fixed64Dub(0);
    public static readonly Fixed64Dub One = new Fixed64Dub(ONE);
    public static readonly Fixed64Dub Half = new Fixed64Dub(ONE >> 1);
    public static readonly Fixed64Dub Two = new Fixed64Dub(ONE << 1);
    public static readonly Fixed64Dub Three = FromInt(3);
    public static readonly Fixed64Dub Four = FromInt(4);
    public static readonly Fixed64Dub Five = FromInt(5);

    public static readonly Fixed64Dub Pi = FromDouble(3.14159265358979323846);
    public static readonly Fixed64Dub TwoPi = FromDouble(6.28318530717958647692);
    public static readonly Fixed64Dub PiHalf = FromDouble(1.5707963267948966);

    // ---------------- conversion ----------------

    public static Fixed64Dub FromInt(int v)
    {
        return new Fixed64Dub((long)v << FRACTIONAL_BITS);
    }

    public int ToInt()
    {
        return (int)(Raw >> FRACTIONAL_BITS);
    }

    public static Fixed64Dub FromFloat(float v)
    {
        return new Fixed64Dub((long)(v * ONE));
    }

    public float ToFloat()
    {
        return (float)Raw / ONE;
    }

    public static Fixed64Dub FromDouble(double v)
    {
        return new Fixed64Dub((long)(v * ONE));
    }

    public double ToDouble()
    {
        return (double)Raw / ONE;
    }

    // ---------------- operators ----------------

    public static Fixed64Dub operator +(Fixed64Dub a, Fixed64Dub b)
        => new Fixed64Dub(a.Raw + b.Raw);

    public static Fixed64Dub operator -(Fixed64Dub a, Fixed64Dub b)
        => new Fixed64Dub(a.Raw - b.Raw);

    public static Fixed64Dub operator -(Fixed64Dub v)
        => new Fixed64Dub(-v.Raw);

    public static bool operator >(Fixed64Dub a, Fixed64Dub b) => a.Raw > b.Raw;
    public static bool operator <(Fixed64Dub a, Fixed64Dub b) => a.Raw < b.Raw;
    public static bool operator >=(Fixed64Dub a, Fixed64Dub b) => a.Raw >= b.Raw;
    public static bool operator <=(Fixed64Dub a, Fixed64Dub b) => a.Raw <= b.Raw;
    public static bool operator ==(Fixed64Dub a, Fixed64Dub b) => a.Raw == b.Raw;
    public static bool operator !=(Fixed64Dub a, Fixed64Dub b) => a.Raw != b.Raw;

    // ---------------- safe multiply ----------------

    public static Fixed64Dub operator *(Fixed64Dub a, Fixed64Dub b)
    {
        long ah = a.Raw >> 32;
        long al = a.Raw & 0xffffffff;

        long bh = b.Raw >> 32;
        long bl = b.Raw & 0xffffffff;

        long hi = ah * bh;
        long mid1 = ah * bl;
        long mid2 = al * bh;
        long lo = al * bl;

        long mid = mid1 + mid2;

        long result =
            (hi << 32) +
            mid +
            (lo >> 32);

        return new Fixed64Dub(result);
    }

    // ---------------- division ----------------

    public static Fixed64Dub operator /(Fixed64Dub a, Fixed64Dub b)
    {
        if (b.Raw == 0)
            throw new DivideByZeroException();

        bool negative = (a.Raw ^ b.Raw) < 0;

        ulong ua = (ulong)Math.Abs(a.Raw);
        ulong ub = (ulong)Math.Abs(b.Raw);

        ulong result = Div128By64(ua, ub);

        long signed = (long)result;
        if (negative) signed = -signed;

        return new Fixed64Dub(signed);
    }

    static ulong Div128By64(ulong a, ulong b)
    {
        // формируем 128-битный числитель
        ulong hi = a >> 32;
        ulong lo = a << 32;

        ulong result = 0;
        ulong rem = hi;

        for (int i = 0; i < 64; i++)
        {
            rem = (rem << 1) | (lo >> 63);
            lo <<= 1;

            result <<= 1;

            if (rem >= b)
            {
                rem -= b;
                result |= 1;
            }
        }

        return result;
    }

    // ---------------- math helpers ----------------

    public static Fixed64Dub Abs(Fixed64Dub v)
    {
        return v.Raw < 0 ? new Fixed64Dub(-v.Raw) : v;
    }

    public static Fixed64Dub Min(Fixed64Dub a, Fixed64Dub b)
    {
        return a.Raw < b.Raw ? a : b;
    }

    public static Fixed64Dub Max(Fixed64Dub a, Fixed64Dub b)
    {
        return a.Raw > b.Raw ? a : b;
    }

    public static Fixed64Dub Clamp(Fixed64Dub v, Fixed64Dub min, Fixed64Dub max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }

    public static Fixed64Dub Lerp(Fixed64Dub a, Fixed64Dub b, Fixed64Dub t)
    {
        return a + (b - a) * t;
    }

    // ---------------- deterministic sqrt ----------------
    public static Fixed64Dub Sqrt(Fixed64Dub x)
    {
        if (x.Raw <= 0)
            return Zero;

        ulong num = (ulong)x.Raw;

        // нужно вычислить sqrt(num << 32)
        ulong remainder = 0;
        ulong root = 0;

        ulong hi = num >> 32;
        ulong lo = num << 32;

        for (int i = 0; i < 64; i++)
        {
            remainder = (remainder << 2) | (hi >> 62);
            hi = (hi << 2) | (lo >> 62);
            lo <<= 2;

            ulong candidate = (root << 2) + 1;

            if (remainder >= candidate)
            {
                remainder -= candidate;
                root = (root << 1) | 1;
            }
            else
            {
                root <<= 1;
            }
        }

        return new Fixed64Dub((long)root);
    }

    static int LeadingZeroCount(ulong x)
    {
        if (x == 0)
            return 64;

        int n = 0;

        if ((x >> 32) == 0) { n += 32; x <<= 32; }
        if ((x >> 48) == 0) { n += 16; x <<= 16; }
        if ((x >> 56) == 0) { n += 8; x <<= 8; }
        if ((x >> 60) == 0) { n += 4; x <<= 4; }
        if ((x >> 62) == 0) { n += 2; x <<= 2; }
        if ((x >> 63) == 0) { n += 1; }

        return n;
    }

    // ---------------- sin/cos LUT ----------------

    const int LUT_SIZE = 4096;
    static readonly long[] sinLut = GenerateSinLut();

    static long[] GenerateSinLut()
    {
        var lut = new long[LUT_SIZE];

        for (int i = 0; i < LUT_SIZE; i++)
        {
            double angle = (double)i / LUT_SIZE * Math.PI * 2;
            lut[i] = (long)(Math.Sin(angle) * ONE);
        }

        return lut;
    }

    public static Fixed64Dub Sin(Fixed64Dub angle)
    {
        long raw = angle.Raw % TwoPi.Raw;

        if (raw < 0)
            raw += TwoPi.Raw;

        int index = (int)((raw * LUT_SIZE) / TwoPi.Raw);

        return new Fixed64Dub(sinLut[index]);
    }

    public static Fixed64Dub Cos(Fixed64Dub angle)
    {
        return Sin(angle + PiHalf);
    }

    // ---------------- misc ----------------

    public int CompareTo(Fixed64Dub other)
        => Raw.CompareTo(other.Raw);

    public bool Equals(Fixed64Dub other)
        => Raw == other.Raw;

    public override bool Equals(object obj)
        => obj is Fixed64Dub f && f.Raw == Raw;

    public override int GetHashCode()
        => Raw.GetHashCode();

    public override string ToString()
        => ToDouble().ToString();

    public static Fixed64Dub MaxValue
        => new Fixed64Dub(long.MaxValue >> 1);
}