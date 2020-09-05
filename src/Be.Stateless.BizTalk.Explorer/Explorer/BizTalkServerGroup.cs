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
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Explorer
{
	/// <summary>
	/// Access point to a BizTalk Server Group which defaults to the local BizTalk Server Group unless specified otherwise.
	/// </summary>
	public static class BizTalkServerGroup
	{
		[SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations")]
		static BizTalkServerGroup()
		{
			const string path = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration";
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			using (var subKey = baseKey.OpenSubKey(path) ?? throw new InvalidOperationException($"Cannot find registry key '{baseKey.Name}\\{path}'"))
			{
				ManagementDatabase = new BizTalkServerManagementDatabase((string) subKey.GetValue("MgmtDBServer"), (string) subKey.GetValue("MgmtDBName"));
				Applications = new ApplicationCollection();
			}
		}

		/// <summary>
		/// The collection of applications installed on the local BizTalk Server Group.
		/// </summary>
		public static ApplicationCollection Applications { get; }

		public static BizTalkServerManagementDatabase ManagementDatabase { get; }
	}
}
