# GunconUSB
Use Guncon 2 and 3 on Windows

#### This is a work in progress project. Not ready for use.
Please contact me if you are interested in helping to develop a driver for use the guncons on windows.

Windows requires a driver to let apps access usb devices.
I use WinUsb for this. It's possible to have a signed driver this way.

App requires TetherScript's [HID Virtual Driver Kit](https://tetherscript.com/hid-driver-kit-home/) to create a virtual Joystick and Mouse.

This project would not be possible without the reverse engineering and documentation of the guncon 2 and guncon 3 by beardypig:

* [Linux kernel driver for the Guncon 2](https://github.com/beardypig/guncon2/)
* [Linux kernel driver for the Guncon 3](https://github.com/beardypig/guncon3)

I also used some code from pcnimdock's [fork](https://github.com/pcnimdock/guncon3)

[WinUSBNet](https://github.com/madwizard-thomas/winusbnet) is used to accecss the WinUSB Guncon driver


You will need a way to output 15khz to a CRT and have composite sync to connect the Guncon 2.