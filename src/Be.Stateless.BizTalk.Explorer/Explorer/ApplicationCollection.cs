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
using Microsoft.BizTalk.ExplorerOM;
using BizTalkApplicationCollection = Microsoft.BizTalk.ExplorerOM.ApplicationCollection;

namespace Be.Stateless.BizTalk.Explorer
{
	public sealed class ApplicationCollection : IDisposable
	{
		#region IDisposable Members

		public void Dispose()
		{
			_explorer.Dispose();
		}

		#endregion

		public Application this[string name]
		{
			get
			{
				var explorerApplication = BizTalkApplicationCollection[name];
				if (explorerApplication == null)
					throw new ArgumentException(
						$"BizTalk Server Application '{name}' cannot be found in BizTalk Server Group [{BizTalkServerGroup.ManagementDatabase}].",
						nameof(name));
				return new Application(explorerApplication, this);
			}
		}

		private BizTalkApplicationCollection BizTalkApplicationCollection
		{
			get
			{
				_explorer = new BtsCatalogExplorer();
				try
				{
					_explorer.ConnectionString = BizTalkServerGroup.ManagementDatabase.ConnectionString;
					_explorer.Refresh();
					return _explorer.Applications;
				}
				catch
				{
					// TODO ? necessary ?
					_explorer.DiscardChanges();
					_explorer.Dispose();
					throw;
				}
			}
		}

		private BtsCatalogExplorer _explorer;
	}
}
