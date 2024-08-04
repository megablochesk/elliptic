using ELO.ECDH;
using ELO.StatisticsCollector;

const int sampleSize = 1000;

GenerateECDHStatistics();
return;

void GenerateECDHStatistics()
{
    var privateKeys = GeneratePrivateKeysList(sampleSize);

    foreach (AlgorithmType type in Enum.GetValues(typeof(AlgorithmType)))
    {
        ProcessAlgorithmType(type, privateKeys);
    }
}

void ProcessAlgorithmType(AlgorithmType type, List<(BigInteger, BigInteger)> privateKeys)
{
    var ecdh = ECDHFactory.CreateECDH(type);
    var filePath = $@"D:\Work\{type.ToString()}.txt";
    var collector = new TimeStatisticsCollector(filePath);

    foreach (var (aliceKeys, bobKeys) in privateKeys)
    {
        collector.RunAndMeasure(() => RunECDHExchange(aliceKeys, bobKeys, ecdh));
    }
}

void RunECDHExchange(BigInteger alicePrivateKey, BigInteger bobPrivateKey, IECDH ecdh)
{
    var alicePublicKey = ecdh.GeneratePublicKey(alicePrivateKey);
    var bobPublicKey = ecdh.GeneratePublicKey(bobPrivateKey);

    var aliceSharedSecret = ecdh.DeriveSharedSecret(alicePrivateKey, bobPublicKey);
    var bobSharedSecret = ecdh.DeriveSharedSecret(bobPrivateKey, alicePublicKey);
}

List<(BigInteger, BigInteger)> GeneratePrivateKeysList(int size)
{
    var result = new List<(BigInteger, BigInteger)>();

    for (var i = 0; i < size; i++)
    {
        var a = ECDH.GeneratePrivateKey();
        var b = ECDH.GeneratePrivateKey();

        result.Add((a, b));
    }

    return result;
}