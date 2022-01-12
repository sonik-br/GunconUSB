/*  WinUSBNet library
 *  (C) 2010 Thomas Bleeker (www.madwizard.org)
 *
 *  Licensed under the MIT license, see license.txt or:
 *  http://www.opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]

namespace MadWizard.WinUSBNet
{
    /// <summary>
    /// The UsbDevice class represents a single WinUSB device.
    /// </summary>
    public class USBDevice : IDisposable
    {
        private API.WinUSBDevice _wuDevice = null;
        private bool _disposed = false;

        /// <summary>
        /// Collection of all pipes available on the USB device
        /// </summary>
        public USBPipeCollection Pipes
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of all interfaces available on the USB device
        /// </summary>
        public USBInterfaceCollection Interfaces
        {
            get;
            private set;
        }

        /// <summary>
        /// Device descriptor with information about the device
        /// </summary>
        public USBDeviceDescriptor Descriptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a new USB device
        /// </summary>
        /// <param name="deviceInfo">USB device info of the device to create</param>
        public USBDevice(USBDeviceInfo deviceInfo)
            : this(deviceInfo.DevicePath)
        {
            // Handled in other constructor
        }

        /// <summary>
        /// Disposes the UsbDevice including all unmanaged WinUSB handles. This function
        /// should be called when the UsbDevice object is no longer in use, otherwise
        /// unmanaged handles will remain open until the garbage collector finalizes the
        /// object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer for the UsbDevice. Disposes all unmanaged handles.
        /// </summary>
        ~USBDevice()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing">Indicates whether Dispose was called manually (true) or by
        /// the garbage collector (false) via the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_wuDevice != null)
                    _wuDevice.Dispose();
            }

            // Clean unmanaged resources here.
            // (none currently)

            _disposed = true;
        }

        /// <summary>
        /// Constructs a new USB device
        /// </summary>
        /// <param name="devicePathName">Device path name of the USB device to create</param>
        public USBDevice(string devicePathName)
        {
            Descriptor = GetDeviceDescriptor(devicePathName);
            _wuDevice = new API.WinUSBDevice();
            try
            {
                _wuDevice.OpenDevice(devicePathName);
                InitializeInterfaces();
            }
            catch (API.APIException e)
            {
                _wuDevice.Dispose();
                throw new USBException("Failed to open device.", e);
            }
        }

        internal API.WinUSBDevice InternalDevice
        {
            get
            {
                return _wuDevice;
            }
        }

        private void InitializeInterfaces()
        {
            int numInterfaces = _wuDevice.InterfaceCount;

            List<USBPipe> allPipes = new List<USBPipe>();

            USBInterface[] interfaces = new USBInterface[numInterfaces];
            // UsbEndpoint
            for (int i = 0; i < numInterfaces; i++)
            {
                API.USB_INTERFACE_DESCRIPTOR descriptor;
                API.WINUSB_PIPE_INFORMATION[] pipesInfo;
                _wuDevice.GetInterfaceInfo(i, out descriptor, out pipesInfo);
                USBPipe[] interfacePipes = new USBPipe[pipesInfo.Length];
                for(int k=0;k<pipesInfo.Length;k++)
                {
                    USBPipe pipe = new USBPipe(this, pipesInfo[k]);
                    interfacePipes[k] = pipe;
                    allPipes.Add(pipe);
                }
                // TODO:
                //if (descriptor.iInterface != 0)
                //    _wuDevice.GetStringDescriptor(descriptor.iInterface);
                USBPipeCollection pipeCollection = new USBPipeCollection(interfacePipes);
                interfaces[i] = new USBInterface(this, i, descriptor, pipeCollection);
            }
            Pipes = new USBPipeCollection(allPipes.ToArray());
            Interfaces = new USBInterfaceCollection(interfaces);
        }

        private void CheckControlParams(int value, int index, byte[] buffer, int length)
        {
            if (value < ushort.MinValue || value > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "Value parameter out of range.");
            if (index < ushort.MinValue || index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index), "Index parameter out of range.");
            if (length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length parameter is larger than the size of the buffer.");
            if (length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(length), "Length too large");
        }

        /// <summary>
        /// Specifies the timeout in milliseconds for control pipe operations. If a control transfer does not finish within the specified time it will fail.
        /// When set to zero, no timeout is used. Default value is 5000 milliseconds.
        /// </summary>
        /// <seealso href="http://msdn.microsoft.com/en-us/library/aa476439.aspx">WinUSB_GetPipePolicy for a more detailed description</seealso>
        public int ControlPipeTimeout
        {
            get
            {
                return (int)_wuDevice.GetPipePolicyUInt(0, 0x00, API.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT);
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Control pipe timeout cannot be negative.");
                _wuDevice.SetPipePolicy(0, 0x00, API.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT, (uint)value);
            }
        }


        /// <summary>
        /// Initiates a control transfer over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter. Alternatively, <see cref="ControlIn(byte,byte,int,int,byte[],int)"/> and
        /// <see cref="ControlOut(byte,byte,int,int,byte[],int)"/> can be used for control transfers in a specific direction, which is the recommended way because
        /// it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known at compile time.
        /// </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received will be
        /// written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe.</param>
        /// <param name="length">Length of the data to transfer. Must be equal to or less than the length of <paramref name="buffer"/>.
        /// The setup packet's length member will be set to this length.</param>
        /// <returns>The number of bytes received from the device.</returns>
        public int ControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, int length)
        {
            // Parameters are int and not ushort because ushort is not CLS compliant.
            CheckNotDisposed();
            CheckControlParams(value, index, buffer, length);

            try
            {
                return _wuDevice.ControlTransfer(requestType, request, (ushort)value, (ushort)index, (ushort)length, buffer);
            }
            catch (API.APIException e)
            {
                throw new USBException("Control transfer failed", e);
            }
        }

        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter. Alternatively, <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> and
        /// <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> can be used for asynchronous control transfers in a specific direction, which is
        /// the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method when the direction is not
        /// known at compile time. </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received will be
        /// written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe. Note: This buffer is not allowed
        /// to change for the duration of the asynchronous operation.</param>
        /// <param name="length">Length of the data to transfer. Must be equal to or less than the length of <paramref name="buffer"/>. The setup packet's length member will be set to this length.</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
        {
            // Parameters are int and not ushort because ushort is not CLS compliant.
            CheckNotDisposed();
            CheckControlParams(value, index, buffer, length);

            USBAsyncResult result = new USBAsyncResult(userCallback, stateObject);

            try
            {
                _wuDevice.ControlTransferOverlapped(requestType, request, (ushort)value, (ushort)index, (ushort)length, buffer, result);
            }
            catch (API.APIException e)
            {
                result.Dispose();
                throw new USBException("Asynchronous control transfer failed", e);
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }
            return result;
        }

        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter. Alternatively, <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> and
        /// <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> can be used for asynchronous control transfers in a specific direction, which is
        /// the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method when the direction is not
        /// known at compile time. </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received will be
        /// written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe. The setup packet's length member will
        /// be set to the length of this buffer. Note: This buffer is not allowed to change for the duration of the asynchronous operation. </param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
        {
            return BeginControlTransfer(requestType, request, value, index, buffer, buffer.Length, userCallback, stateObject);
        }


        /// <summary>
        /// Waits for a pending asynchronous control transfer to complete.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> object representing the asynchronous operation,
        /// as returned by one of the ControlIn, ControlOut or ControlTransfer methods.</param>
        /// <returns>The number of bytes transfered during the operation.</returns>
        /// <remarks>Every asynchronous control transfer must have a matching call to <see cref="EndControlTransfer"/> to dispose
        /// of any resources used and to retrieve the result of the operation. When the operation was successful the method returns the number
        /// of bytes that were transfered. If an error occurred during the operation this method will throw the exceptions that would
        /// otherwise have occurred during the operation. If the operation is not yet finished EndControlTransfer will wait for the
        /// operation to finish before returning.</remarks>
        public int EndControlTransfer(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new NullReferenceException("asyncResult cannot be null");
            if (!(asyncResult is USBAsyncResult))
                throw new ArgumentException("AsyncResult object was not created by calling one of the BeginControl* methods on this class.");

            // todo: check duplicate end control
            USBAsyncResult result = (USBAsyncResult)asyncResult;
            try
            {
                if (!result.IsCompleted)
                    result.AsyncWaitHandle.WaitOne();

                if (result.Error != null)
                    throw new USBException("Asynchronous control transfer from pipe has failed.", result.Error);

                return result.BytesTransfered;
            }
            finally
            {
                result.Dispose();
            }

        }


        /// <summary>
        /// Initiates a control transfer over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter). Alternatively, <see cref="ControlIn(byte,byte,int,int,byte[])"/> and
        /// <see cref="ControlOut(byte,byte,int,int,byte[])"/> can be used for control transfers in a specific direction, which is the recommended way because
        /// it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known at compile time.
        /// </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The data to transfer in the data stage of the control. When the transfer is in the IN direction the data received will be
        /// written to this buffer. For an OUT direction transfer the contents of the buffer are written sent through the pipe. The length of this
        /// buffer is used as the number of bytes in the control transfer. The setup packet's length member will be set to this length as well.</param>
        /// <returns>The number of bytes received from the device.</returns>
        public int ControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer)
        {
            return ControlTransfer(requestType, request, value, index, buffer, buffer.Length);
        }

        /// <summary>
        /// Initiates a control transfer without a data stage over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter). Alternatively, <see cref="ControlIn(byte,byte,int,int)"/> and
        /// <see cref="ControlOut(byte,byte,int,int)"/> can be used for control transfers in a specific direction, which is the recommended way because
        /// it prevents using the wrong direction accidentally. Use the ControlTransfer method when the direction is not known at compile time.
        /// </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        public void ControlTransfer(byte requestType, byte request, int value, int index)
        {
            // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
            ControlTransfer(requestType, request, value, index, new byte[0], 0);
        }

        private void CheckIn(byte requestType)
        {
            if ((requestType & 0x80) == 0) // Host to device?
                throw new ArgumentException("Request type is not IN.");
        }

        private void CheckOut(byte requestType)
        {
            if ((requestType & 0x80) == 0x80) // Device to host?
                throw new ArgumentException("Request type is not OUT.");
        }

        /// <summary>
        /// Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter). A buffer to receive the data is automatically created by this method.
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="length">Length of the data to transfer. A buffer will be created with this length and the length member of the setup packet
        /// will be set to this length.</param>
        /// <returns>A buffer containing the data transfered.</returns>
        /// <remarks>This routine initially allocates a buffer to hold the <paramref name="length"/> bytes of data expected from the device.
        /// If the device responds with less data than expected, this routine will allocate a smaller buffer to copy and return only the bytes actually received.
        /// </remarks>
        public byte[] ControlIn(byte requestType, byte request, int value, int index, int length)
        {
            CheckIn(requestType);
            byte[] buffer = new byte[length];
            int actuallyReceived = ControlTransfer(requestType, request, value, index, buffer, buffer.Length);
            if (actuallyReceived < length)
            {
                byte[] outBuffer = new byte[actuallyReceived];
                Array.Copy(buffer, 0, outBuffer, 0, actuallyReceived);
                return outBuffer;
            }
            return buffer;
        }


        /// <summary>
        /// Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that will receive the data transfered.</param>
        /// <param name="length">Length of the data to transfer. The length member of the setup packet will be set to this length. The buffer specified
        /// by the <paramref name="buffer"/> parameter should have at least this length.</param>
        /// <returns>The number of bytes received from the device.</returns>
        public int ControlIn(byte requestType, byte request, int value, int index, byte[] buffer, int length)
        {
            CheckIn(requestType);
            return ControlTransfer(requestType, request, value, index, buffer, length);
        }

        /// <summary>
        /// Initiates a control transfer over the default control endpoint. The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter). The length of buffer given by the <paramref name="buffer"/> parameter will dictate
        /// the number of bytes that are transfered and the value of the setup packet's length member.
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that will receive the data transfered. The length of this buffer will be the number of bytes transfered.</param>
        /// <returns>The number of bytes received from the device.</returns>
        public int ControlIn(byte requestType, byte request, int value, int index, byte[] buffer)
        {
            CheckIn(requestType);
            return ControlTransfer(requestType, request, value, index, buffer);
        }

        /// <summary>
        /// Initiates a control transfer without a data stage over the default control endpoint. The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter). The setup packets' length member will be set to zero.
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        public void ControlIn(byte requestType, byte request, int value, int index)
        {
            CheckIn(requestType);
            // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
            ControlTransfer(requestType, request, value, index, new byte[0]);
        }

        /// <summary>
        /// Initiates a control transfer over the default control endpoint. The request should have an OUT direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">A buffer containing the data to transfer in the data stage.</param>
        /// <param name="length">Length of the data to transfer. Only the first <paramref name="length"/> bytes of <paramref name="buffer"/> will be transfered.
        /// The setup packet's length parameter is set to this length.</param>
        public void ControlOut(byte requestType, byte request, int value, int index, byte[] buffer, int length)
        {
            CheckOut(requestType);
            ControlTransfer(requestType, request, value, index, buffer, length);
        }

        /// <summary>
        /// Initiates a control transfer over the default control endpoint. The request should have an OUT direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">A buffer containing the data to transfer in the data stage. The complete buffer is transfered. The setup packet's length
        /// parameter is set to the length of this buffer.</param>
        public void ControlOut(byte requestType, byte request, int value, int index, byte[] buffer)
        {
            CheckOut(requestType);
            ControlTransfer(requestType, request, value, index, buffer);
        }

        /// <summary>
        /// Initiates a control transfer without a data stage over the default control endpoint. The request should have an OUT direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter. The setup packets' length member will be set to zero.
        /// </summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        public void ControlOut(byte requestType, byte request, int value, int index)
        {
            CheckOut(requestType);
            // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
            ControlTransfer(requestType, request, value, index, new byte[0]);
        }



        /// <summary>
        /// Initiates an asynchronous control transfer without a data stage over the default control endpoint. This method allows both IN and OUT direction transfers, depending
        /// on the highest bit of the <paramref name="requestType"/> parameter. Alternatively, <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> and
        /// <see cref="BeginControlIn(byte,byte,int,int,byte[],int,AsyncCallback,object)"/> can be used for asynchronous control transfers in a specific direction, which is
        /// the recommended way because it prevents using the wrong direction accidentally. Use the BeginControlTransfer method when the direction is not
        /// known at compile time. </summary>
        /// <param name="requestType">The setup packet request type.</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
        {
            // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
            return BeginControlTransfer(requestType, request, value, index, new byte[0], 0, userCallback, stateObject);
        }



        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint.  The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that will receive the data transfered.</param>
        /// <param name="length">Length of the data to transfer. Must be equal to or less than the length of <paramref name="buffer"/>. The setup packet's length member will be set to this length.</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
        {
            CheckIn(requestType);
            return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
        }

        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint. The request should have an IN direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that will receive the data transfered. The setup packet's length member will be set to the length of this buffer.</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
        {
            CheckIn(requestType);
            return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
        }

        /// <summary>
        /// Initiates an asynchronous control transfer without a data stage over the default control endpoint.
        /// The request should have an IN direction (specified by the highest bit of the <paramref name="requestType"/> parameter).
        /// The setup packets' length member will be set to zero.</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the IN direction (highest bit set).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
        {
            CheckIn(requestType);
            return BeginControlTransfer(requestType, request, value, index, userCallback, stateObject);
        }

        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint.  The request should have an OUT direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that contains the data to be transfered.</param>
        /// <param name="length">Length of the data to transfer. Must be equal to or less than the length of <paramref name="buffer"/>. The setup packet's length member will be set to this length.</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
        {
            CheckOut(requestType);
            return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
        }

        /// <summary>
        /// Initiates an asynchronous control transfer over the default control endpoint. The request should have an OUT direction (specified by the highest bit
        /// of the <paramref name="requestType"/> parameter).</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="buffer">The buffer that contains the data to be transfered. The setup packet's length member will be set to the length of this buffer.</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
        {
            CheckOut(requestType);
            return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
        }

        /// <summary>
        /// Initiates an asynchronous control transfer without a data stage over the default control endpoint.
        /// The request should have an OUT direction (specified by the highest bit of the <paramref name="requestType"/> parameter).
        /// The setup packets' length member will be set to zero.</summary>
        /// <param name="requestType">The setup packet request type. The request type must specify the OUT direction (highest bit cleared).</param>
        /// <param name="request">The setup packet device request.</param>
        /// <param name="value">The value member in the setup packet. Its meaning depends on the request. Value should be between zero and 65535 (0xFFFF).</param>
        /// <param name="index">The index member in the setup packet. Its meaning depends on the request. Index should be between zero and 65535 (0xFFFF).</param>
        /// <param name="userCallback">An optional asynchronous callback, to be called when the control transfer is complete. Can be null if no callback is required.</param>
        /// <param name="stateObject">A user-provided object that distinguishes this particular asynchronous operation. Can be null if not required.</param>
        /// <returns>An <see cref="IAsyncResult"/> object representing the asynchronous control transfer, which could still be pending.</returns>
        /// <remarks>This method always completes immediately even if the operation is still pending. The <see cref="IAsyncResult"/> object returned represents the operation
        /// and must be passed to <see cref="EndControlTransfer"/> to retrieve the result of the operation. For every call to this method a matching call to
        /// <see cref="EndControlTransfer"/> must be made. When <paramref name="userCallback"/> specifies a callback function, this function will be called when the operation is completed. The optional
        /// <paramref name="stateObject"/> parameter can be used to pass user-defined information to this callback or the <see cref="IAsyncResult"/>. The <see cref="IAsyncResult"/>
        /// also provides an event handle (<see cref="IAsyncResult.AsyncWaitHandle" />) that will be triggered when the operation is complete as well.
        /// </remarks>
        public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
        {
            CheckOut(requestType);
            // TODO: null instead of empty buffer. But overlapped code would have to be fixed for this (no buffer to pin)
            return BeginControlTransfer(requestType, request, value, index, new byte[0], userCallback, stateObject);
        }


        private void CheckNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("USB device object has been disposed.");
        }

        /// <summary>
        /// Finds WinUSB devices with a GUID matching the parameter guidString
        /// </summary>
        /// <param name="guidString">The GUID string that the device should match.
        /// The format of this string may be any format accepted by the constructor
        /// of the System.Guid class</param>
        /// <returns>An array of USBDeviceInfo objects representing the
        /// devices found. When no devices are found an empty array is
        /// returned.</returns>
        public static USBDeviceInfo[] GetDevices(string guidString)
        {
            return GetDevices(new Guid(guidString));
        }

        /// <summary>
        /// Finds WinUSB devices with a GUID matching the parameter guid
        /// </summary>
        /// <param name="guid">The GUID that the device should match.</param>
        /// <returns>An array of USBDeviceInfo objects representing the
        /// devices found. When no devices are found an empty array is
        /// returned.</returns>
        public static USBDeviceInfo[] GetDevices(Guid guid)
        {
            API.DeviceDetails[] detailList = API.DeviceManagement.FindDevicesFromGuid(guid);

            USBDeviceInfo[] devices = new USBDeviceInfo[detailList.Length];

            for (int i = 0; i < detailList.Length; i++)
            {
                devices[i] = new USBDeviceInfo(detailList[i]);
            }
            return devices;
        }

        /// <summary>
        /// Finds the first WinUSB device with a GUID matching the parameter guid.
        /// If multiple WinUSB devices match the GUID only the first one is returned.
        /// </summary>
        /// <param name="guid">The GUID that the device should match.</param>
        /// <returns>An UsbDevice object representing the device if found. If
        /// no device with the given GUID could be found null is returned.</returns>
        public static USBDevice GetSingleDevice(Guid guid)
        {
            API.DeviceDetails[] detailList = API.DeviceManagement.FindDevicesFromGuid(guid);
            if (detailList.Length == 0)
                return null;


            return new USBDevice(detailList[0].DevicePath);
        }

        /// <summary>
        /// Finds the first WinUSB device with a GUID matching the parameter guidString.
        /// If multiple WinUSB devices match the GUID only the first one is returned.
        /// </summary>
        /// <param name="guidString">The GUID string that the device should match.</param>
        /// <returns>An UsbDevice object representing the device if found. If
        /// no device with the given GUID could be found null is returned.</returns>
        public static USBDevice GetSingleDevice(string guidString)
        {

            return USBDevice.GetSingleDevice(new Guid(guidString));
        }

        private static USBDeviceDescriptor GetDeviceDescriptor(string devicePath)
        {
            try
            {
                USBDeviceDescriptor descriptor;
                using (API.WinUSBDevice wuDevice = new API.WinUSBDevice())
                {
                    wuDevice.OpenDevice(devicePath);
                    API.USB_DEVICE_DESCRIPTOR deviceDesc = wuDevice.GetDeviceDescriptor();

                    // Get first supported language ID
                    ushort[] langIDs;

                    try
                    {
                        langIDs = wuDevice.GetSupportedLanguageIDs();
                    }
                    catch (Exception)
                    {
                        //fix for original guncon2
                        return new USBDeviceDescriptor(devicePath, deviceDesc, null, null, null);
                    }
                    
                    
                    ushort langID = 0;
                    if (langIDs.Length > 0)
                        langID = langIDs[0];

                    string manufacturer = null, product = null, serialNumber = null;
                    byte idx = 0;
                    idx = deviceDesc.iManufacturer;
                    if (idx > 0)
                        manufacturer = wuDevice.GetStringDescriptor(idx, langID);

                    idx = deviceDesc.iProduct;
                    if (idx > 0)
                        product = wuDevice.GetStringDescriptor(idx, langID);

                    idx = deviceDesc.iSerialNumber;
                    if (idx > 0)
                        serialNumber = wuDevice.GetStringDescriptor(idx, langID);
                    descriptor = new USBDeviceDescriptor(devicePath, deviceDesc, manufacturer, product, serialNumber);
                }
                return descriptor;

            }
            catch (API.APIException e)
            {
                throw new USBException("Failed to retrieve device descriptor.", e);
            }
        }

    }
}
