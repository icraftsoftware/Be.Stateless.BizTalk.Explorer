#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using BizTalkReceivePort = Microsoft.BizTalk.ExplorerOM.ReceivePort;

namespace Be.Stateless.BizTalk.Explorer
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public class ReceivePort
	{
		public ReceivePort(BizTalkReceivePort port)
		{
			BizTalkReceivePort = port ?? throw new ArgumentNullException(nameof(port));
			ReceiveLocations = new(BizTalkReceivePort.ReceiveLocations);
		}

		public ReceiveLocationCollection ReceiveLocations { get; }

		private BizTalkReceivePort BizTalkReceivePort { get; }
	}
}
