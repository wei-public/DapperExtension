using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Wei.DapperExtension.Utils;

namespace Wei.DapperExtension
{
    public static class DbConnectionExtension
    {

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体</returns>
        public static T Insert<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var incrementKey = CacheUtil.GetInstance().GetIncrementKey<T>();
            var sqlBuilder = new SqlBuilder<T>(connection);
            if (incrementKey == null)
            {
                sqlBuilder.BuildInsertSql();
                sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
                var insertRow = connection.Execute(sqlBuilder.Sql, entity, transaction, timeOut);
                if (insertRow > 0)
                    return entity;
                else
                    throw new Exception($"新增异常，受影响行数为：{insertRow}");
            }
            sqlBuilder.BuildInsertAndGetIdSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            var id = connection.QueryFirstOrDefault<int>(sqlBuilder.Sql, entity, transaction, commandTimeout: timeOut);
            if (id <= 0) throw new Exception("新增异常，未返回自增Id");
            incrementKey.SetValue(entity, Convert.ChangeType(id, incrementKey.PropertyType), null);
            return entity;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">实体</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体</returns>
        public async static Task<T> InsertAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var incrementKey = CacheUtil.GetInstance().GetIncrementKey<T>();
            var sqlBuilder = new SqlBuilder<T>(connection);
            if (incrementKey == null)
            {
                sqlBuilder.BuildInsertSql();
                sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
                var insertRow = await connection.ExecuteAsync(sqlBuilder.Sql, entity, transaction, timeOut);
                if (insertRow > 0)
                    return entity;
                else
                    throw new Exception($"新增异常，受影响行数为：{insertRow}");
            }
            sqlBuilder.BuildInsertAndGetIdSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            var id = await connection.QueryFirstOrDefaultAsync<int>(sqlBuilder.Sql, entity, transaction, timeOut);
            if (id <= 0) throw new Exception("新增异常，未返回自增Id");
            incrementKey.SetValue(entity, Convert.ChangeType(id, incrementKey.PropertyType), null);
            return entity;
        }

        /// <summary>
        /// 新增(批量)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entities">实体集合</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Insert<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildInsertSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entities);
            return connection.Execute(sqlBuilder.Sql, entities, transaction, timeOut);
        }

        /// <summary>
        /// 新增(批量)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entities">实体集合</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public async static Task<int> InsertAsync<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildInsertSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entities);
            return await connection.ExecuteAsync(sqlBuilder.Sql, entities, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="id">主键Id</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildDeleteSql(id);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return connection.Execute(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="id">主键Id</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public async static Task<int> DeleteAsync<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildDeleteSql(id);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return await connection.ExecuteAsync(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">要删除的实体（主键必须有值）</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Delete<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildDeleteSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            return connection.Execute(sqlBuilder.Sql, entity, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">要删除的实体（主键必须有值）</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public async static Task<int> DeleteAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildDeleteSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            return await connection.ExecuteAsync(sqlBuilder.Sql, entity, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">删除条件(避免删除全部数据，必须有条件)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Delete<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildDeleteSql(predicate);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return connection.Execute(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">删除条件(避免删除全部数据，必须有条件)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响行数</returns>
        public async static Task<int> DeleteAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildDeleteSql(predicate);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return await connection.ExecuteAsync(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 根据条件获取第一个或默认
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体对象</returns>
        public static T FirstOrDefault<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildFirstSql(predicate);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return connection.QueryFirstOrDefault<T>(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 根据条件获取第一个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">过滤条件</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体对象</returns>
        public async static Task<T> FirstOrDefaultAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildFirstSql(predicate);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return await connection.QueryFirstOrDefaultAsync<T>(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="id">主键id</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体对象</returns>
        public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildGetSql(id);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return connection.QueryFirstOrDefault<T>(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 根据Id获取
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="id">主键id</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体对象</returns>
        public async static Task<T> GetAsync<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameter = sqlBuilder.BuildGetSql(id);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameter);
            return await connection.QueryFirstOrDefaultAsync<T>(sqlBuilder.Sql, parameter, transaction, timeOut);
        }

        /// <summary>
        /// 更新(根据主键)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">需要更新实体对象,主键必须有值</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Update<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildUpdateSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            return connection.Execute(sqlBuilder.Sql, entity, transaction, timeOut);
        }

        /// <summary>
        /// 更新(根据主键)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="entity">需要更新实体对象,主键必须有值</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public async static Task<int> UpdateAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            sqlBuilder.BuildUpdateSql();
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, entity);
            return await connection.ExecuteAsync(sqlBuilder.Sql, entity, transaction, timeOut);
        }

        /// <summary>
        /// 更新(根据条件更新指定字段)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">更新条件(避免全表更新，必须有值)</param>
        /// <param name="updateAction">更新指定字段回调函数</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public static int Update<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, Action<T> updateAction, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (updateAction == null) return 0;
            var entity = connection.FirstOrDefault<T>(predicate, transaction, timeOut);
            if (entity == null) return 0;
            var oldEntity = entity.Copy();
            updateAction(entity);
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildUpdateSql(predicate, entity, oldEntity);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            return connection.Execute(sqlBuilder.Sql, parameters, transaction, timeOut);
        }

        /// <summary>
        /// 更新(根据条件更新指定字段)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">更新条件(避免全表更新，必须有值)</param>
        /// <param name="updateAction">更新指定字段回调函数</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>受影响的行数</returns>
        public async static Task<int> UpdateAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, Action<T> updateAction, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) where T : class
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (updateAction == null) return 0;
            var entity = await connection.FirstOrDefaultAsync<T>(predicate, transaction, timeOut);
            if (entity == null) return 0;
            var oldEntity = entity.Copy();
            updateAction(entity);
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildUpdateSql(predicate, entity, oldEntity);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            return string.IsNullOrEmpty(sqlBuilder.Sql) ? 0 : await connection.ExecuteAsync(sqlBuilder.Sql, parameters, transaction, timeOut);

        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序(eg: order by id desc)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体集合</returns>
        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate = null, string orderBy = null, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildGetAllSql(predicate, orderBy);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            return connection.Query<T>(sqlBuilder.Sql, parameters, transaction, commandTimeout: timeOut);
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序(eg: order by id desc)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>实体集合</returns>
        public async static Task<IEnumerable<T>> GetAllAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate = null, string orderBy = null, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildGetAllSql(predicate, orderBy);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            return await connection.QueryAsync<T>(sqlBuilder.Sql, parameters, transaction, commandTimeout: timeOut);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序,默认以主键倒序(eg: order by id desc)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>Tuple(总记录数,分页数据)</returns>
        public static Tuple<long, IEnumerable<T>> GetPage<T>(this IDbConnection connection, int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null, string orderBy = null, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildGetPageSql(predicate, pageIndex, pageSize, orderBy);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            var multi = connection.QueryMultiple(sqlBuilder.Sql, parameters, transaction, timeOut);
            var totalCount = multi.Read<long>().FirstOrDefault();
            var items = multi.Read<T>();
            return new Tuple<long, IEnumerable<T>>(totalCount, items);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序,默认以主键倒序(eg: order by id desc)</param>
        /// <param name="transaction">事务</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
        /// <returns>Tuple(总记录数,分页数据)</returns>
        public async static Task<Tuple<long, IEnumerable<T>>> GetPageAsync<T>(this IDbConnection connection, int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null, string orderBy = null, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)
        {
            var sqlBuilder = new SqlBuilder<T>(connection);
            var parameters = sqlBuilder.BuildGetPageSql(predicate, pageIndex, pageSize, orderBy);
            sqlExecuteBeforeAction?.Invoke(sqlBuilder.Sql, parameters);
            var multi = await connection.QueryMultipleAsync(sqlBuilder.Sql, parameters, transaction, timeOut);
            var totalCount = (await multi.ReadAsync<long>()).FirstOrDefault();
            var items = await multi.ReadAsync<T>();
            return new Tuple<long, IEnumerable<T>>(totalCount, items);
        }
    }
}
