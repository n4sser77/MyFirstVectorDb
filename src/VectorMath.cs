namespace MyFirstVectorDb;

public static class VectorMath
{
   public static float CosineSimilarity(Span<float> vectorA, Span<float> vectorB)
   {
      if (vectorA.Length != vectorB.Length)
      {
         throw new ArgumentException("Vectors must be of the same length.");
      }

      float dot = 0f, normA = 0f, normB = 0f;

      for (int i = 0; i < vectorA.Length; i++)
      {
         dot += vectorA[i] * vectorB[i];
         normA += vectorA[i] * vectorA[i];
         normB += vectorB[i] * vectorB[i];
      }

      return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
   }
}
