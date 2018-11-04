using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for managing the network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="UdpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        public UdpNetworkListener(MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
            : base (messageReceivedHandler, clientDisconnectedHandler) {  }

        #endregion

        private const int bufSize = 4096;
        private State state = new State();
        private EndPoint _endPointFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        #region public methods implementation

        /// <summary>
        /// Method responsible for start the client network udp listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            try
            {
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_listener.ReceiveBufferSize = maxMessageBuffer;
                //_listener.SendBufferSize = maxMessageBuffer;

                //_listener.Bind(new IPEndPoint(_remoteEndPoint.Address, 0));
                //_listener.Connect(_remoteEndPoint);

                _listener.Connect(IPAddress.Parse(ip), port);
                Receive();

                //_buffer = new byte[maxMessageBuffer];
                //_stream = new NetworkStream(_listener);

                //_stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }


        private void Receive()
        {
            _listener.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref _endPointFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _listener.EndReceiveFrom(ar, ref _endPointFrom);
                _listener.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref _endPointFrom, recv, so);

                _messageReceivedHandler(new KingBuffer(state.buffer));

                //Console.WriteLine("CLIENT RECV: {0}: {1}, {2}", _endPointFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public override void SendMessage(IKingBuffer kingBuffer)
        {
            try
            {
                var data = kingBuffer.ToArray();

                _listener.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int bytes = _listener.EndSend(ar);
                    //Console.WriteLine("CLIENT SEND: {0}, {1}", bytes, kingBuffer.ReadString());
                }, state);

                //_stream.BeginWrite(kingBuffer.ToArray(), 0, kingBuffer.Length(), null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        public void Send(string text)
        {
            
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from received message from connected server. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected server.</param>
        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (_listener.Connected)
                {
                    var endRead = _stream.EndRead(asyncResult);

                    if (endRead != 0)
                    {
                        var numArray = new byte[endRead];
                        Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                        _stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);
                        
                        _messageReceivedHandler(new KingBufferBase(numArray));

                        return;
                    }
                }

                _stream.Close();
                _clientDisconnectedHandler();
            }
            catch (Exception ex)
            {
                _stream.Close();
                _clientDisconnectedHandler();
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
