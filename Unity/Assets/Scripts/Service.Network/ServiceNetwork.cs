using System;
using System.Linq;
using System.Net;
using App;
using Lidgren.Network;
using Model.Data;
using Model.Rules;
using UnityEngine;

namespace Service.Network
{
	public class ServiceNetwork
	{
		private readonly IContext _context;
		private NetServer _server;
		private NetClient _client;

		private DateTime _lastPulse;

		public ServiceNetwork(IContext context)
		{
			_context = context;
		}

		public bool ConnectionReady { get; private set; }

		public void Start()
		{
			var repository = _context.Resolve<Repository>();

			var serverConfig = new NetPeerConfiguration(repository.DataNetwork.AppId);
			serverConfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			serverConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			serverConfig.EnableMessageType(NetIncomingMessageType.DebugMessage);
			serverConfig.Port = repository.DataNetwork.PortServer;
			_server = new NetServer(serverConfig);
			_server.Start();

			var clientConfig = new NetPeerConfiguration(repository.DataNetwork.AppId);
			clientConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			clientConfig.EnableMessageType(NetIncomingMessageType.DebugMessage);
			_client = new NetClient(clientConfig);
			_client.Start();

			_lastPulse = DateTime.UtcNow;
		}

		public void Stop()
		{
			_server?.Shutdown("shutdown");
			_client?.Shutdown("shutdown");
		}

		public void Update(Repository repository)
		{
			if(repository.DataNetwork.Selection != -1)
			{
				var desc = repository.DataNetwork.DataConnections[repository.DataNetwork.Selection];
				if(desc.Local)
				{
					repository.GenerateMatch();
					ConnectionReady = true;
					repository.DataNetwork.Selection = -1;
				}
			}
			else
			{
				ConnectionReady = false;
			}

			ServerMessageCallback(repository);
			ClientMessageCallBack(repository);

			if(DateTime.UtcNow - _lastPulse > TimeSpan.FromSeconds(1d))
			{
				_client.DiscoverLocalPeers(repository.DataNetwork.DataConnections.First(_ => _.Local).DataIpEndPoint.Port);
				_lastPulse = DateTime.UtcNow;
			}
		}

		private void ClientMessageCallBack(Repository repository)
		{
			while(_client.ReadMessage(out var incoming))
			{
				// TODO: auto dictionary - should convert to manual with overrides
				switch(incoming.MessageType)
				{
					case NetIncomingMessageType.DiscoveryResponse:
						var appId = incoming.ReadString();
						var name = incoming.ReadString();
						Sync(repository, appId, name, incoming.SenderEndPoint);
						Debug.Log($"Found server at {incoming.SenderEndPoint} app: {appId} name: {name}");
						break;
				}

				_client.Recycle(incoming);
			}
		}

		private void ServerMessageCallback(Repository repository)
		{
			while(_server.ReadMessage(out var incoming))
			{
				// TODO: auto dictionary - should convert to manual with overrides
				switch(incoming.MessageType)
				{
					case NetIncomingMessageType.DiscoveryRequest:
						var response = _server.CreateMessage();
						response.Write(repository.DataNetwork.AppId);
						response.Write(repository.DataMetaLocal.DataNameLocal);
						_server.SendDiscoveryResponse(response, incoming.SenderEndPoint);
						Debug.Log($"Discovery request from {incoming.SenderEndPoint} name: {incoming.ReadString()}");
						break;
				}

				_server.Recycle(incoming);
			}
		}

		private void Sync(Repository repository, string appId, string name, IPEndPoint ep)
		{
			var index = Array.FindIndex(repository.DataNetwork.DataConnections, _ => _.DataNameRemote == name);
			if(index == -1)
			{
				index = repository.DataNetwork.DataConnections.Length;
				Array.Resize(ref repository.DataNetwork.DataConnections, index + 1);
				repository.DataNetwork.DataConnections[index] = new ModelConnection
				{
					Local = false,
					DataIpEndPoint = ep,
					//DataNameRemote = message.ReadString(),
				};
			}
		}
	}
}
