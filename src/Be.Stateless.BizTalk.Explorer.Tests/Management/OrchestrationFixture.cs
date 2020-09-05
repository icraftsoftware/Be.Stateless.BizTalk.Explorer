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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.BizTalk.ExplorerOM;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Management
{
	public class OrchestrationFixture : IDisposable
	{
		public OrchestrationFixture()
		{
			var assembly = Assembly.Load("Microsoft.BizTalk.Edi.BatchingOrchestration, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			_orchestrationType = assembly.GetType("Microsoft.BizTalk.Edi.BatchSuspendOrchestration.BatchElementSuspendService", true);
		}

		#region IDisposable Members

		public void Dispose()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
		}

		#endregion

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void EnlistEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Enlist()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void EnlistStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Enlist()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void EnlistUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Enlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Enlisted);
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StartEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Start()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Started);
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StartStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Start()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StartUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Start()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StopEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Stop()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StopStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Stop()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Enlisted);
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void StopUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Stop()).Should().Throw<COMException>();
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void ThrowsIfNotBtxServiceDerivedType()
		{
			Action(() => new Orchestration(typeof(string)))
				.Should().Throw<ArgumentException>()
				.WithMessage("Type 'System.String' is not an BTXService-derived orchestration type*");
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void UnenlistEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Action(() => orchestration.Unenlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Unenlisted);
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void UnenlistStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Action(() => orchestration.Unenlist()).Should().NotThrow();
			orchestration.Status.Should().Be(OrchestrationStatus.Unenlisted);
		}

		[SkippableFact(typeof(FileNotFoundException), typeof(TypeLoadException))]
		public void UnenlistUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Action(() => orchestration.Unenlist()).Should().Throw<COMException>();
		}

		private readonly Type _orchestrationType;
	}
}
