using System;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using Microsoft.VisualBasic;
using MyFirstVectorDb;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;
using OpenAI.Embeddings;


VectorStore store = new VectorStore();

OllamaApiClient ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "granite-embedding:30m");
bool isOn = await ollama.IsRunningAsync();
Console.WriteLine("\r\nOllama is running: " + isOn + " selected modal: " + ollama.SelectedModel);

// foreach (var input in SampleText.Inputs)
// {
//    // var embeddings = DummyEmbedder.Embed(input);
//    var ollamaEmb = await ollama.EmbedAsync(input);
//    // System.Console.WriteLine(ollamaEmb.Embeddings[0]);
//    store.Add(new VectorRecord(input, ollamaEmb.Embeddings[0]));
// }
MyFirstVectorDb.Document doc = new MyFirstVectorDb.Document("Spaghetti Guide", SampleText.longDocument);
store.AddDocument(doc);


#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
List<string> chunks = TextChunker.SplitPlainTextLines(doc.Text, 64);
doc.Chunks = chunks;
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

for (int i = 0; i < chunks.Count(); i++)
{
   var emb = await ollama.EmbedAsync(chunks[i]);
   var chunkRecord = new ChunkRecord(doc.Id, emb.Embeddings[0], i);
   store.Add(chunkRecord);
}

string userPrompt = "What is a great add-on to spaghetti?";
EmbedResponse query = await ollama.EmbedAsync(userPrompt);

List<(ChunkRecord Record, float score)> results = store.Search(query.Embeddings[0], topK: 3);
List<int> releventChunkIndexes = new();
Console.WriteLine("Top chunk matches:");
foreach (var (record, score) in results)
{
   Console.WriteLine($"  ChunkMetaData: {record.ChunkMetaData}  score: {score:F4}");
   releventChunkIndexes.Add(record.ChunkIndex);
}
var sysPrompt = """
You are an ai assistent, 
that can only answer based on text provided to you.
 You are not allowed to anwser outside the context provided to you, only if you think the documents can still be relevent to the user.
 summerize the texts and give an answer to the user only if you find it relevent to the users requst.
  If there is no matching documents t othe user query you tell them that you have not found what the user is looking for,
  and explain what documents you have.
""";
var retrivedDocVector = results[0];
var retrivedDoc = store.GetDocumentById(retrivedDocVector.Record.DocumentId);

if (retrivedDoc is null)
{
   Console.WriteLine("No doucment was found");
   return;
}

var chatStream = ollama.ChatAsync(new()
{
   Model = "deepseek-r1:8b",
   Think = false,

   Messages = [
      new(){
         Role = OllamaSharp.Models.Chat.ChatRole.System,
         Content = sysPrompt,
      },
      new() {
         Role = OllamaSharp.Models.Chat.ChatRole.User ,
         Content = userPrompt,
      },
      new() {
         Role = OllamaSharp.Models.Chat.ChatRole.System,
         Content = $" context: [DocumentId: {retrivedDoc.Id}, Title: {retrivedDoc.Title}, Content: { retrivedDoc.GetDocumentChunksByIndex(releventChunkIndexes.ToArray()) } ]"
      }
   ]
});

var response = await chatStream.StreamToEndAsync();
if (response is null)
{
   Console.WriteLine("No response");
   return;
}
Console.WriteLine(response.Message.Content);
