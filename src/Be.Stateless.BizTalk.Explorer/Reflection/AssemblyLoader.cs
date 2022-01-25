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

using System.Reflection;

namespace Be.Stateless.BizTalk.Reflection
{
	public static class AssemblyLoader
	{
		public static Assembly Load(string path)
		{
			// see https://stackoverflow.com/a/1477899/1789441
			// see https://stackoverflow.com/a/41858160/1789441
			// see https://stackoverflow.com/a/7354279/
			// see https://docs.microsoft.com/en-us/archive/blogs/suzcook/loadassemblyname
			return Assembly.Load(Assembly.LoadFrom(path).GetName());
		}
	}
}
