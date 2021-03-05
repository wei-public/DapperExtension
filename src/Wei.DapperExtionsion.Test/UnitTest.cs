using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wei.DapperExtension;
using Wei.DapperExtionsion.Test.Entities;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Wei.DapperExtionsion.Test
{
    public class UnitTest : SqlLiteTestBase
    {
        private T Insert<T>(T entity) where T : class => Connection.Insert<T>(entity);
        private Task<T> InsertAsync<T>(T entity) where T : class => Connection.InsertAsync(entity);

        [Fact, Order(0)]
        public void Init()
        {
            Connection.Execute(CreateTableSql);
            Assert.True(true);
        }

        [Fact, Order(1)]
        public void Test_Insert()
        {
            var methodName = nameof(Test_Insert);
            var result1 = Insert(new TestModelInt { MethodName = methodName });
            Assert.True(result1.Id > 0);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = Connection.FirstOrDefault<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal(multipeKeyEntity.TypeId, result2.TypeId);
            Assert.Equal(multipeKeyEntity.Type, result2.Type);
        }


        [Fact, Order(1)]
        public async Task Test_InsertAsync()
        {
            var methodName = nameof(Test_InsertAsync);
            var result = await InsertAsync(new TestModelInt { MethodName = methodName });
            Assert.True(result.Id > 0);

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = await Connection.FirstOrDefaultAsync<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal(multipeKeyEntity.TypeId, result2.TypeId);
            Assert.Equal(multipeKeyEntity.Type, result2.Type);
        }

        [Fact, Order(2)]
        public void Test_Update()
        {
            var methodName = nameof(Test_Update);
            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            intEntity.Result = "success";
            var result = Connection.Update(intEntity);
            Assert.Equal(1, result);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            multipeKeyEntity.Result = "success";
            Connection.Update(multipeKeyEntity);
            var result2 = Connection.FirstOrDefault<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal("success", result2.Result);

        }
        [Fact, Order(2)]
        public async Task Test_UpdateAsync()
        {
            var methodName = nameof(Test_UpdateAsync);
            var intEntity = await InsertAsync(new TestModelInt { MethodName = methodName });
            intEntity.Result = "success";
            var result = await Connection.UpdateAsync(intEntity);
            Assert.Equal(1, result);

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            multipeKeyEntity.Result = "success";
            await Connection.UpdateAsync(multipeKeyEntity);
            var result2 = await Connection.FirstOrDefaultAsync<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);

            Assert.Equal("success", result2.Result);
        }

        [Fact, Order(2)]
        public void Test_UpdateBy()
        {
            var methodName = nameof(Test_UpdateBy);
            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            var result = Connection.Update<TestModelInt>(x => x.Id == intEntity.Id, x => x.Result = "success");
            Assert.Equal(1, result);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            Connection.Update<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId, x => x.Result = "success");
            var result2 = Connection.FirstOrDefault<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal("success", result2.Result);
        }

        [Fact, Order(2)]
        public async Task Test_UpdateByAsync()
        {
            var methodName = nameof(Test_UpdateByAsync);
            var intEntity = await InsertAsync(new TestModelInt { MethodName = methodName });
            var result = await Connection.UpdateAsync<TestModelInt>(x => x.Id == intEntity.Id, x => x.Result = "success");
            Assert.Equal(1, result);

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            await Connection.UpdateAsync<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId, x => x.Result = "success");
            var result2 = await Connection.FirstOrDefaultAsync<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal("success", result2.Result);

        }

        [Fact, Order(3)]
        public void Test_Get()
        {
            var intEntity = Insert(new TestModelInt { MethodName = nameof(Test_Get) });
            var result = Connection.Get<TestModelInt>(intEntity.Id);
            Assert.Equal(intEntity.Id, result.Id);

        }

        [Fact, Order(3)]
        public async Task Test_GetAsync()
        {
            var intEntity = await InsertAsync(new TestModelInt { MethodName = nameof(Test_GetAsync) });
            await Connection.InsertAsync(intEntity);
            var result = await Connection.GetAsync<TestModelInt>(intEntity.Id);
            Assert.Equal(intEntity.Id, result.Id);
        }

        [Fact, Order(4)]
        public void Test_FirstOrDefault()
        {
            var methodName = nameof(Test_FirstOrDefault);
            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            var result = Connection.FirstOrDefault<TestModelInt>(x => x.Id == intEntity.Id);
            Assert.Equal(intEntity.Id, result.Id);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = Connection.FirstOrDefault<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal(multipeKeyEntity.TypeId, result2.TypeId);
            Assert.Equal(multipeKeyEntity.Type, result2.Type);
        }

        [Fact, Order(4)]
        public async Task Test_FirstOrDefaultAsync()
        {
            var intEntity = await InsertAsync(new TestModelInt { MethodName = nameof(Test_FirstOrDefaultAsync) });
            var result = await Connection.FirstOrDefaultAsync<TestModelInt>(x => x.Id == intEntity.Id);
            Assert.Equal(intEntity.Id, result.Id);
        }

        [Fact, Order(5)]
        public void Test_Delete()
        {
            var intEntity = Insert(new TestModelInt { MethodName = nameof(Test_Delete) });
            var result = Connection.Delete(intEntity);
            Assert.Equal(1, result);
        }

        [Fact, Order(5)]
        public async Task Test_DeleteAsync()
        {
            var intEntity = await InsertAsync(new TestModelInt { MethodName = nameof(Test_Delete) });
            var result = await Connection.DeleteAsync(intEntity);
            Assert.Equal(1, result);
        }

        [Fact, Order(5)]
        public void Test_DeleteById()
        {
            var intEntity = Insert(new TestModelInt { MethodName = nameof(Test_DeleteById) });
            var result = Connection.Delete<TestModelInt>(intEntity.Id);
            Assert.Equal(1, result);
        }



        [Fact, Order(5)]
        public async Task Test_DeleteByIdAsync()
        {
            var intEntity = await InsertAsync(new TestModelInt { MethodName = nameof(Test_DeleteByIdAsync) });
            var result = await Connection.DeleteAsync<TestModelInt>(intEntity.Id);
            Assert.Equal(1, result);
        }



        [Fact, Order(5)]
        public void Test_DeleteBy()
        {
            var methodName = nameof(Test_DeleteBy);
            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            var result = Connection.Delete<TestModelInt>(x => x.Id == intEntity.Id);
            Assert.Equal(1, result);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = Connection.Delete<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal(1, result2);
        }

        [Fact, Order(5)]
        public async Task Test_DeleteByAsync()
        {
            var methodName = nameof(Test_DeleteByAsync);
            var intEntity = await InsertAsync(new TestModelInt { MethodName = methodName });
            var result = await Connection.DeleteAsync<TestModelInt>(x => x.Id == intEntity.Id);
            Assert.Equal(1, result);

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = await Connection.DeleteAsync<TestModelMultipeKey>(x => x.Type == multipeKeyEntity.Type && x.TypeId == multipeKeyEntity.TypeId);
            Assert.Equal(1, result2);
        }

        [Fact, Order(6)]
        public void Test_GetAll()
        {
            var methodName = nameof(Test_GetAll);
            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            var result = Connection.GetAll<TestModelInt>(x => x.Id > 0);
            Assert.True(result.Count() > 0);

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = Connection.GetAll<TestModelMultipeKey>(x => x.MethodName == multipeKeyEntity.MethodName);
            Assert.True(result2.Count() > 0);
        }

        [Fact, Order(6)]
        public async Task Test_GetAllAsync()
        {
            var methodName = nameof(Test_GetAllAsync);
            var intEntity = await InsertAsync(new TestModelInt { MethodName = methodName });
            var result = await Connection.GetAllAsync<TestModelInt>(x => x.Id > 0);
            Assert.True(result.Count() > 0);

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = await Connection.GetAllAsync<TestModelMultipeKey>(x => x.MethodName == multipeKeyEntity.MethodName);
            Assert.True(result2.Count() > 0);
        }

        [Fact, Order(6)]
        public void Test_GetPage()
        {
            var methodName = nameof(Test_GetPage);

            var intEntity = Insert(new TestModelInt { MethodName = methodName });
            var result = Connection.GetPage<TestModelInt>(1, 5, x => x.Id > 0);
            Assert.True(result.Item1 > 0);
            Assert.Equal(5, result.Item2.Count());

            var multipeKeyEntity = Insert(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = Connection.GetPage<TestModelMultipeKey>(1, 5, x => x.TypeId != "");
            Assert.True(result2.Item1 > 0);
            Assert.Equal(5, result2.Item2.Count());
        }

        [Fact, Order(6)]
        public async Task Test_GetPageAsync()
        {
            var methodName = nameof(Test_GetPageAsync);
            var intEntity = await InsertAsync(new TestModelInt { MethodName = methodName });
            var result = await Connection.GetPageAsync<TestModelInt>(2, 5, x => x.Id > 0);
            Assert.True(result.Item1 > 0);
            Assert.Equal(5, result.Item2.Count());

            var multipeKeyEntity = await InsertAsync(new TestModelMultipeKey { Type = methodName, TypeId = Guid.NewGuid().ToString(), MethodName = methodName });
            var result2 = await Connection.GetPageAsync<TestModelMultipeKey>(1, 5, x => x.TypeId != "");
            Assert.True(result2.Item1 > 0);
            Assert.Equal(5, result2.Item2.Count());
        }
    }
}
