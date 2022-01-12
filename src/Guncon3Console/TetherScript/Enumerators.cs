namespace Guncon3Console.TetherScript
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public enum DriversConst : ushort
    {
        TTC_VENDORID = 0xF00F,
        TTC_PRODUCTID_JOYSTICK = 0x00000001,
        TTC_PRODUCTID_MOUSEABS = 0x00000002,
        TTC_PRODUCTID_KEYBOARD = 0x00000003,
        TTC_PRODUCTID_GAMEPAD = 0x00000004,
        TTC_PRODUCTID_MOUSEREL = 0x00000005,
    }
}
