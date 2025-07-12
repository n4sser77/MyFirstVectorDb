namespace MyFirstVectorDb;

/// <summary>
/// Vector store client db, currently only on memory
/// </summary>
public class VectorStore
{
   private readonly List<ChunkRecord> _records = new();
   private readonly List<Document> _documents = new();



   /// <summary>
   /// Adds a ChunkRecord to the vector store
   /// </summary>
   /// <param  name="record"></param>
   public void Add(ChunkRecord record) => _records.Add(record);
   /// <summary>
   /// Adds a full document object
   /// </summary>
   /// <param name="doc"></param>
   public void AddDocument(Document doc) => _documents.Add(doc);


   /// <summary>
   /// Search the vector store
   /// </summary>
   /// <param name="queryEmbedding">the embedded string to be looked up on the store</param>
   /// <param name="topK">Limits the result count</param>
   /// <returns></returns>
   public List<(ChunkRecord Record, float Score)> Search(float[] queryEmbedding, int topK = 3)
   {
      var results = new List<(ChunkRecord Record, float Score)>();

      foreach (var record in _records)
      {
         float score = VectorMath.CosineSimilarity(queryEmbedding, record.Embedding);
         results.Add((record, score));
      }

      return results
            .OrderByDescending(r => r.Score).Take(topK).ToList();
   }

   /// <summary>
   /// Gets a document by its id
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   public Document? GetDocumentById(string id)
   {
      return _documents.FirstOrDefault(d => d.Id == id);
   }




}
