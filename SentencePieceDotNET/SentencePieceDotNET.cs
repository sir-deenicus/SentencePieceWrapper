using System;
using System.Runtime.InteropServices;
using System.Text;

//Follows rough scaffold in https://www.codeproject.com/Articles/18032/How-to-Marshal-a-C-Class 

namespace SentencePieceDotNET
{
    public class SentencePieceDotNET : IDisposable
    {
        const string dllname = "SentencePieceWrapper.dll";

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]  
        static private extern IntPtr CreateSentencePieceProcessor();

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern void Dispose(IntPtr pointer);

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static private extern int Load(IntPtr pointer, string path);

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern IntPtr EncodeSentence(IntPtr pSProc, byte[] sentenceBytes, out int idslen);   

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern IntPtr EncodeSentenceAsPieces(IntPtr pSProc, byte[] sentenceBytes, out int pieceslen, out int bufferlen);  

        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern IntPtr DecodeSentence(IntPtr pSProc, int len, int[] ids, out int textlen); 
  
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern void FreeBuffer(IntPtr buffer);
 
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        static private extern void FreeIdsBuffer(IntPtr sentence);

        private IntPtr sentencePieceProcessor;

        public SentencePieceDotNET()
        {
            this.sentencePieceProcessor = CreateSentencePieceProcessor();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Load(string path)
        {
            return Load(sentencePieceProcessor, path) == 1;
        }

        /// <summary>
        /// Encodes the given sentence into an array of integer token ids.
        /// </summary>
        /// <param name="sentence">The sentence to encode.</param>
        /// <returns>An array of integer token ids representing the encoded sentence.</returns> 
        public int[] Encode(string sentence)
        {
            var b = Encoding.Unicode.GetBytes(sentence);
            var bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, b);

            var encodedPtr = EncodeSentence(sentencePieceProcessor, bytes, out int len); 
            if (encodedPtr == IntPtr.Zero)
            {
                return new int[0];
            }

            var ids = new int[len];
            Marshal.Copy(encodedPtr, ids, 0, len);

            //Free mem allocated in the C++ code
            FreeIdsBuffer(encodedPtr);

            return ids; 
        } 

        /// <summary>
        /// Encodes the given sentence into an array of sentence pieces.
        /// </summary>
        /// <param name="sentence">The sentence to encode.</param>
        /// <returns>An array of subword sentence pieces representing the encoded sentence.</returns>
        public string[] EncodeAsPieces(string sentence)
        {
            var b = Encoding.Unicode.GetBytes(sentence);
            var bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, b);
            
            var buffptr = EncodeSentenceAsPieces(sentencePieceProcessor, bytes, out int piecesLength, out int bufferLength);
            if (buffptr == IntPtr.Zero)
            {
                return new string[0];
            }
            var buffer = new byte[bufferLength];

            Marshal.Copy(buffptr, buffer, 0, bufferLength);
            FreeBuffer(buffptr);
             
            var packed = buffer[0..bufferLength]; 
            var stringout = new string[piecesLength]; 
            int loc = 0;
            for(int i = 0; i < piecesLength; i++)
            {
                byte len = packed[loc]; 
                stringout[i] = Encoding.UTF8.GetString(packed, loc + 1, len);
                loc = loc + len + 1; 
            } 
            
            return stringout; 
        }

        /// <summary>
        /// Decodes the given array of token ids into a sentence.
        /// </summary>
        /// <param name="ids">The array of token (ids) to decode.</param>
        /// <returns>The decoded sentence.</returns>    
        public string Decode(int[] ids)
        {
            var txtbufferpntr = DecodeSentence(sentencePieceProcessor, ids.Length, ids, out int len);
            if (txtbufferpntr == IntPtr.Zero)
            {
                return string.Empty;
            }
            var textbuffer = new byte[len];
            
            Marshal.Copy(txtbufferpntr, textbuffer, 0, len);

            FreeBuffer(txtbufferpntr);

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
