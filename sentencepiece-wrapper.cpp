#include <sentencepiece_processor.h> 

extern "C" __declspec(dllexport) void FreeBuffer(unsigned char* buffer) {
    if(buffer != nullptr){ 
        delete[] buffer;
    }
}

extern "C" __declspec(dllexport) void FreeIdsBuffer(int* sentence) {
    if (sentence != nullptr) {
        delete[] sentence;
    }
}

extern "C" __declspec(dllexport) void Dispose(sentencepiece::SentencePieceProcessor * pSProc)
{
    if (pSProc != nullptr) {
        delete pSProc;
    }
} 

extern "C" __declspec(dllexport) sentencepiece::SentencePieceProcessor * CreateSentencePieceProcessor()
{
    return new sentencepiece::SentencePieceProcessor();
}

extern "C" __declspec(dllexport) int Load(sentencepiece::SentencePieceProcessor * pSProc, const char* fname)
{
    if (pSProc != nullptr)
    {
        auto status = pSProc->Load(fname);
        return status.ok() ? 1 : 0;
    }
    else
    {
        return 0;
    }
}

extern "C" __declspec(dllexport) int* EncodeSentence(sentencepiece::SentencePieceProcessor * pSProc, const char* sentenceBytes, int* idslen)
{
    if (pSProc != nullptr)
    {
        std::vector<int> ids;

        pSProc->Encode(sentenceBytes, &ids);

        *idslen = (int)ids.size();

        int* idsout = new int[*idslen];
        std::copy(ids.begin(), ids.end(), idsout);

        return idsout;
    }
    else
    {
        *idslen = 0;
        return nullptr;
    }
}

extern "C" __declspec(dllexport) unsigned char* EncodeSentenceAsPieces(sentencepiece::SentencePieceProcessor * pSProc, const char* sentenceBytes, int* pieceslen, int* bufferlen)
{
    unsigned char* buffer = nullptr;
    if (pSProc != nullptr)
    { 
        std::vector<std::string> piecesVec = pSProc->EncodeAsPieces(sentenceBytes);  

        int veclen = (int)piecesVec.size();   
        auto pieceSizes = std::make_unique<unsigned char[]>(veclen);

        int totalBufferLength = 0;
        int c = 0;
        for (const auto& value : piecesVec) { 
            pieceSizes[c] = value.size();
            totalBufferLength += pieceSizes[c] + 1; // +1 for the length byte
            c++;
        }

        buffer = new unsigned char[totalBufferLength];        

        int loc = 0;
        for (int i = 0; i < veclen; i ++) {  
            std::string str = piecesVec[i];  
            buffer[loc] = pieceSizes[i];
            std::copy(str.begin(), str.end(), buffer + loc + 1);
            loc += pieceSizes[i] + 1; 
        } 

        *pieceslen = veclen;
        *bufferlen = totalBufferLength; 
    }
    return buffer;
}

extern "C" __declspec(dllexport) unsigned char* DecodeSentence(sentencepiece::SentencePieceProcessor * pSProc, const int len, const int* ids, int* textlen)
{
    unsigned char* textbuffer = nullptr;
    if (pSProc != nullptr)
    {
        std::string text;
        std::vector<int> idsvec(ids, ids + len);
        
        pSProc->Decode(idsvec, &text);

        *textlen = (int)text.length();

        textbuffer = new unsigned char[*textlen];
        std::copy(text.begin(), text.end(), textbuffer);
    }
    return textbuffer;
}
