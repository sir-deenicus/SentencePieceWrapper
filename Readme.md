# SentencePieceDotNET

This is a barebones wrapper around the [SentencePiece](https://github.com/google/sentencepiece) tokenizer library for .NET. The library provides a simple and efficient way to tokenize sentences using the SentencePiece model.

## Features

The library currently supports the following operations:

- **Encoding sentences**: You can encode a sentence into an array of pieces or an array of integer IDs.
- **Decoding IDs**: You can decode an array of integer IDs back into a sentence.

## Usage

Here are the main methods provided by the library:

- `string[] EncodeAsPieces(string sentence)`

- `int[] Encode(string sentence)`

- `string Decode(int[] ids)`
  
## Limitations

The current implementation only supports encoding and decoding operations. Other features of the SentencePiece library, such as training new models, are not currently supported.

*Current ver: v0.2.0 (Feb 19 2024)*