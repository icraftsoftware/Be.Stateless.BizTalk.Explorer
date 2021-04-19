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

using Be.Stateless.BizTalk.Management;

namespace Be.Stateless.BizTalk.Explorer
{
	/// <summary>
	/// Access point to a BizTalk Server Group which defaults to the local BizTalk Server Group unless specified otherwise.
	/// </summary>
	public static class BizTalkServerGroup
	{
		static BizTalkServerGroup()
		{
			Applications = new ApplicationCollection();
			if (BizTalkInstallation.IsConfigured)
				ManagementDatabase = new BizTalkServerManagementDatabase(BizTalkInstallation.ManagementDatabaseServer, BizTalkInstallation.ManagementDatabaseName);
		}

		/// <summary>
		/// The collection of applications installed on the local BizTalk Server Group.
		/// </summary>
		public static ApplicationCollection Applications { get; }

		public static bool IsConfigured => BizTalkInstallation.IsConfigured;

		public static BizTalkServerManagementDatabase ManagementDatabase { get; }
	}
}
