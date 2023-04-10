using System;
using CSM.API;

namespace CSM.Helpers
{
    public class WorldFileCombiner
    {
        
        private readonly byte[] _worldFile;
        public int RemainingBytes { get; private set; }

        public WorldFileCombiner(byte[] slice, int remainingBytes)
        {
            _worldFile = new byte[slice.Length + remainingBytes];
            Array.Copy(slice, 0, _worldFile, 0, slice.Length);
            RemainingBytes = remainingBytes;
        }

        public void AddSlice(byte[] slice, int remainingBytes)
        {
            int start = _worldFile.Length - RemainingBytes;
            Array.Copy(slice, 0, _worldFile, start, slice.Length);
            if (RemainingBytes - slice.Length != remainingBytes)
            {
                Log.Warn("Calculated and received remaining bytes didn't match.");
            }
            RemainingBytes = remainingBytes;
        }

        public byte[] GetWorldFile()
        {
            return RemainingBytes == 0 ? _worldFile : new byte[0];
        }

        public int GetTotalBytes()
        {
            return _worldFile.Length;
        }
    }
    
    public class WorldFileSlicer
    {
        private const int ChunkSize = 32768;

        private readonly byte[] _worldFile;

        private int _cursor = 0;

        public int RemainingBytes { get; private set; }

        public WorldFileSlicer(byte[] worldFile)
        {
            _worldFile = worldFile;
            RemainingBytes = worldFile.Length;
        }

        public byte[] Take()
        {
            if (RemainingBytes <= 0)
            {
                return new byte[0];
            }

            int length = ChunkSize < RemainingBytes ? ChunkSize: RemainingBytes;

            byte[] dest = new byte[length];

            Array.Copy(_worldFile, _cursor, dest, 0, length);

            RemainingBytes -= length;
            _cursor += ChunkSize;

            return dest;
        }
    }

    public class FileSizeHelper
    {
        public static String FormatFilesizeIEC(double size, int exp)
        {
            if (size >= 1024)
            {
                return FormatFilesizeIEC(size / 1024, exp + 3);
            }
            return $"{size:0.##} {GetPrefix(exp)}iB";
        }

        private static String GetPrefix(int exp)
        {
            switch (exp)
            {
                case 0:
                    return "";
                case 3:
                    return "k";
                case 6:
                    return "M";
                case 9:
                    return "G";
                case 12:
                    return "T";
                case 15:
                    return "P";
                default:
                    return "";
            }
        }
    }
}
