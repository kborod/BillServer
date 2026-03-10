using System;

public readonly struct Fixed64 : IComparable<Fixed64>, IEquatable<Fixed64>
{
    const int FRACTIONAL_BITS = 32;
    const long ONE = 1L << FRACTIONAL_BITS;
    const long FRACTION_MASK = ONE - 1;

    public readonly long Raw;

    public Fixed64(long raw)
    {
        Raw = raw;
    }

    // ---------------- constants ----------------

    public static readonly Fixed64 Zero = new Fixed64(0);
    public static readonly Fixed64 One = new Fixed64(ONE);
    public static readonly Fixed64 Half = new Fixed64(ONE >> 1);
    public static readonly Fixed64 Two = new Fixed64(ONE << 1);
    public static readonly Fixed64 Three = FromInt(3);
    public static readonly Fixed64 Four = FromInt(4);
    public static readonly Fixed64 Five = FromInt(5);

    public static readonly Fixed64 Pi = FromDouble(3.14159265358979323846);
    public static readonly Fixed64 TwoPi = FromDouble(6.28318530717958647692);
    public static readonly Fixed64 PiHalf = FromDouble(1.5707963267948966);

    // ---------------- conversion ----------------

    public static Fixed64 FromInt(int v)
    {
        return new Fixed64((long)v << FRACTIONAL_BITS);
    }

    public int ToInt()
    {
        return (int)(Raw >> FRACTIONAL_BITS);
    }

    public static Fixed64 FromFloat(float v)
    {
        return new Fixed64((long)MathF.Round(v * ONE));
        //return new Fixed64((long)(v * ONE));
    }

    public float ToFloat()
    {
        return (float)Raw / ONE;
    }

    public static Fixed64 FromDouble(double v)
    {
        return new Fixed64((long)Math.Round(v * ONE));
        //return new Fixed64((long)(v * ONE));
    }

    public double ToDouble()
    {
        return (double)Raw / ONE;
    }

    // ---------------- operators ----------------

    public static Fixed64 operator +(Fixed64 a, Fixed64 b)
        => new Fixed64(a.Raw + b.Raw);

    public static Fixed64 operator -(Fixed64 a, Fixed64 b)
        => new Fixed64(a.Raw - b.Raw);

    public static Fixed64 operator -(Fixed64 v)
        => new Fixed64(-v.Raw);

    public static bool operator >(Fixed64 a, Fixed64 b) => a.Raw > b.Raw;
    public static bool operator <(Fixed64 a, Fixed64 b) => a.Raw < b.Raw;
    public static bool operator >=(Fixed64 a, Fixed64 b) => a.Raw >= b.Raw;
    public static bool operator <=(Fixed64 a, Fixed64 b) => a.Raw <= b.Raw;
    public static bool operator ==(Fixed64 a, Fixed64 b) => a.Raw == b.Raw;
    public static bool operator !=(Fixed64 a, Fixed64 b) => a.Raw != b.Raw;

    // ---------------- safe multiply ----------------

    public static Fixed64 operator *(Fixed64 a, Fixed64 b)
    {
        long aRaw = a.Raw;
        long bRaw = b.Raw;

        ulong aLo = (uint)aRaw;
        long aHi = aRaw >> 32;

        ulong bLo = (uint)bRaw;
        long bHi = bRaw >> 32;

        ulong lo = aLo * bLo;
        long mid1 = aHi * (long)bLo;
        long mid2 = (long)aLo * bHi;
        long hi = aHi * bHi;

        long result =
            (hi << 32) +
            mid1 +
            mid2 +
            (long)(lo >> 32);

        return new Fixed64(result);
    }

    //public static Fixed64 operator *(Fixed64 a, Fixed64 b)
    //{
    //    long ah = a.Raw >> 32;
    //    long al = a.Raw & 0xffffffff;

    //    long bh = b.Raw >> 32;
    //    long bl = b.Raw & 0xffffffff;

    //    long hi = ah * bh;
    //    long mid1 = ah * bl;
    //    long mid2 = al * bh;
    //    long lo = al * bl;

    //    long mid = mid1 + mid2;

    //    long result =
    //        (hi << 32) +
    //        mid +
    //        (lo >> 32);

    //    return new Fixed64(result);
    //}

    // ---------------- division ----------------

    public static Fixed64 operator /(Fixed64 a, Fixed64 b)
    {
        if (b.Raw == 0)
            throw new DivideByZeroException();

        bool negative = (a.Raw ^ b.Raw) < 0;

        ulong ua = (ulong)Math.Abs(a.Raw);
        ulong ub = (ulong)Math.Abs(b.Raw);

        ulong result = Div128By64(ua, ub);

        long signed = (long)result;
        if (negative) signed = -signed;

        return new Fixed64(signed);
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

    public static Fixed64 Abs(Fixed64 v)
    {
        return v.Raw < 0 ? new Fixed64(-v.Raw) : v;
    }

    public static Fixed64 Min(Fixed64 a, Fixed64 b)
    {
        return a.Raw < b.Raw ? a : b;
    }

    public static Fixed64 Max(Fixed64 a, Fixed64 b)
    {
        return a.Raw > b.Raw ? a : b;
    }

    public static Fixed64 Clamp(Fixed64 v, Fixed64 min, Fixed64 max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }

    public static Fixed64 Lerp(Fixed64 a, Fixed64 b, Fixed64 t)
    {
        return a + (b - a) * t;
    }

    // ---------------- deterministic sqrt ----------------
    public static Fixed64 Sqrt(Fixed64 x)
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

        return new Fixed64((long)root);
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

    public static Fixed64 Sin(Fixed64 angle)
    {
        long raw = angle.Raw % TwoPi.Raw;

        if (raw < 0)
            raw += TwoPi.Raw;

        int index = (int)((raw * LUT_SIZE) / TwoPi.Raw);

        return new Fixed64(sinLut[index]);
    }

    public static Fixed64 Cos(Fixed64 angle)
    {
        return Sin(angle + PiHalf);
    }

    // ---------------- misc ----------------

    public int CompareTo(Fixed64 other)
        => Raw.CompareTo(other.Raw);

    public bool Equals(Fixed64 other)
        => Raw == other.Raw;

    public override bool Equals(object obj)
        => obj is Fixed64 f && f.Raw == Raw;

    public override int GetHashCode()
        => Raw.GetHashCode();

    public override string ToString()
        => ToDouble().ToString();

    public static Fixed64 MaxValue
        //=> new Fixed64(long.MaxValue >> 1);
        => FromInt(16777216);
}