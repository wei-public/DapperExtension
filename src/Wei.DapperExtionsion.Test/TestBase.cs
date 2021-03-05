using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace Wei.DapperExtionsion.Test
{
    public class SqlServerTestBase
    {
        public SqlServerTestBase()
        {
            Connection = new SqlConnection("data source=localhost;initial catalog=demo;user id=sa;password=sasa;");
            CreateTableSql = GetCreateTableSql();
        }
        public IDbConnection Connection { get; }
        public string CreateTableSql { get; }
        private string GetCreateTableSql()
        {
            var sql = @"
                    IF EXISTS(Select 1 From Sysobjects Where Name='TestModelInt')
                    DROP table TestModelInt

                    CREATE TABLE [dbo].[TestModelInt] (
                      [Id] int  IDENTITY(1,1) NOT NULL,
                      [MethodName] varchar(255) COLLATE Chinese_PRC_CI_AS  NOT NULL,
                      [Result] varchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
                      CONSTRAINT [PK__TestModelInt__Id] PRIMARY KEY CLUSTERED ([Id])
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
                    ON [PRIMARY]
                    ) 

                    IF EXISTS(Select 1 From Sysobjects Where Name='TestModelMultipeKey')
                    DROP table TestModelMultipeKey

                    CREATE TABLE [dbo].[TestModelMultipeKey] (
                      [TypeId] varchar(255) COLLATE Chinese_PRC_CI_AS  NOT NULL,
                      [Type] varchar(255) COLLATE Chinese_PRC_CI_AS  NOT NULL,
                      [MethodName] varchar(255) COLLATE Chinese_PRC_CI_AS  NOT NULL,
                      [Result] varchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
                      CONSTRAINT [PK__TestModelMultipeKey__Id] PRIMARY KEY CLUSTERED ([TypeId], [Type])
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
                    ON [PRIMARY]
                    )";
            return sql;
        }
    }

    public class MySqlTestBase
    {
        public MySqlTestBase()
        {
            Connection = new MySqlConnection("server=localhost;database=demo;uid=root;password=root");
            CreateTableSql = GetCreateTableSql();
        }
        public IDbConnection Connection { get; }
        public string CreateTableSql { get; }
        private string GetCreateTableSql()
        {
            var sql = @"
                    drop table if exists TestModelInt;
                    CREATE TABLE `TestModelInt` (
                      `Id` int NOT NULL AUTO_INCREMENT,
                      `MethodName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
                      `Result` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
                      PRIMARY KEY (`Id`) USING BTREE
                    ) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

                    drop table if exists TestModelMultipeKey;
                    CREATE TABLE `TestModelMultipeKey` (
                      `TypeId` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
                      `Type` varchar(255) NOT NULL,
                      `MethodName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
                      `Result` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
                      PRIMARY KEY (`TypeId`,`Type`) USING BTREE
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;";
            return sql;
        }
    }

    public class SqlLiteTestBase
    {
        public SqlLiteTestBase()
        {
            if (!File.Exists("Test.sqlite"))
                SQLiteConnection.CreateFile("Test.sqlite");

            Connection = new SQLiteConnection("Data Source=Test.sqlite;Version=3;");
            CreateTableSql = GetCreateTableSql();
        }
        public IDbConnection Connection { get; }
        public string CreateTableSql { get; }
        private string GetCreateTableSql()
        {
            var sql = @"
            drop table if exists 'TestModelInt';
            CREATE TABLE 'TestModelInt' (
                'Id' integer NOT NULL,
                'MethodName' TEXT,
                'Result' TEXT,
                PRIMARY KEY('Id')
            );
            drop table if exists 'TestModelMultipeKey';
            CREATE TABLE 'TestModelMultipeKey'(
              'TypeId' TEXT NOT NULL,
              'Type' TEXT NOT NULL,
              'MethodName' TEXT,
              'Result' TEXT,
              PRIMARY KEY('TypeId', 'Type')
            );";
            return sql;
        }
    }
}
