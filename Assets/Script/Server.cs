using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _FromClient, Packet _Packet);
    public static Dictionary<int, PacketHandler> PacketHandlers;

    private static TcpListener TcpListener;
    private static UdpClient UdpListener;

    public static void Start(int _MaxPlayers, int _Port)
    {
        MaxPlayers = _MaxPlayers;
        Port = _Port;

        Debug.Log("Starting Server...");
        InitializeServerData();

        TcpListener = new TcpListener(IPAddress.Any, Port);
        TcpListener.Start();
        TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        UdpListener = new UdpClient(Port);
        UdpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server Started on {Port}");
    }

    private static void TCPConnectCallback(IAsyncResult _Result)
    {
        TcpClient _Client = TcpListener.EndAcceptTcpClient(_Result);
        TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming Connection from {_Client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (Clients[i].MyTcp.Socket == null)
            {
                Clients[i].MyTcp.Connect(_Client);
                return;
            }
        }

        Debug.Log($"{_Client.Client.RemoteEndPoint} Failed To Connect: Server Full");
    }

    private static void UDPReceiveCallback(IAsyncResult _Result)
    {
        try
        {
            IPEndPoint _ClientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _Data = UdpListener.EndReceive(_Result, ref _ClientEndPoint);
            UdpListener.BeginReceive(UDPReceiveCallback, null);

            if (_Data.Length < 4)
            {
                return;
            }

            using (Packet _Packet = new Packet(_Data))
            {
                int _ClientId = _Packet.ReadInt();

                if (_ClientId == 0)
                {
                    return;
                }

                if (Clients[_ClientId].MyUdp.EndPoint == null)
                {
                    Clients[_ClientId].MyUdp.Connect(_ClientEndPoint);
                    return;
                }

                if (Clients[_ClientId].MyUdp.EndPoint.ToString() == _ClientEndPoint.ToString())
                {
                    Clients[_ClientId].MyUdp.HandleData(_Packet);
                }
            }
        }
        catch (Exception _Ex)
        {
            Debug.Log($"Error Receiveing Udp Data: {_Ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _Packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                UdpListener.BeginSend(_Packet.ToArray(), _Packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _Ex)
        {
            Debug.Log($"Error Sending Data To {_clientEndPoint} Via Udp: {_Ex}");
        }
    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            Clients.Add(i, new Client(i));
        }

        PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement},
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot},
                { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem},
            };
        Debug.Log("Initialized Packets");
    }

    public static void Stop()
    {
        TcpListener.Stop();
        UdpListener.Close();
    }
}
