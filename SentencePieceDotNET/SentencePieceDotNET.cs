using System;
using System.Runtime.InteropServices;
using System.Text;

//Follows rough scaffold in https://www.codeproject.com/Articles/18032/How-to-Marshal-a-C-Class 

namespace SentencePieceDotNET
{
    public class SentencePieceDotNET : IDisposable
    {
        const string dllname = "SentencePieceWrapper.dll";

        [DllImport(dllname)]
        static private extern IntPtr CreateSentencePieceProcessor();

        [DllImport(dllname)]
        static private extern void Dispose(IntPtr pointer);

        [DllImport(dllname)]
        static private extern void Load(IntPtr pointer, string path);

        [DllImport(dllname)]
        static private extern void EncodeSentence(IntPtr pSProc, byte[] sentenceBytes, out int idslen, int[] idsout);

        [DllImport(dllname)]
        static private extern void DecodeSentence(IntPtr pSProc, int len, int[] ids, out int textlen, byte[] textbuffer);

        private IntPtr sentencePieceProcessor;

        public SentencePieceDotNET()
        {
            this.sentencePieceProcessor = CreateSentencePieceProcessor();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Load(string path)
        {
            Load(sentencePieceProcessor, path);
        }

        public int[] Encode(string sentence)
        {
            var b = Encoding.Unicode.GetBytes(sentence);
            var bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, b);
            var buffer = new int[bytes.Length];

            EncodeSentence(sentencePieceProcessor, bytes, out int len, buffer);
            return buffer[0..len];
        } 

        public string Decode(int[] ids)
        {
            var textbuffer = new byte[ids.Length * 50];
            DecodeSentence(sentencePieceProcessor, ids.Length, ids, out int len, textbuffer);

            return Encoding.UTF8.GetString(textbuffer, 0, len);
        } 

        protected virtual void Dispose(bool bDisposing)
        {
            if (this.sentencePieceProcessor != IntPtr.Zero)
            {
                Dispose(this.sentencePieceProcessor);
                this.sentencePieceProcessor = IntPtr.Zero;
            }

            if (bDisposing)
            {
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory
                GC.SuppressFinalize(this);
            }
        }

        // This finalizer is called when Garbage collection occurs, but only if
        // the IDisposable.Dispose method wasn't already called.
        ~SentencePieceDotNET()
        {
            Dispose(false);
        }
    }
}
