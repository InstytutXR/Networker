﻿using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;

namespace Networker.Example.Json.Middleware
{
	public class RoleCheckMiddleware : IMiddlewareHandler
	{
		private readonly ILogger<RoleCheckMiddleware> logger;

		public RoleCheckMiddleware(ILogger<RoleCheckMiddleware> logger)
		{
			this.logger = logger;
		}

		public async Task<bool> Process(IPacketContext context)
		{
			var roleAttribute = context.Handler.GetType()
			                           .GetCustomAttribute<RoleRequired>();

			if(roleAttribute == null)
				return true;

			if(roleAttribute.RoleName == "Admin" && this.IsAdmin(context.Sender))
			{
				return true;
			}

			this.logger.LogCritical("Somebody tried to do something they did not have permission for!");
			context.Sender.Send(new NotAllowedResponsePacket());

			return false;

		}

		private bool IsAdmin(ISender contextSender)
		{
			//Add some BETTER custom logic here
			return contextSender.EndPoint.Address == IPAddress.Parse("127.0.0.1");
		}
	}
}