using ELO.Points;
using System.Numerics;

namespace ELO;

public static class Curve
{
    public static readonly BigInteger P = BigInteger.Parse("115792089210356248762697446949407573530086143415290314195533631308867097853951", System.Globalization.NumberStyles.Integer);
    public static readonly BigInteger A = -3;
    public static readonly BigInteger B = BigInteger.Parse("41058363725152142129326129780047268409114441015993725554835256314039467401291", System.Globalization.NumberStyles.Integer);
    public static readonly BigInteger N = BigInteger.Parse("115792089210356248762697446949407573529996955224135760342422259061068512044369", System.Globalization.NumberStyles.Integer);

    public static readonly BigInteger Gx = BigInteger.Parse("6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", System.Globalization.NumberStyles.HexNumber);
    public static readonly BigInteger Gy = BigInteger.Parse("4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", System.Globalization.NumberStyles.HexNumber);
    public static readonly AffinePoint G = new(Gx, Gy);
}