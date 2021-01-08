#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using BizTalkReceiveLocation = Microsoft.BizTalk.ExplorerOM.ReceiveLocation;

namespace Be.Stateless.BizTalk.Explorer
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public class ReceiveLocation
	{
		public ReceiveLocation(BizTalkReceiveLocation location)
		{
			BizTalkReceiveLocation = location ?? throw new ArgumentNullException(nameof(location));
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public bool Enabled
		{
			get => BizTalkReceiveLocation.Enable;
			set => BizTalkReceiveLocation.Enable = value;
		}

		public string Name => BizTalkReceiveLocation.Name;

		[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Public API.")]
		private BizTalkReceiveLocation BizTalkReceiveLocation { get; set; }

		public void Disable()
		{
			if (Enabled) Enabled = false;
		}

		public void Enable()
		{
			if (!Enabled) Enabled = true;
		}
	}
}
