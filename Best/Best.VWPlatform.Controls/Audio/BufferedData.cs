using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Controls.Audio
{
    public class BufferedData
    {
        private object _lockObj = new object();
        private byte[] _bufferedBytes = null;
        private int _writePosition;
        private int _readPosition;
        private int _byteCount;

        public int ByteCount { get { return _byteCount; } }

        public BufferedData(int pBufLen)
        {
            _bufferedBytes = new byte[pBufLen];
        }

        public int Write(byte[] pBytes, int pOffset, int pCount)
        {
            lock (_lockObj)
            {
                int bytesWritten = 0;
                if (pCount > _bufferedBytes.Length - _byteCount)
                {
                    return 0;
                    //pCount = _bufferedBytes.Length - _byteCount;
                }

                int writeToEnd = Math.Min(_bufferedBytes.Length - _writePosition, pCount);
                Array.Copy(pBytes, pOffset, _bufferedBytes, _writePosition, writeToEnd);
                _writePosition += writeToEnd;
                _writePosition %= _bufferedBytes.Length;
                bytesWritten += writeToEnd;

                if (bytesWritten < pCount)
                {
                    Array.Copy(pBytes, pOffset + bytesWritten, _bufferedBytes, _writePosition, pCount - bytesWritten);
                    _writePosition += (pCount - bytesWritten);
                    bytesWritten = pCount;
                }

                _byteCount += bytesWritten;

                return  bytesWritten;
            }
        }

        public int Read(byte[] pBytes, int pOffset, int pCount)
        {
            lock (_lockObj)
            {
                if (pCount > _byteCount)
                {
                    return 0;
                    //pCount = _byteCount;
                }

                int bytesRead = 0;
                int readToEnd = Math.Min(_bufferedBytes.Length - _readPosition, pCount);
                Array.Copy(_bufferedBytes, _readPosition, pBytes, pOffset, readToEnd);
                bytesRead += readToEnd;
                _readPosition += readToEnd;
                _readPosition %= _bufferedBytes.Length;

                if (bytesRead < pCount)
                {
                    Array.Copy(_bufferedBytes, _readPosition, pBytes, pOffset + bytesRead, pCount - bytesRead);
                    _readPosition += (pCount - bytesRead);
                    bytesRead = pCount;
                }
                _byteCount -= bytesRead;

                return bytesRead;
            }
        }

    }
}
