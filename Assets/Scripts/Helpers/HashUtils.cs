public static class HashUtils
{
    public static int Combine(int a, int b)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + a;
            hash = hash * 31 + b;
            return hash;
        }
    }

    public static int Combine(int a, int b, int c)
    {
        unchecked { int h = 17; h = h * 31 + a; h = h * 31 + b; h = h * 31 + c; return h; }
    }

    public static int Combine(int a, int b, int c, int d)
    {
        unchecked { int h = 17; h = h * 31 + a; h = h * 31 + b; h = h * 31 + c; h = h * 31 + d; return h; }
    }
}