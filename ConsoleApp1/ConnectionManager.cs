//-----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
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
        private ConnectionSettings settings = ConnectionSettings.Create();
        private  IEventStoreConnection connection = EventStoreConnection.Create(connectionString, "PostingConnection");
        private volatile string connectionStatus = "Closed";
      

        public ConnectionManager()
        {         
            connectionStatus = "Closed";
        }
        /// <summary>
        /// Gets the status of the connection
        /// </summary>
        public string Status { get => connectionStatus; }

        public IEventStoreConnection Connection { get => connection; }

        public void Connect() {
            // check is code is reconnecting or is open
            if (connectionStatus == "Connected" || connectionStatus == "Reconnecting")
            {
                return;
            }
            else{
                connectionStatus = "Closed";

                if (connection != null) {
                    connection.Closed -= ConnectionClosedEvent;
                    connection.Connected -= ConnectedEvent;
                    connection.Disconnected -= DisConnectedEvent;
                    connection.Reconnecting -= ReConnectingEvent;
                    connection.ErrorOccurred -= ConnectionErrorEvent;
                    connection.Dispose();
                    connection = null;
                }
                
                connection = EventStoreConnection.Create(connectionString);
                // Register events.
                connection.Closed += ConnectionClosedEvent;
                connection.Connected += ConnectedEvent;
                connection.Disconnected += DisConnectedEvent;
                connection.Reconnecting += ReConnectingEvent;
                connection.ErrorOccurred += ConnectionErrorEvent;

               
            }

             try
            {
                connection.ConnectAsync().Wait();
                Thread.Sleep(2000);
            }
            catch(Exception ex)
            {
               connectionStatus = "Closed";
               System.Diagnostics.Debug.WriteLine("Connection is disposed. Re-init Connection" + ex.Message);
            }
        }

        private void ConnectionClosedEvent(object sender, EventArgs a)
        {
            connectionStatus = "Closed";
            System.Diagnostics.Debug.WriteLine("Closed");
        }

        private void ConnectedEvent(object sender, EventArgs a)
        {
            connectionStatus = "Connected";
            System.Diagnostics.Debug.WriteLine("Connected");
        }

        private void DisConnectedEvent(object sender, EventArgs a)
        {
             connectionStatus = "Disconnected";
            System.Diagnostics.Debug.WriteLine("Disconnected");
        }

        private void ReConnectingEvent(object sender, EventArgs a)
        {
            connectionStatus = "Reconnecting";
            System.Diagnostics.Debug.WriteLine("Reconnecting");

        }

        private void ConnectionErrorEvent(object sender, EventArgs a)
        {
            connectionStatus = "Error";
            System.Diagnostics.Debug.WriteLine("Error");
        }
    }
}
