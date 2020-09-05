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
using System.Data.SqlClient;
using System.Diagnostics;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Explorer
{
	public class BizTalkServerManagementDatabase
	{
		public BizTalkServerManagementDatabase(string server, string database)
		{
			if (server.IsNullOrEmpty()) throw new ArgumentNullException(nameof(server));
			if (database.IsNullOrEmpty()) throw new ArgumentNullException(nameof(database));
			Server = server;
			Database = database;
		}

		#region Base Class Member Overrides

		public override string ToString()
		{
			return $"{Server}:{Database}";
		}

		#endregion

		public string ConnectionString => new SqlConnectionStringBuilder {
			ApplicationName = $"ExplorerOM/{Process.GetCurrentProcess().ProcessName}",
			DataSource = Server,
			InitialCatalog = Database,
			IntegratedSecurity = true
		}.ConnectionString;

		public string Database { get; }

		public string Server { get; }
	}
}
