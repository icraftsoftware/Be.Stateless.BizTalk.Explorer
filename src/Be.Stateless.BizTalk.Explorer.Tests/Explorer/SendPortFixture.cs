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
using FluentAssertions;
using Microsoft.BizTalk.ExplorerOM;
using Xunit;

namespace Be.Stateless.BizTalk.Explorer
{
	public class SendPortFixture
	{
		[SkippableFact(typeof(ArgumentException), typeof(InvalidOperationException))]
		public void Unenlist()
		{
			var application = BizTalkServerGroup.Applications["BizTalk Server Default Application"];
			var sendPort = application.SendPorts["DummySendPort"];

			sendPort.Unenlist();
			application.ApplyChanges();
			sendPort.Status.Should().Be(PortStatus.Bound);

			sendPort.Enlist();
			application.ApplyChanges();
			sendPort.Status.Should().Be(PortStatus.Stopped);

			sendPort.Unenlist();
			application.ApplyChanges();
			sendPort.Status.Should().Be(PortStatus.Bound);

			sendPort.Start();
			application.ApplyChanges();
			sendPort.Status.Should().Be(PortStatus.Started);
		}
	}
}
