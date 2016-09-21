using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Networking.Sockets;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.IO;
using System.Diagnostics;
using Windows.Networking;
using Windows.Storage;
using Windows.Security.Cryptography;
using Windows.UI.Popups;
using System.Runtime.InteropServices.WindowsRuntime;

namespace VisionTest.SocketServices
{

    public interface IBufferSerializable
    {


    }

    public class InstructionBuffer : IBufferSerializable
    {
        public int bufferSize;
        public string bufferData;
        public virtual Byte[] Serialize()
        {
            throw new Exception();
        }
    }

    //원배는 굳이 안봐도 되는 버퍼클래스
    public class LoginDataBuffer : InstructionBuffer
    {
        public bool isSuccess;
        public LoginDataBuffer(string data, bool _isSuccess)
        {
            isSuccess = _isSuccess;
            bufferData = data;
            bufferSize = Encoding.UTF8.GetByteCount(bufferData);
        }



        public override Byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {





                    if (isSuccess == true)
                        bufferData = "LoginData@Success";
                    else
                        bufferData = "LoginData@Failed";
                    writer.Write(Encoding.UTF8.GetBytes(bufferData));

                }
                return m.ToArray();
            }
        }



    }
    //원배는 굳이 안봐도 되는 버퍼클래스
    public class FileBuffer : InstructionBuffer
    {
        public byte[] FileData;
        public string FileName;
        public int FileLen;

        public FileBuffer(Byte[] filedata, string filename)
        {
            FileData = filedata;
            FileName = filename;
            FileLen = FileData.Length;
        }
        public FileBuffer()
        {

        }
        public override Byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    bufferData = "Download@" + FileName;
                    writer.Write(Encoding.UTF8.GetBytes(bufferData));
                    // Debug.WriteLine("Send FileLen:" + FileLen.ToString());


                }
                return m.ToArray();
            }
        }



    }
    public class SocketService
    {
        public delegate void FileReceivedHandlar(FileBuffer _filebuffer);
        public delegate void LoginDataReceivedHandlar(LoginDataBuffer _loginDataBuffer);

        public event FileReceivedHandlar FileReceived;
        public event LoginDataReceivedHandlar LoginDataReceived;


        string ip;
        int port;
        public bool ClientConnected = false;
        StreamSocket client;
        DataWriter writer;

        private StreamSocketListener _listner;
        private StreamSocketListener listner
        {
            set
            {
                _listner = value;
            }
            get
            {
                if (_listner == null)
                    _listner = new StreamSocketListener();
                return _listner;
            }
        }
        public SocketService()
        {
            ip = "localhost";
            port = 9999;
        }

        public SocketService(string _ip, int _port)
        {
            ip = _ip;
            port = _port;
        }

        public async Task ConnectAsync()
        {
            client = new StreamSocket();
            HostName hostName = new HostName(ip);
            
            await client.ConnectAsync(hostName, port.ToString());
            
            // Debug.WriteLine("Connected!!");

        }
        public void ServerClose()
        {
            
            _listner.Dispose();
        }
        public void ClientClose()
        {
            if (client != null)
                client.Dispose();
        }
        public async Task SendAsync(LoginDataBuffer buffer)
        {

            {
                Byte[] sendData = buffer.Serialize();
                writer.WriteInt32(Encoding.UTF8.GetByteCount(buffer.bufferData));
                writer.WriteBytes(sendData);
                try
                {
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    // Debug.WriteLine("Send Complete");
                }
                catch (Exception e)
                {

                     Debug.WriteLine(e.Message);
                }
            }
        }
        public async Task SendAsync(FileBuffer buffer)
        {
            byte[] sendData = buffer.Serialize();


            {
                writer.WriteInt32(Encoding.UTF8.GetByteCount(buffer.bufferData));
                writer.WriteBytes(sendData);
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.WriteInt32(buffer.FileLen);
                await writer.StoreAsync();

                uint total = (uint)buffer.FileLen;

                writer.WriteBytes(buffer.FileData);
                await writer.StoreAsync();










            }

        }

        public async Task StartListener()
        {

            // Debug.WriteLine("Listner : Start Receive");
            
            listner.ConnectionReceived += async (s, e) =>
            {
                writer = new DataWriter(e.Socket.OutputStream);
                client = e.Socket;
                Debug.WriteLine("Listner : ConnectReceived");
                using (DataReader reader = new DataReader(e.Socket.InputStream))
                {
                    try
                    {
                        ClientConnected = true;
                        while (true)
                        {
                            
                            uint sizeFiledCount = await reader.LoadAsync(4);
                            if (sizeFiledCount != 4)
                                break;
                            sizeFiledCount = reader.ReadUInt32();
                            Debug.WriteLine(sizeFiledCount);
                            uint message_len = await reader.LoadAsync(sizeFiledCount);
                            string message = reader.ReadString(message_len);
                            var bufferInfo = message.Split(new char[] { '@' });
                            if (bufferInfo[0] == "LoginData")
                            {
                                LoginDataBuffer buffer;
                                if (bufferInfo[1] == "Success")
                                    buffer = new LoginDataBuffer(message, true);
                                else
                                    buffer = new LoginDataBuffer(message, false);

                                if (LoginDataReceived != null)
                                    LoginDataReceived(buffer);
                            }
                            else if (bufferInfo[0] == "Download")
                            {
                                var buffer = new FileBuffer();
                                buffer.FileName = bufferInfo[1];
                                Debug.WriteLine(buffer.FileName);
                                await reader.LoadAsync(4);
                                buffer.FileLen = reader.ReadInt32();
                                Debug.WriteLine("Read FileLen:" + buffer.FileLen.ToString());

                                StorageFolder folder = ApplicationData.Current.LocalFolder;
                                // Debug.WriteLine(folder.Path);
                                var file = await folder.CreateFileAsync(buffer.FileName, CreationCollisionOption.ReplaceExisting);
                                using (var stream = await file.OpenStreamForWriteAsync())
                                {
                                    int index = 0;
                                    int total = buffer.FileLen;
                                    uint numReadBytes;
                                    do
                                    {


                                        numReadBytes = await reader.LoadAsync(1024);
                                        byte[] b = new byte[numReadBytes];
                                        reader.ReadBytes(b);
                                        stream.Write(b, 0, (int)numReadBytes);

                                    }
                                   
                                    while (numReadBytes > 0);


                                }
                                using( var stream = await file.OpenStreamForReadAsync())
                                {
                                    buffer.FileData = new byte[stream.Length];
                                    stream.Read(buffer.FileData,0,buffer.FileData.Length);
                                }

                                if (FileReceived != null)
                                    FileReceived(buffer);
                                    // Debug.WriteLine("Complete File Received");






                            }
                        }
                    }
                    catch (Exception ex)
                    {
                         Debug.WriteLine(ex.Message);
                        e.Socket.Dispose();
                    }
                }





            };



            await listner.BindEndpointAsync(new HostName(ip), "" + port);
        }


        

    }
}

