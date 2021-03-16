using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Wei.DapperExtension.Utils;
using Wei.DapperExtionsion.Test.Entities;
using Xunit;
using System.Linq;

namespace Wei.DapperExtionsion.Test
{
    public class WhereBuilderTest
    {
        public class BuilderTest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class WhereInput
        {
            public int Id { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public bool IsDelete { get; set; }
            public int[] Ids { get; set; }
        }

        [Fact(DisplayName = "=")]
        public void Test_Equal()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id == 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id = 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id == input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id = @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }


        [Fact(DisplayName = ">")]
        public void Test_GreaterThan()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id > 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id > 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id > input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id > @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }

        [Fact(DisplayName = ">=")]
        public void Test_GreaterThanOrEqual()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id >= 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id >= 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id >= input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id >= @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }

        [Fact(DisplayName = "<")]
        public void Test_LessThan()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id < 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id < 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id < input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id < @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }

        [Fact(DisplayName = "<=")]
        public void Test_LessThanOrEqual()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id <= 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id <= 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id <= input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id <= @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }

        [Fact(DisplayName = "<>")]
        public void Test_NotEqual()
        {
            Expression<Func<BuilderTest, bool>> expression_const = x => x.Id != 0;
            var builder_const = expression_const.Build();
            Assert.Equal("Id <> 0", builder_const.Sql);

            var input = new WhereInput { Id = 1 };
            Expression<Func<BuilderTest, bool>> expression_member = x => x.Id != input.Id;
            var builder_member = expression_member.Build();
            Assert.Equal("Id <> @1", builder_member.Sql);
            Assert.Equal(1, builder_member.Parameters.Count);
            Assert.Equal(1, builder_member.Parameters[0].Value);
        }

        [Fact(DisplayName = "Contains")]
        public void Test_Contains()
        {
            var ids = new int[] { 1, 2, 3 };
            Expression<Func<BuilderTest, bool>> expression_contains = x => ids.Contains(x.Id);
            var builder_contains = expression_contains.Build();
            Assert.Equal("Id IN (@1,@2,@3)", builder_contains.Sql);
            Assert.Equal(ids.Length, builder_contains.Parameters.Count);
            for (int i = 0; i < ids.Length; i++)
            {
                Assert.Equal(ids[i], builder_contains.Parameters[i].Value);
            }
        }

        [Fact(DisplayName = "NotContains")]
        public void Test_NotContains()
        {
            var ids = new int[] { 1, 2, 3 };
            Expression<Func<BuilderTest, bool>> expression_notContains = x => !ids.Contains(x.Id);

            var builder_notContains = expression_notContains.Build();
            Assert.Equal("(NOT Id IN (@1,@2,@3))", builder_notContains.Sql);
            Assert.Equal(ids.Length, builder_notContains.Parameters.Count);
            for (int i = 0; i < ids.Length; i++)
            {
                Assert.Equal(ids[i], builder_notContains.Parameters[i].Value);
            }
        }

        [Fact(DisplayName = "Like")]
        public void Test_Like()
        {
            Expression<Func<BuilderTest, bool>> expression_like = x => x.Name.Contains("123");
            var builder_like = expression_like.Build();
            Assert.Equal("Name LIKE '%123%'", builder_like.Sql);
        }

        [Fact(DisplayName = "StartsWith")]
        public void Test_StartsWith()
        {
            Expression<Func<BuilderTest, bool>> expression_startsWith = x => x.Name.StartsWith("123");
            var builder_startsWith = expression_startsWith.Build();
            Assert.Equal("Name LIKE '123%'", builder_startsWith.Sql);
        }

        [Fact(DisplayName = "EndsWith")]
        public void Test_EndsWith()
        {
            var name = "123";
            Expression<Func<BuilderTest, bool>> expression_endsWith = x => x.Name.EndsWith(name);
            var builder_endsWith = expression_endsWith.Build();
            Assert.Equal("Name LIKE @1", builder_endsWith.Sql);
            Assert.Equal("%123", builder_endsWith.Parameters[0].Value);
        }

        [Fact(DisplayName = "And")]
        public void Test_And()
        {
            Expression<Func<BuilderTest, bool>> expression_and = x => x.Id == 1 && x.Name == "123";
            var builder_and = expression_and.Build();
            Assert.Equal("Id = 1 AND Name = '123'", builder_and.Sql);
        }

        [Fact(DisplayName = "Or")]
        public void Test_Or()
        {
            Expression<Func<BuilderTest, bool>> expression_or = x => x.Id == 1 || x.Name.StartsWith("123");
            var builder_or = expression_or.Build();
            Assert.Equal("(Id = 1 OR Name LIKE '123%')", builder_or.Sql);
        }

        [Fact(DisplayName = "NotBool")]
        public void Test_Bool()
        {
            Expression<Func<BuilderTest, bool>> expression_or = x => !x.IsDelete;
            var builder_or = expression_or.Build();
            Assert.Equal("IsDelete = 1", builder_or.Sql);
        }
    }
}
