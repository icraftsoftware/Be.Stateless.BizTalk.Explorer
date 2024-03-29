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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Management;
using Be.Stateless.BizTalk.Reflection.Extensions;

namespace Be.Stateless.BizTalk.Reflection
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public sealed class BizTalkAssemblyResolver : IDisposable
	{
		static BizTalkAssemblyResolver()
		{
			_systemProbingFolderPaths = BizTalkInstallation.IsInstalled
				? new[] {
					BizTalkInstallation.InstallationPath,
					BizTalkInstallation.DeveloperToolsPath,
					BizTalkInstallation.PipelineToolsPath
				}
				: Array.Empty<string>();
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public BizTalkAssemblyResolver(Action<string> logAppender, params string[] probingFolderPaths) : this(logAppender, false, probingFolderPaths) { }

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Beginning with the .NET Framework 4, the <see cref="ResolveEventHandler"/> event is raised for all assemblies,
		/// including resource assemblies. In earlier versions, the event was not raised for resource assemblies. If the
		/// operating system is localized, the handler might be called multiple times: once for each culture in the fallback
		/// chain.
		/// </para>
		/// </remarks>
		/// <param name="logAppender">
		/// </param>
		/// <param name="skipResourceAssemblies">
		/// Whether to skip resolution for resource assemblies.
		/// </param>
		/// <param name="probingFolderPaths">
		/// </param>
		/// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.assemblyresolve">AppDomain.AssemblyResolve Event</seealso>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public BizTalkAssemblyResolver(Action<string> logAppender, bool skipResourceAssemblies, params string[] probingFolderPaths)
		{
			_logAppender = logAppender;
			_skipResourceAssemblies = skipResourceAssemblies;
			_userProbingFolderPaths = probingFolderPaths?
					.Where(p => !string.IsNullOrWhiteSpace(p))
					.Distinct()
					.ToArray()
				?? Array.Empty<string>();
			_assembliesPendingResolution = new();
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

			if (!_systemProbingFolderPaths.Any()) logAppender?.Invoke("BizTalk system folder paths could not be found.");
		}

		#region IDisposable Members

		public void Dispose()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
		}

		#endregion

		[SuppressMessage("ReSharper", "CommentTypo")]
		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			// see https://docs.microsoft.com/en-us/troubleshoot/dotnet/framework/serialization-onassemblyresolve-event-causes-recursion#workaround-2-write-a-more-robust-assembly-resolve-handler
			// see https://docs.microsoft.com/en-us/dotnet/standard/assembly/resolve-loads#the-correct-way-to-handle-assemblyresolve
			if (_assembliesPendingResolution.Contains(args.Name)) return null;

			var assemblyName = new AssemblyName(args.Name);
			if (_skipResourceAssemblies && assemblyName.IsResourceAssembly()) return null;
			if (assemblyName.IsNonExistentMicrosoftAssembly() || assemblyName.IsNonExistentStatelessAssembly()) return null;
			try
			{
				if (!_assembliesPendingResolution.Add(args.Name)) return null;
				_logAppender?.Invoke(
					args.RequestingAssembly != null
						? $"Resolving assembly '{args.Name}' for assembly '{args.RequestingAssembly.FullName}'."
						: $"Resolving assembly '{args.Name}'.");
				var resolvedPath = _systemProbingFolderPaths.Concat(_userProbingFolderPaths)
					.Select(
						path => {
							var probingPath = Path.Combine(path, assemblyName.Name + ".dll");
							_logAppender?.Invoke($"Probing '{probingPath}'.");
							return probingPath;
						})
					.FirstOrDefault(File.Exists);
				if (resolvedPath != null)
				{
					_logAppender?.Invoke($"Resolved assembly '{resolvedPath}'.");
					return AssemblyLoader.Load(resolvedPath);
				}
				_logAppender?.Invoke($"Could not resolve assembly '{args.Name}'.");
				return null;
			}
			finally
			{
				_assembliesPendingResolution.Remove(args.Name);
			}
		}

		private static readonly string[] _systemProbingFolderPaths;
		private readonly HashSet<string> _assembliesPendingResolution;
		private readonly Action<string> _logAppender;
		private readonly bool _skipResourceAssemblies;
		private readonly string[] _userProbingFolderPaths;
	}
}
