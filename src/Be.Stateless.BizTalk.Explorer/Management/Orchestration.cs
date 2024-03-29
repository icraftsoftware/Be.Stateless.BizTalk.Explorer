﻿#region Copyright & License

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
using System.Diagnostics.CodeAnalysis;
using System.Management;
using Be.Stateless.Reflection.Extensions;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.XLANGs.BTXEngine;

namespace Be.Stateless.BizTalk.Management
{
	public class Orchestration : IDisposable
	{
		public Orchestration(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (!typeof(BTXService).IsAssignableFrom(type))
				throw new ArgumentException(
					$"Type '{type.FullName}' is not an BTXService-derived orchestration type",
					nameof(type));
			_orchestrationType = type;
			_managementObject = new() { Path = ManagementPath };
			_managementObject.Get();
		}

		~Orchestration()
		{
			Dispose(false);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		public OrchestrationStatus Status
		{
			get
			{
				var status = _managementObject["OrchestrationStatus"];
				return status.ToString() switch {
					"2" => OrchestrationStatus.Unenlisted,
					"3" => OrchestrationStatus.Enlisted,
					"4" => OrchestrationStatus.Started,
					_ => throw new InvalidOperationException("Unknown WMI OrchestrationStatus.")
				};
			}
		}

		[SuppressMessage("ReSharper", "UseStringInterpolation")]
		private ManagementPath ManagementPath
		{
			get
			{
				var assemblyName = _orchestrationType.Assembly.GetName();
				var path = string.Format(
					@"\\.\root\MicrosoftBizTalkServer:MSBTS_Orchestration.AssemblyCulture='{0}',AssemblyName='{1}',AssemblyPublicKeyToken='{2}',AssemblyVersion='{3}',Name='{4}'",
					assemblyName.GetCultureName(),
					assemblyName.Name,
					assemblyName.GetPublicKeyTokenString(),
					assemblyName.Version,
					_orchestrationType.FullName);
				return new(path);
			}
		}

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Dispose pattern.")]
		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed) return;
			if (disposing) _managementObject?.Dispose();
			_isDisposed = true;
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public void EnsureStarted()
		{
			if (Status != OrchestrationStatus.Started)
			{
				EnsureNotUnenlisted();
				Start();
			}
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public void EnsureNotStarted()
		{
			if (Status == OrchestrationStatus.Started)
			{
				Stop();
			}
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public void EnsureUnenlisted()
		{
			if (Status != OrchestrationStatus.Unenlisted)
			{
				EnsureNotStarted();
				Unenlist();
			}
		}

		public void EnsureNotUnenlisted()
		{
			if (Status == OrchestrationStatus.Unenlisted)
			{
				Enlist();
			}
		}

		/// <summary>
		/// Enlists the orchestration by creating its activation subscription.
		/// </summary>
		public void Enlist()
		{
			WmiEnlist();
			WmiRefresh();
		}

		/// <summary>
		/// Starts the orchestration by enabling its activation subscription.
		/// </summary>
		public void Start()
		{
			WmiStart(1, 1, 1);
			WmiRefresh();
		}

		/// <summary>
		/// Stops the orchestration by disabling its activation subscription.
		/// </summary>
		public void Stop()
		{
			WmiStop();
			WmiRefresh();
		}

		/// <summary>
		/// Terminates all instances of the orchestration and removes its activation subscription.
		/// </summary>
		public void Unenlist()
		{
			WmiUnenlist(2);
			WmiRefresh();
		}

		/// <summary>
		/// Enlists the orchestration by creating its activation subscription.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/msbts-orchestration-enlist-method-wmi">MSBTS_Orchestration.Enlist</seealso>
		private void WmiEnlist()
		{
			_managementObject.InvokeMethod("Enlist", Array.Empty<object>());
		}

		/// <summary>
		/// Starts the orchestration by enabling its activation subscription.
		/// </summary>
		/// <param name="autoEnableReceiveLocationFlag">
		/// Specifies whether receive locations associated with this orchestration should be automatically enabled. Permissible
		/// values for this parameter are (1) "No auto enable of receive locations related to this orchestration", or (2) "Enable
		/// all receive locations related to this orchestration". Note that the integer values must be used in code and script.
		/// The default value is 1.
		/// </param>
		/// <param name="autoResumeOrchestrationInstanceFlag">
		/// Specifies whether service instances of this orchestration type that were manually suspended previously should be
		/// automatically resumed. Permissible values for this parameter are (1) "No auto resume of service instances of this
		/// orchestration", or (2) "Automatically resume all suspended service instances of this orchestration" Note that the
		/// integer values must be used in code and script. The default value is 2.
		/// </param>
		/// <param name="autoStartSendPortsFlag">
		/// Specifies whether send ports and send port groups imported by this orchestration should be automatically started.
		/// Permissible values for this parameter are (1) "No auto start of send ports and send port groups of this
		/// orchestration", or (2) "Start all send ports and send port groups associated with this orchestration." Note that the
		/// integer values must be used in code and script. If the value is 1 and there exists a send port or send port group
		/// that is in the Bound state, WMI will fail this orchestration start operation. The default value is 2.
		/// </param>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/msbts-orchestration-start-method-wmi">MSBTS_Orchestration.Start</seealso>
		private void WmiStart(uint autoEnableReceiveLocationFlag = 1, uint autoResumeOrchestrationInstanceFlag = 2, uint autoStartSendPortsFlag = 2)
		{
			_managementObject.InvokeMethod("Start", new object[] { autoEnableReceiveLocationFlag, autoResumeOrchestrationInstanceFlag, autoStartSendPortsFlag });
		}

		/// <summary>
		/// Stops the orchestration by disabling its activation subscription.
		/// </summary>
		/// <param name="autoDisableReceiveLocationFlag">
		/// Permissible values for this property are (1) "No auto disable of receive locations related to this Orchestration", or
		/// (2) "Disable all receive locations related to this orchestration that are not shared by other orchestration(s)". Note
		/// that the integer values must be used in code and script. The default value is 1.
		/// </param>
		/// <param name="autoSuspendServiceInstanceFlag">
		/// Permissible values for this property are (1) "No auto suspend of service instances of this Orchestration", or (2)
		/// "Suspend all running service instances of this Orchestration". Note that the integer values must be used in code and
		/// script. The default value is 2.
		/// </param>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/msbts-orchestration-stop-method-wmi">MSBTS_Orchestration.Stop</seealso>
		private void WmiStop(uint autoDisableReceiveLocationFlag = 1, uint autoSuspendServiceInstanceFlag = 2)
		{
			_managementObject.InvokeMethod("Stop", new object[] { autoDisableReceiveLocationFlag, autoSuspendServiceInstanceFlag });
		}

		/// <summary>
		/// Terminates all instances of the orchestration and removes its activation subscription.
		/// </summary>
		/// <param name="autoTerminateOrchestrationInstanceFlag">
		/// An Integer specifying whether instances of this orchestration type should be automatically terminated. Permissible
		/// values for this parameter are (1) "Do not terminate service instances of this orchestration," or (2) "Terminate all
		/// service instances of this orchestration." Note that the integer values must be used in code and script. The default
		/// value is 1.
		/// </param>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/msbts-orchestration-unenlist-method-wmi">MSBTS_Orchestration.Unenlist</seealso>
		private void WmiUnenlist(uint autoTerminateOrchestrationInstanceFlag = 1)
		{
			_managementObject.InvokeMethod("Unenlist", new object[] { autoTerminateOrchestrationInstanceFlag });
		}

		private void WmiRefresh()
		{
			_managementObject = new() { Path = _managementObject.Path };
			_managementObject.Get();
		}

		private readonly Type _orchestrationType;
		private bool _isDisposed;
		private ManagementObject _managementObject;
	}
}
