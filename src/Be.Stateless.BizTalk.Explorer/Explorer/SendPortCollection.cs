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
using BizTalkSendPortCollection = Microsoft.BizTalk.ExplorerOM.SendPortCollection;

namespace Be.Stateless.BizTalk.Explorer
{
	public class SendPortCollection
	{
		public SendPortCollection(BizTalkSendPortCollection ports)
		{
			BizTalkSendPortCollection = ports ?? throw new ArgumentNullException(nameof(ports));
		}

		public SendPort this[string name]
		{
			get
			{
				var explorerSendPort = BizTalkSendPortCollection[name]
					?? throw new ArgumentException(
						$"BizTalk Send Port '{name}' cannot be found in BizTalk Server Group [{BizTalkServerGroup.ManagementDatabase}].",
						nameof(name));
				return new(explorerSendPort);
			}
		}

		[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Public API.")]
		private BizTalkSendPortCollection BizTalkSendPortCollection { get; set; }
	}
}
