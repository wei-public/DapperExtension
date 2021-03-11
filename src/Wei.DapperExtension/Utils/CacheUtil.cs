using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wei.DapperExtension.Attributes;

namespace Wei.DapperExtension.Utils
{
    public class CacheUtil
    {
        private static CacheUtil instance;
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> typeTableNameCache = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> typeAllPropertiesCache = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]> typeKeyPropertiesCache = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> typeQuerySqlCache = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        public static CacheUtil GetInstance()
        {
            if (instance == null)
                instance = new CacheUtil();
            return instance;
        }

        private bool IsWriteable(PropertyInfo pi)
        {
            var writeAttribute = pi.GetCustomAttribute<WriteAttribute>();
            if (writeAttribute == null) return true;
            return writeAttribute.IsWrite;
        }

        private bool IsIncrement(PropertyInfo pi)
        {
            var keyAttribute = pi.GetCustomAttribute<KeyAttribute>();
            if (keyAttribute == null) return false;
            return keyAttribute.IsIncrement;
        }

        /// <summary>
        /// 获取所有属性缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excludeIncrementKey">是否排除自增主键</param>
        /// <returns></returns>
        public List<PropertyInfo> GetTypePropertiesCache<T>(bool excludeIncrementKey = false)
        {
            var type = typeof(T);
            if (!typeAllPropertiesCache.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> allProperties))
            {
                allProperties = type.GetProperties().Where(IsWriteable).ToArray();
                typeAllPropertiesCache[type.TypeHandle] = allProperties;
            }
            if (excludeIncrementKey)
            {
                var incrementKey = GetIncrementKey<T>();
                if (incrementKey != null)
                    allProperties = allProperties.Where(x => x.Name != incrementKey.Name);
            }

            return allProperties.ToList();
        }

        /// <summary>
        /// 获取自增主键
        /// </summary>
        /// <returns></returns>
        public PropertyInfo GetIncrementKey<T>()
        {
            var type = typeof(T);
            PropertyInfo incrementKey = null;
            List<PropertyInfo> allProperties = null;
            if (typeKeyPropertiesCache.TryGetValue(type.TypeHandle, out PropertyInfo[] keyProperties))
            {
                if (keyProperties != null && keyProperties.Length > 0)
                    incrementKey = keyProperties.FirstOrDefault(IsIncrement);
            }
            else
            {
                allProperties = GetTypePropertiesCache<T>();
                var primaryKeys = allProperties.Where(x => x.GetCustomAttribute<KeyAttribute>() != null).ToList();
                if (primaryKeys != null && primaryKeys.Count > 0)
                {
                    typeKeyPropertiesCache[type.TypeHandle] = primaryKeys.ToArray();
                    incrementKey = primaryKeys.FirstOrDefault(IsIncrement);
                }
            }
            if (incrementKey == null)
            {
                if (allProperties == null)
                    allProperties = GetTypePropertiesCache<T>();
                // 如果有字段名为Id, 且类型为  int 或者 long  默认标记为自增主键
                var idProp = allProperties?.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null)
                {
                    if (idProp.PropertyType.Name.ToLower().Contains("int"))
                        incrementKey = idProp;
                }
            }

            return incrementKey;
        }

        /// <summary>
        /// 是否有自增主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasIncrementKey<T>()
        {
            return GetIncrementKey<T>() != null;
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public PropertyInfo[] GetPrimaryKeys<T>()
        {
            var type = typeof(T);
            List<PropertyInfo> allProperties = null;

            if (!typeKeyPropertiesCache.TryGetValue(type.TypeHandle, out PropertyInfo[] keyProperties))
            {
                allProperties = GetTypePropertiesCache<T>();
                var primaryKeys = allProperties.Where(x => x.GetCustomAttribute<KeyAttribute>() != null).ToList();
                if (primaryKeys != null && primaryKeys.Count > 0)
                {
                    typeKeyPropertiesCache[type.TypeHandle] = primaryKeys.ToArray();
                    return typeKeyPropertiesCache[type.TypeHandle];
                }
            }

            if (keyProperties == null || keyProperties.Length == 0)
            {
                if (allProperties == null)
                    allProperties = GetTypePropertiesCache<T>();
                // 如果有字段名为Id, 且类型为  int 或者 long  默认标记为主键
                var idProp = allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null)
                {
                    if (idProp.PropertyType.Name.ToLower().Contains("int"))
                        return new PropertyInfo[] { idProp };
                }
            }

            return keyProperties;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTableName<T>()
        {
            var type = typeof(T);
            if (!typeTableNameCache.TryGetValue(type.TypeHandle, out string tableName))
            {
                var attribute = type.GetCustomAttribute<TableAttribute>(false);
                tableName = attribute == null ? type.Name : attribute.Name;
                typeTableNameCache[type.TypeHandle] = tableName;
            }
            return tableName;
        }

        /// <summary>
        /// 设置数据库表名称(对于表名称动态生成的，可以动态设置表名称)
        /// </summary>
        public static void SetTableName<T>(string tableName) => typeTableNameCache[typeof(T).TypeHandle] = tableName;

        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static string GetColumnName(PropertyInfo pi)
        {
            var columnName = pi.Name;
            var attribute = pi.GetCustomAttribute<ColumnAttribute>();
            if (attribute != null)
                columnName = attribute.Name;
            return columnName;
        }

        /// <summary>
        /// 获取GetSql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetQuerySql<T>(PropertyInfo key)
        {
            var type = typeof(T);
            if (!typeQuerySqlCache.TryGetValue(typeof(T).TypeHandle, out string sql))
            {
                var tableName = GetTableName<T>();
                sql = $"SELECT * FROM {tableName} WHERE {GetColumnName(key)} = @{GetColumnName(key)}";
                typeQuerySqlCache[type.TypeHandle] = sql;
            }
            return sql;
        }

    }
}
