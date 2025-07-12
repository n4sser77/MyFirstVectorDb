using System.Security.Cryptography.X509Certificates;

namespace MyFirstVectorDb;

public readonly record struct VectorRecord(string Id, float[] Vector)
{

}



