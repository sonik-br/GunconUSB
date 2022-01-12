using System;
using System.Collections.Generic;

namespace Guncon3Console
{
    public static class GunState
    {
        public static readonly Dictionary<GunButton, bool> BtnState;

        static GunState()
        {
            var values = Enum.GetValues(typeof(GunButton));
            BtnState = new Dictionary<GunButton, bool>(values.Length);

            foreach (GunButton item in values)
                BtnState.Add(item, false);
        }


        public static long ABS_RY { get; set; }
        public static long ABS_RX { get; set; }
        public static long ABS_HAT0Y { get; set; }
        public static long ABS_HAT0X { get; set; }
        public static short Z { get; set; }
        public static short ABS_Y { get; set; }
        public static short ABS_X { get; set; }

        //public static bool BTN_TRIGGER_LAST { get; set; }

        //private static bool _BTN_TRIGGER;
        //public static bool BTN_TRIGGER
        //{
        //    get { return _BTN_TRIGGER; }
        //    set
        //    {
        //        if (_BTN_TRIGGER != BTN_TRIGGER_LAST)
        //            BTN_TRIGGER_LAST = _BTN_TRIGGER;

        //        _BTN_TRIGGER = value;
        //    }
        //}


        //public static bool A1 { get; set; }
        //public static bool A2 { get; set; }
        //public static bool B1 { get; set; }
        //public static bool B2 { get; set; }



        //public static bool BTN_C1_LAST { get; set; }

        //private static bool _BTN_C1;
        //public static bool C1
        //{
        //    get { return _BTN_C1; }
        //    set
        //    {
        //        if (_BTN_C1 != BTN_C1_LAST)
        //            BTN_C1_LAST = _BTN_C1;

        //        _BTN_C1 = value;
        //    }
        //}


        //public static bool BTN_C2_LAST { get; set; }

        //private static bool _BTN_C2;
        //public static bool C2
        //{
        //    get { return _BTN_C2; }
        //    set
        //    {
        //        if (_BTN_C2 != BTN_C2_LAST)
        //            BTN_C2_LAST = _BTN_C2;

        //        _BTN_C2 = value;
        //    }
        //}



        //public static bool A_STICK_BUTTON { get; set; }
        //public static bool B_STICK_BUTTON { get; set; }



        public static bool INDICATOR1 { get; set; }
        public static bool INDICATOR2 { get; set; }

        public static bool IsInsideScreen { get { return !INDICATOR2; } }
    }
}
