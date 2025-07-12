using System.Runtime.CompilerServices;

namespace MyFirstVectorDb;

public class Document
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public List<string> Chunks { get; set; }

    ///
    public Document(string title, string text)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Text = text;
    }

    public string GetDocumentChunksByIndex(params int[] indexes)
    {
        string chunks = "";
        try
        {
            foreach (var i in indexes)
            {

                chunks += Chunks[i] + "\r\n";
            }
        }
        catch (ArgumentOutOfRangeException)
        {


            return "Not found";
        }


        return chunks;
    }
}

