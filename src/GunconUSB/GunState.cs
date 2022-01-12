namespace GunconUSB
{
    internal static class GunState
    {
        public static bool BtnA;
        public static bool BtnB;
        public static bool BtnC;
        public static bool Trigger;
        public static bool Start;
        public static bool Select;

        public static sbyte PadX;
        public static sbyte PadY;

        public static int PointerX;
        public static int PointerY;

        public static int MinX = int.MaxValue;
        public static int MinY = int.MaxValue;
        public static int MaxX = int.MinValue;
        public static int MaxY = int.MinValue;
    }
}
