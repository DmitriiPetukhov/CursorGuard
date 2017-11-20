using System;

namespace CursorGuard
{
    public class ForegroundWindowInfo
    {
        public IntPtr Handle { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }
    }
}