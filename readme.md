# 🧠 MyFirstVectorDb

> A local vector store built from scratch in C#, designed to explore how vector databases, semantic search, chunking, and LLM-based retrieval-augmented generation (RAG) work using real embeddings and local models.

---

## ✅ Project Overview

**MyFirstVectorDb** is a console-based prototype of a lightweight vector database that:

* Stores text chunks and their embeddings
* Performs similarity search using cosine similarity
* Connects to a local Ollama embedding model
* Retrieves relevant chunks based on a user query
* Feeds the result into a local LLM to generate meaningful answers based only on those chunks (RAG-style flow)

---

## 📦 Core Components

### ✅ `VectorStore`

The main "database client" interface.

* Stores all documents and chunk records.
* Performs similarity search via cosine similarity.
* Allows retrieval of full documents by ID.

---

### ✅ `Document`

Represents a full document in the store.

* Has an `Id`, a `Title`, and a `List<string>` of **chunks** (split paragraphs or sentences).
* Chunking is done at load time using the [`TextChunker`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.text.textchunker) from Semantic Kernel.
* Does not store the full raw text — only chunks.

---

### ✅ `ChunkRecord`

Represents a single chunk of a document.

* Contains:

  * `DocumentId`: which document it belongs to
  * `float[] Embedding`: numeric representation of the chunk
  * `ChunkIndex`: position of the chunk in the document

Each chunk is what gets compared during search.

---

### ✅ `VectorMath`

Holds the cosine similarity logic used to compare vectors:

```csharp
return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
```

This measures how "close" two vectors are in high-dimensional space, meaning how semantically similar two pieces of text are.

---

### ✅ `VectorRecord`

An **older version** of `ChunkRecord`.
Used before we introduced chunking, and simply stored full strings and their embeddings.
Now deprecated in favor of chunk-based storage and retrieval.

---

## 📚 What I Learned

### 🧭 What is a Vector Database?

A vector database stores **high-dimensional numerical vectors** derived from text, images, audio, or any data that can be embedded. These vectors are searched using similarity measures like cosine similarity, not by keyword matching.

---

### 🧠 What is Embedding?

**Embedding** is the process of transforming text into a vector (a list of float numbers) that captures semantic meaning. For example:

```
"How do I cook pasta?" → [0.12, -0.03, 0.85, ...]
```

If two vectors are close in space, their text is semantically similar.

We used Ollama’s `granite-embedding:30m` model to generate these embeddings locally.

---

### 🔍 What is Similarity Search?

When a user asks a question, it is also embedded into a vector. That vector is then compared to all stored vectors using cosine similarity.

The **top-k** closest vectors are retrieved, and their associated text chunks are considered relevant to the query.

---

### 🔄 Chunking

Instead of embedding an entire document, we:

* Split the document into manageable chunks using `TextChunker.SplitPlainTextLines(doc.Text, 64);` (max 64 tokens per line)
* Embed each chunk separately
* Store chunk index and its relation to the full document

This allows **finer-grained search** and avoids exceeding LLM context limits.

---

### 🧩 Using the LLM for Answers (RAG)

Once we retrieved the most relevant chunks:

1. We created a **prompt** that gave the LLM only the chunked content.
2. We set a **system instruction** telling the model to only answer based on those chunks.
3. We let the model generate a response to the user’s question.

This mirrors how real RAG systems like ChatGPT with retrieval or LangChain pipelines work.

---

## 🧪 Sample Prompt to the LLM

```plaintext
System:
You are an AI assistant. Answer based only on the provided context.

Context:
Chunk 0: Spaghetti is a type of pasta...
Chunk 3: Tomato sauce is the classic pairing...

User:
What is a great add-on to spaghetti?
```

The model uses only this context to answer. If nothing relevant is found, it is instructed to say so.

---

## 🛠️ Technical Setup

* Language: C# 13 (.NET 9 )
* Local LLM: Ollama (with `granite-embedding:30m` and `deepseek:8b`)
* Embedding: Real, dense float\[] vectors
* Chunking: `TextChunker.SplitPlainTextLines()` from Semantic Kernel
* Search: Brute-force cosine similarity on `float[]`
* Interaction: Console + `OllamaSharp`

---

## 🤔 Reflections and Takeaways

### ✅ What went well

* C# is perfectly capable of building a vector database.
* Ollama models were easy to work with.
* Building from scratch helped me **truly understand** embeddings, search, and chunking.

### 🧱 What could be improved

* Add persistence (e.g., JSON or SQLite)
* Handle large corpora (e.g., streaming and chunk windowing)
* Add token estimation and truncation to handle large LLM prompts
* Add top-k reranking (advanced scoring, multi-vector fusion)

---

## 🚀 Next Steps

* 🔄 Add saving/loading the vector store
* 🔎 Build a REST API around the DB
* 💬 Support OpenAI/LLM chat backends with consistent schema
* 📊 Add metrics (embedding speed, search time, etc.)
* 🧠 Add multi-document search + metadata filters

---
