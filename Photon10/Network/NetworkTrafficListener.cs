using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Photon10.Services.GameHandler;

namespace Photon10.Network
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public string packetData = string.Empty;
    }

    public delegate void GamePacketReceived(PacketReceivedEventArgs eventArgs);
    public static class NetworkTrafficListener
    {
        public delegate void processPacket(byte[] bytes);
        public static event GamePacketReceived EventReceived;

        private static int _port = 7501;

        private static bool stop = false;
        private static Task listenerThread;
        public static void Start()
        {
            listenerThread = Task.Run(() => StartListening());
        }
        private static void StartListening()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
            //Create socket
            UdpClient listener = new(_port);
            //Bind socket to endpoint
            try
            {

                while(!stop)
                {
                    byte[] bytes = listener.Receive(ref endPoint);
                    if(bytes.Length >0)
                    {
                        var data = Encoding.UTF8.GetString(bytes);
                        var args = new PacketReceivedEventArgs
                        {
                            packetData = data
                        };
                        FirePacketReceived(args);
                    }
                }
                listener.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        
        public static void FirePacketReceived(PacketReceivedEventArgs e)
        {
            if(EventReceived != null)
                EventReceived(e);
        }
        public static void Stop()
        {
            stop = true;
        }
    }
}
