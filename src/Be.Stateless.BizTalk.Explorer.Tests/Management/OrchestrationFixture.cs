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
using System.Reflection;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Microsoft.BizTalk.ExplorerOM;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Management
{
	[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
	public class OrchestrationFixture : IDisposable
	{
		[SuppressMessage("ReSharper", "InvertIf")]
		public OrchestrationFixture()
		{
			if (BizTalkServerGroup.IsConfigured)
			{
				var assembly = Assembly.Load("Microsoft.BizTalk.Edi.BatchingOrchestration, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
				_orchestrationType = assembly.GetType("Microsoft.BizTalk.Edi.BatchSuspendOrchestration.BatchElementSuspendService", true);
			}
		}

		#region IDisposable Members

		[SuppressMessage("ReSharper", "InvertIf")]
		public void Dispose()
		{
			if (BizTalkServerGroup.IsConfigured)
			{
				var orchestration = new Orchestration(_orchestrationType);
				orchestration.EnsureUnenlisted();
			}
		}

		#endregion

		[SkippableFact]
		public void EnlistEnlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Enlist()).Should().Throw<COMException>();
		}

		[SkippableFact]
		public void EnlistStartedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Enlist()).Should().Throw<COMException>();
		}

		[SkippableFact]
		public void EnlistUnenlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Enlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Enlisted);
		}

		[SkippableFact]
		public void StartEnlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Start()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Started);
		}

		[SkippableFact]
		public void StartStartedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Start()).Should().Throw<COMException>();
		}

		[SkippableFact]
		public void StartUnenlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Start()).Should().Throw<COMException>();
		}

		[SkippableFact]
		public void StopEnlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Stop()).Should().Throw<COMException>();
		}

		[SkippableFact]
		public void StopStartedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Stop()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Enlisted);
		}

		[SkippableFact]
		public void StopUnenlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Stop()).Should().Throw<COMException>();
		}

		[SkippableFact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void ThrowsIfNotBtxServiceDerivedType()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Action(() => new Orchestration(typeof(string)))
				.Should().Throw<ArgumentException>()
				.WithMessage("Type 'System.String' is not an BTXService-derived orchestration type*");
		}

		[SkippableFact]
		public void UnenlistEnlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Unenlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Unenlisted);
		}

		[SkippableFact]
		public void UnenlistStartedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Unenlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Unenlisted);
		}

		[SkippableFact]
		public void UnenlistUnenlistedOrchestration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Unenlist()).Should().Throw<COMException>();
		}

		private readonly Type _orchestrationType;
	}
}
