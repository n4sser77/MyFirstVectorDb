namespace MyFirstVectorDb;

public record struct ChunkRecord(string DocumentId, float[] Embedding, int ChunkIndex)
{
   public string ChunkMetaData => $"Doc: {DocumentId}, Chunk: {ChunkIndex}";

}



