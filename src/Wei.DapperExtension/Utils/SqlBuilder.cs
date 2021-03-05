using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Wei.DapperExtension.SqlAdapter;

namespace Wei.DapperExtension.Utils
{
    public class SqlBuilder<T>
    {
        private readonly CacheUtil _cache = CacheUtil.GetInstance();
        private readonly string _tableName = CacheUtil.GetTableName<T>();
        private readonly ISqlAdapter<T> _sqlAdapter;
        public SqlBuilder(IDbConnection connection)
        {
            var name = connection.ToString();
            if (name.Contains(".SqlConnection"))
                _sqlAdapter = new SqlServerAdapter<T>();
            else if (name.Contains(".MySqlConnection"))
                _sqlAdapter = new MySqlSqlAdapter<T>();
            else if (name.Contains(".SQLiteConnection"))
                _sqlAdapter = new SQLiteAdapter<T>();
            else
                throw new NotSupportedException(name);
        }
        public string Sql { get; private set; }

        public void BuildInsertSql()
        {
            var column = new StringBuilder();
            var parameter = new StringBuilder();
            AppendInsertColumn(column, parameter);
            Sql = $"INSERT INTO {_tableName} ({column}) VALUES ({parameter});";
        }

        public void BuildInsertAndGetIdSql()
        {
            var column = new StringBuilder();
            var parameter = new StringBuilder();
            AppendInsertColumn(column, parameter);
            Sql = $"INSERT INTO {_tableName} ({column}) VALUES ({parameter});{_sqlAdapter.GetIncrementIdSql()};";
        }

        public DynamicParameters BuildDeleteSql(object id)
        {
            var columnName = CacheUtil.GetColumnName(GetSingleKey());
            var parameter = new DynamicParameters();
            parameter.Add($"@{columnName}", id);
            Sql = $"DELETE FROM {_tableName} WHERE {columnName} = @{columnName};";
            return parameter;
        }

        public void BuildDeleteSql()
        {
            var where = new StringBuilder();
            var primaryKeys = _cache.GetPrimaryKeys<T>();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                var primaryKey = primaryKeys[i];
                where.Append($"{CacheUtil.GetColumnName(primaryKey)} = @{primaryKey.Name}");
                if (i < primaryKeys.Length - 1) where.Append($" AND ");
            }
            Sql = $"DELETE FROM {_tableName} WHERE {where};";
        }

        public DynamicParameters BuildDeleteSql(Expression<Func<T, bool>> predicate)
        {
            var wherePart = predicate.Build();
            if (!wherePart.HasSql)
                throw new Exception("没有检测到where条件");
            Sql = $"DELETE FROM {_tableName} WHERE {wherePart.Sql}";
            return wherePart.DynamicParameters;
        }

        public DynamicParameters BuildGetSql(object id)
        {
            var key = GetSingleKey();
            Sql = CacheUtil.GetQuerySql<T>(key);
            var parameters = new DynamicParameters();
            parameters.Add($"@{CacheUtil.GetColumnName(key)}", id);
            return parameters;
        }

        public DynamicParameters BuildFirstSql(Expression<Func<T, bool>> predicate)
        {
            var wherePart = predicate.Build();
            var sqlFormat = _sqlAdapter.GetFirstSql();
            var where = string.Empty;
            if (wherePart.HasSql)
                where = $" WHERE {wherePart.Sql} ";
            Sql = string.Format(sqlFormat, _tableName, where);
            return wherePart.DynamicParameters;
        }

        public void BuildUpdateSql()
        {
            var updateColumn = new StringBuilder();
            var excludeIdProps = _cache.GetTypePropertiesCache<T>(true);
            for (var i = 0; i < excludeIdProps.Count; i++)
            {
                var property = excludeIdProps[i];
                updateColumn.Append($"{CacheUtil.GetColumnName(property)} = @{property.Name}");
                if (i < excludeIdProps.Count - 1) updateColumn.Append(", ");
            }
            var where = new StringBuilder();
            var primaryKeys = _cache.GetPrimaryKeys<T>();
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                var primaryKey = primaryKeys[i];
                where.Append($"{CacheUtil.GetColumnName(primaryKey)} = @{primaryKey.Name}");
                if (i < primaryKeys.Length - 1) where.Append($" AND ");
            }
            Sql = $"UPDATE {_tableName} SET {updateColumn} WHERE {where};";
        }

        public DynamicParameters BuildUpdateSql(Expression<Func<T, bool>> predicate, T entity, T oldEntity)
        {
            var wherePart = predicate.Build();
            var parameters = wherePart.DynamicParameters;
            var excludeIdProps = _cache.GetTypePropertiesCache<T>(true);
            var changeColumns = new List<string>();
            for (var i = 0; i < excludeIdProps.Count; i++)
            {
                var property = excludeIdProps[i];
                var oldValue = property.GetValue(oldEntity);
                var newValue = property.GetValue(entity);
                if (oldValue?.GetHashCode() != newValue?.GetHashCode())
                {
                    changeColumns.Add($"{CacheUtil.GetColumnName(property)} = @{property.Name}");
                    parameters.Add($"@{CacheUtil.GetColumnName(property)}", newValue);
                }
            }
            if (changeColumns.Count <= 0)
                return null;

            Sql = $"UPDATE {_tableName} SET {string.Join(",", changeColumns)} WHERE {wherePart.Sql};";
            return parameters;
        }

        public DynamicParameters BuildGetAllSql(Expression<Func<T, bool>> predicate, string orderBy)
        {
            var where = string.Empty;
            var wherePart = predicate.Build();
            if (wherePart.HasSql)
                where = $"WHERE {wherePart.Sql}";
            Sql = $"SELECT * FROM {_tableName} {where} {orderBy};";
            return wherePart.DynamicParameters;
        }

        public DynamicParameters BuildGetPageSql(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, string orderBy = null)
        {
            var where = string.Empty;
            var wherePart = predicate.Build();
            if (wherePart.HasSql)
                where = $"WHERE {wherePart.Sql}";
            if (string.IsNullOrEmpty(orderBy))
            {
                //没有排序时，默认已自增主键降序排列
                var keys = _cache.GetPrimaryKeys<T>();
                if (keys == null || keys.Length == 0)
                    throw new Exception($"[{_tableName}]未获取到主键信息");
                orderBy = $"ORDER BY {CacheUtil.GetColumnName(keys[0])} DESC";
            }
            var pageStart = (pageIndex - 1) * pageSize;
            var page = _sqlAdapter.GetPageSql(pageStart, pageSize);
            Sql = $"SELECT {_sqlAdapter.GetCountSql()} FROM {_tableName} {where};SELECT * FROM {_tableName} {where} {orderBy} {page};";
            return wherePart.DynamicParameters;
        }

        private void AppendInsertColumn(StringBuilder column, StringBuilder parameter)
        {
            var excludeIdProps = _cache.GetTypePropertiesCache<T>(true);
            if (excludeIdProps == null && excludeIdProps.Count == 0)
                throw new Exception("没有找到需要新增的字段");
            for (var i = 0; i < excludeIdProps.Count; i++)
            {
                var property = excludeIdProps[i];
                column.AppendFormat("{0}", CacheUtil.GetColumnName(property));
                parameter.AppendFormat("@{0}", property.Name);
                if (i < excludeIdProps.Count - 1)
                {
                    column.Append(", ");
                    parameter.Append(", ");
                }
            }
        }

        private PropertyInfo GetSingleKey()
        {
            var keys = _cache.GetPrimaryKeys<T>();
            if (keys == null || keys.Length == 0)
                throw new Exception($"【{CacheUtil.GetTableName<T>()}】未获取到主键信息");

            if (keys != null && keys.Length > 1)
                throw new Exception("该不支持多主键");
            return keys[0];
        }
    }
}
