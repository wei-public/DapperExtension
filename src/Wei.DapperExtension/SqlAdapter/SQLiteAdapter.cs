using Wei.DapperExtension.Utils;

namespace Wei.DapperExtension.SqlAdapter
{
    public class SQLiteAdapter<T> : ISqlAdapter<T>
    {
        public string GetIncrementIdSql() => CacheUtil.GetInstance().HasIncrementKey<T>() ? "SELECT LAST_INSERT_ROWID()" : "";
        public string GetFirstSql() => "SELECT * FROM {0} {1} LIMIT 1";
        public string GetCountSql() => "COUNT(1)";
        public string GetPageSql(int pageStart, int pageSize) => $"LIMIT {pageSize} OFFSET {pageStart}";
    }
}
