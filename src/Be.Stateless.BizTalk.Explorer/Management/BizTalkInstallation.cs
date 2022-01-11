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
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Management
{
	public static class BizTalkInstallation
	{
		[SuppressMessage("ReSharper", "InvertIf")]
		static BizTalkInstallation()
		{
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			using (var btsKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0"))
			{
				if (btsKey?.GetValue("InstallPath") is string installPath)
				{
					IsInstalled = true;
					_installationPath = installPath;
					_pipelineToolsPath = Path.Combine(installPath, @"SDK\Utilities\PipelineTools\");
					using (var administrationKey = btsKey.OpenSubKey("Administration"))
					{
						_managementDatabaseName = (string) administrationKey?.GetValue("MgmtDBName");
						_managementDatabaseServer = (string) administrationKey?.GetValue("MgmtDBServer");
						_sqlScriptsPath = (string) administrationKey?.GetValue("SQLScriptPath");
					}
					// see Microsoft.BizTalk.Studio.Extensibility.ProjectSystemHelper, Microsoft.BizTalk.Studio.Extensibility, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
					using (var xmlToolsKey = btsKey.OpenSubKey("XML TOOLS"))
					{
						_developerToolsPath = (string) xmlToolsKey?.GetValue("DataFilesPath");
					}
				}
			}
		}

		public static string DeveloperToolsPath => _developerToolsPath ?? ThrowInstallationException();

		public static string InstallationPath => _installationPath ?? ThrowInstallationException();

		internal static bool IsConfigured => IsInstalled && _managementDatabaseName != null && _managementDatabaseServer != null;

		public static bool IsInstalled { get; }

		public static string ManagementDatabaseName => _managementDatabaseName ?? ThrowInstallationException();

		public static string ManagementDatabaseServer => _managementDatabaseServer ?? ThrowInstallationException();

		public static string PipelineToolsPath => _pipelineToolsPath ?? ThrowInstallationException();

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static string SqlScriptsPath => _sqlScriptsPath ?? ThrowInstallationException();

		private static string ThrowInstallationException([CallerMemberName] string name = null)
		{
			if (!IsInstalled) throw new InvalidOperationException("BizTalk Server is not installed.");
			throw new InvalidOperationException($"BizTalk Server installation is incomplete; {name} could not be determined.");
		}

		private static readonly string _developerToolsPath;
		private static readonly string _installationPath;
		private static readonly string _managementDatabaseName;
		private static readonly string _managementDatabaseServer;
		private static readonly string _pipelineToolsPath;
		private static readonly string _sqlScriptsPath;
	}
}
