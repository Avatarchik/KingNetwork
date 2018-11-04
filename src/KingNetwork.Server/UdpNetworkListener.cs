using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the udp network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region private members
        
        /// <summary>
		/// The endpoint value to received data.
		/// </summary>
        private EndPoint _endPointFrom;

        #endregion

        #region constructors

        private const int bufSize = 4096;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        private State state = new State();

        private AsyncCallback recv = null;

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
            try
            {
                _clientConnectedHandler = clientConnectedHandler;
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _endPointFrom = new IPEndPoint(IPAddress.Any, 0);

                //_listener.BeginAccept(new AsyncCallback(OnAccept), null);

                _listener.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
                _listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                Receive();

                //_listener.BeginAccept(new AsyncCallback(OnAccept), null);
                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        private void Receive()
        {
            _listener.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref _endPointFrom, new AsyncCallback(OnAccept), null);

            //_listener.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref _endPointFrom, recv = (ar) =>
            //{
            //    State so = (State)ar.AsyncState;
            //    int bytes = _listener.EndReceiveFrom(ar, ref _endPointFrom);
            //    _listener.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref _endPointFrom, recv, so);
            //    //Console.WriteLine("SERVER RECV: {0}: {1}, {2}", _endPointFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            //}, state);
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                _clientConnectedHandler(_listener);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
            //finally
            //{
            //    _listener.BeginAccept(new AsyncCallback(OnAccept), null);
            //}
        }

        #endregion
    }
}
