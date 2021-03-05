using Wei.DapperExtension.Utils;

namespace Wei.DapperExtension.SqlAdapter
{
    public class SqlServerAdapter<T> : ISqlAdapter<T>
    {
        public string GetIncrementIdSql() => CacheUtil.GetInstance().HasIncrementKey<T>() ? "SELECT SCOPE_IDENTITY() id" : "";
        public string GetFirstSql() => "SELECT TOP 1 * FROM {0} {1}";
        public string GetCountSql() => "COUNT_BIG(1)";
        public string GetPageSql(int pageStart, int pageSize) => $"OFFSET {pageStart} ROWS FETCH NEXT {pageSize} ROWS ONLY";
    }
}
