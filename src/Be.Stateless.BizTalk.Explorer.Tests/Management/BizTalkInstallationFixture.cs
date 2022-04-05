#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Management
{
	public class BizTalkInstallationFixture
	{
		[SkippableFact]
		public void DeveloperToolsPath()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.DeveloperToolsPath.Should().Be($@"{INSTALLATION_PATH}\Developer Tools\");
		}

		[SkippableFact]
		public void InstallationPath()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.InstallationPath.Should().Be($@"{INSTALLATION_PATH}\");
		}

		[Fact]
		public void IsInstalled()
		{
			BizTalkInstallation.IsInstalled.Should().Be(BizTalkServerGroup.IsConfigured);
		}

		[SkippableFact]
		public void ManagementDatabaseName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.ManagementDatabaseName.Should().Be("BizTalkMgmtDb");
		}

		[SkippableFact]
		public void ManagementDatabaseServer()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.ManagementDatabaseServer.Should().Be(Environment.MachineName);
		}

		[SkippableFact]
		public void PipelineToolsPath()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.PipelineToolsPath.Should().Be($@"{INSTALLATION_PATH}\SDK\Utilities\PipelineTools\");
		}

		[SkippableFact]
		public void SqlScriptsPath()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			BizTalkInstallation.SqlScriptsPath.Should().Be($@"{INSTALLATION_PATH}\Schema\");
		}

		private const string INSTALLATION_PATH = @"C:\Program Files (x86)\Microsoft BizTalk Server";
	}
}
