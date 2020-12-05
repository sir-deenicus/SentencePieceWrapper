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

