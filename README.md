# Wei.DapperExtension
> nuget package **Wei.DapperExtension**

基于.netstandard2.0平台对[Dapper](https://github.com/StackExchange/Dapper) 组件封装的CURD


|数据库| 是否支持 |
| ------------| --------------- |
| SqlServer | ✔ |
| MySql     | ✔ |
| SqlLite     | ✔ |
| 其他 ...     | 待添加|



***

## 通用CURD扩展方法
> 所有接口分为同步和异步，下面只列出同步接口
```C#
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
public static T Insert<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

/// <summary>
/// 新增(批量)
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <param name="connection">数据库连接对象</param>
/// <param name="entities">实体集合/param>
/// <param name="transaction">事务</param>
/// <param name="timeOut">超时时间</param>
/// <param name="sqlExecuteBeforeAction">sql执行之前回调</param>
/// <returns>受影响的行数</returns>
public static int Insert<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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
public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null) 

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
public static int Delete<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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
public static int Delete<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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
public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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
public static int Update<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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
public static int Update<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, Action<T> updateAction, IDbTransaction transaction = null, int? timeOut = null, Action<string, object> sqlExecuteBeforeAction = null)

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


```

> 推荐>>>基于Wei.DapperExtension封装的[DapperRepository](https://github.com/wei-public/DapperRepository)