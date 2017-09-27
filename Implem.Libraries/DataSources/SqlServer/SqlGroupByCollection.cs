﻿using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlGroupByCollection : ListEx<SqlGroupBy>
    {
        public SqlGroupByCollection(params SqlGroupBy[] sqlGroupByCollection)
        {
            AddRange(sqlGroupByCollection);
        }

        public SqlGroupByCollection Add(string columnBracket, string tableName)
        {
            Add(new SqlGroupBy(columnBracket, tableName));
            return this;
        }

        public void BuildCommandText(StringBuilder commandText, string tableBracket)
        {
            if (this.Any())
            {
                commandText.Append("group by ", this.Select(o =>
                    Sqls.TableAndColumnBracket(
                        tableBracket: tableBracket,
                        columnBracket: o.ColumnBracket))
                            .Join(), " ");
            }
        }
    }
}
