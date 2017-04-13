﻿using System;
using System.Threading;
using SimpleNet.Client;
using SimpleNet.Common;
using SimpleNet.Helpers;
using SimpleNet.Server;

namespace SimpleNet.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new SimpleNetServerBuilder().UseConsoleLogger()
                                                     .UseIpAddresses(new[] {"127.0.0.1"})
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageDispatchPacket,
                                                         ChatMessageDispatchPacketHandler>()
                                                     .RegisterPacketHandler<ServerInformationRequestPacket,
                                                         ServerInformationRequestPacketHandler>()
                                                     .Build<ExampleServer>()
                                                     .Start();

            var client = new SimpleNetClientBuilder().UseConsoleLogger()
                                                     .UseIp("127.0.0.1")
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageReceivedPacket,
                                                         ChatMessageReceivedPacketHandler>()
                                                     .RegisterPacketHandler<ServerInformationResponsePacket,
                                                         ServerInformationResponsePacketHandler>()
                                                     .Build<ExampleClient>()
                                                     .Connect();

            client.Send(new ChatMessageDispatchPacket
                        {
                            Message = "I am the message",
                            Sender = "The Sender",
                            UniqueKey = "ChatMessageDispatchPacket"
                        });

            client.Send(new ChatMessageDispatchPacket
                        {
                            Message = "I am a UDP message",
                            Sender = "The Sender (UDP)",
                            UniqueKey = "ChatMessageDispatchPacket"
                        },
                SimpleNetProtocol.Udp);

            new Thread(() =>
                       {
                           while(true)
                           {
                               server.Broadcast(new ServerInformationResponsePacket
                                                {
                                                    MachineName =
                                                        Environment
                                                            .MachineName,
                                                    UniqueKey =
                                                        "ServerInformationResponsePacket"
                                                });

                               Thread.Sleep(5000);
                           }
                       }).Start();

            /*client.CreatePacket(new ServerInformationRequestPacket())
                .HandleResponse<ServerInformationResponsePacket>(
                    packet => Console.WriteLine($"I am sync. {packet.MachineName}")).Send();

            client.CreatePacket(new ServerInformationRequestPacket())
                .HandleResponseAsync<ServerInformationResponsePacket>(
                    packet => Console.WriteLine($"I am async. {packet.MachineName}")).Send();*/
        }
    }
}