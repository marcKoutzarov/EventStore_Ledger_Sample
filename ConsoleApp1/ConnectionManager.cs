using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public class ConnectionManager
    {
        private static readonly String connectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";
        private ConnectionSettings _settings = ConnectionSettings.Create();
        private  IEventStoreConnection _connection = EventStoreConnection.Create(connectionString, "PostingConnection");

        private volatile string _connectionStatus = "Closed";
      
        public ConnectionManager()
        {
           
            _connectionStatus = "Closed";
        }

        public string Status { get => _connectionStatus; }

        public IEventStoreConnection Connection { get => _connection;}

        public void Connect() {

            // check is code is reconnecting or is open
            if (_connectionStatus == "Connected" || _connectionStatus == "Reconnecting")
            {
                return;

            }else{

                _connectionStatus = "Closed";

                if (_connection != null) {
                    _connection.Closed -= ConnectionClosedEvent;
                    _connection.Connected -= ConnectedEvent;
                    _connection.Disconnected -= DisConnectedEvent;
                    _connection.Reconnecting -= ReConnectingEvent;
                    _connection.ErrorOccurred -= ConnectionErrorEvent;
                    _connection.Dispose();
                    _connection = null;
                }
                
                _connection = EventStoreConnection.Create(connectionString, "PostingConnection");

                _connection.Closed += ConnectionClosedEvent;
                _connection.Connected += ConnectedEvent;
                _connection.Disconnected += DisConnectedEvent;
                _connection.Reconnecting += ReConnectingEvent;
                _connection.ErrorOccurred += ConnectionErrorEvent;

               
            }

             try
            {
                
                _connection.ConnectAsync().Wait();
                Thread.Sleep(2000);
            }
            catch(Exception ex)
            {
               _connectionStatus = "Closed";
               System.Diagnostics.Debug.WriteLine("Connection is disposed. Re-init Connection" + ex.Message);
            }
        }

        private void ConnectionClosedEvent(object sender, EventArgs a)
        {
            _connectionStatus = "Closed";
            System.Diagnostics.Debug.WriteLine("Closed");
        }

        private void ConnectedEvent(object sender, EventArgs a)
        {
            _connectionStatus = "Connected";
            System.Diagnostics.Debug.WriteLine("Connected");
        }

        private void DisConnectedEvent(object sender, EventArgs a)
        {
             _connectionStatus = "Disconnected";
            System.Diagnostics.Debug.WriteLine("Disconnected");
        }

        private void ReConnectingEvent(object sender, EventArgs a)
        {
            _connectionStatus = "Reconnecting";
            System.Diagnostics.Debug.WriteLine("Reconnecting");

        }

        private void ConnectionErrorEvent(object sender, EventArgs a)
        {
            _connectionStatus = "Error";
            System.Diagnostics.Debug.WriteLine("Error");
        }

    }
}
