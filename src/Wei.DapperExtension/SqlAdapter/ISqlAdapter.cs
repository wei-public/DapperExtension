namespace Wei.DapperExtension.SqlAdapter
{
    public interface ISqlAdapter<T>
    {
        string GetIncrementIdSql();
        string GetFirstSql();
        string GetCountSql();
        string GetPageSql(int pageStart, int pageSize);
    }
}
