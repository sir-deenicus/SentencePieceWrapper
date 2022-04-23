// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <sentencepiece_processor.h> 
extern "C" __declspec(dllexport) sentencepiece::SentencePieceProcessor * CreateSentencePieceProcessor()
{
    return new sentencepiece::SentencePieceProcessor();
}

extern "C" __declspec(dllexport) void Dispose(sentencepiece::SentencePieceProcessor * pSProc)
{
    if (pSProc != NULL) {
        delete pSProc;
        pSProc = NULL;
    }
}

extern "C" __declspec(dllexport) void Load(sentencepiece::SentencePieceProcessor * pSProc, const char* fname)
{
    if (pSProc != NULL)
    {
        pSProc->Load(fname);
    }
}

extern "C" __declspec(dllexport) void EncodeSentence(sentencepiece::SentencePieceProcessor * pSProc, const char* sentenceBytes, int* idslen, int* idsout)
{
    if (pSProc != NULL)
    {
        std::vector<int> ids;

        pSProc->Encode(sentenceBytes, &ids);

        *idslen = (int)ids.size();

        std::copy(ids.begin(), ids.end(), idsout);
    }
}

extern "C" __declspec(dllexport) void EncodeSentenceAsPieces(sentencepiece::SentencePieceProcessor * pSProc, const char* sentenceBytes, int* pieceslen, int* bufferlen, unsigned char* textbuffer)
{
    if (pSProc != NULL)
    { 
        std::vector<std::string> piecesVec; 
        int btotlen = 0;
        int c = 0;      

        piecesVec = pSProc->EncodeAsPieces (sentenceBytes);  

        int veclen = (int)piecesVec.size();   
        auto bs = std::make_unique<unsigned char[]>(veclen);

        for (const auto& value : piecesVec) { 
            btotlen = btotlen + value.size();
            bs[c] = static_cast<unsigned char>(value.size()); 
            c++; 
        }
                
        btotlen = btotlen + veclen; 

        auto buffer = new unsigned char[btotlen];        

        int loc = 0;
        for (int i = 0; i < veclen; i ++) {  
            std::string str = piecesVec[i];  
            buffer[loc] = bs[i];
            std::copy(str.begin(), str.end(), buffer + loc + 1);
            loc = loc + bs[i] + 1; 
        } 

        *pieceslen = veclen;
        *bufferlen = btotlen; 
        memcpy(textbuffer, buffer, btotlen);
    }
}

extern "C" __declspec(dllexport) void DecodeSentence(sentencepiece::SentencePieceProcessor * pSProc, const int len, const int* ids, int* textlen, unsigned char* textbuffer)
{
    if (pSProc != NULL)
    {
        std::string text;
        std::vector<int> idsvec(ids, ids + len);
        
        pSProc->Decode(idsvec, &text);

        *textlen = (int)text.length();

        std::copy(text.begin(), text.end(), textbuffer);
    }
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    return TRUE;
}

