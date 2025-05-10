using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data
{
    public class AppMigration
    {
        private readonly AppDbContext _dbContext;

        public AppMigration(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ApplyCustomMigrations()
        {
            // Check and create the Customers table if it doesn't exist
            if (!await TableExists("Customers"))
            {
                var migrationBuilder = new MigrationBuilder("SqlServer");

                migrationBuilder.CreateTable(
                    name: "Customers",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        FirstName = table.Column<string>(nullable: false),
                        LastName = table.Column<string>(nullable: false),
                        Email = table.Column<string>(nullable: true),
                        PasswordHash = table.Column<byte[]>(nullable: true),
                        PasswordSalt = table.Column<byte[]>(nullable: true),
                        CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Customers", x => x.Id);
                    }
                );

                // Apply the generated SQL manually
                foreach (var command in migrationBuilder.Operations)
                {
                    var sqlGenerator = _dbContext.GetService<IMigrationsSqlGenerator>();
                    var commands = sqlGenerator.Generate(new[] { command }, _dbContext.Model);
                    foreach (var cmd in commands)
                    {
                        await _dbContext.Database.ExecuteSqlRawAsync(cmd.CommandText);
                    }
                }
            }

            // Add PasswordHash column if it doesn't exist
            await AddColumnIfNotExist("Customers", "PasswordHash", table =>
            {
                table.AddColumn<byte[]>(
                    name: "PasswordHash",
                    table: "Customers",
                    nullable: true
                );
            });

            // Add PasswordSalt column if it doesn't exist
            await AddColumnIfNotExist("Customers", "PasswordSalt", table =>
            {
                table.AddColumn<byte[]>(
                    name: "PasswordSalt",
                    table: "Customers",
                    nullable: true
                );
            });
        }

        #region Utilities

        private async Task AddColumnIfNotExist(string tableName, string columnName, Action<MigrationBuilder> addColumnAction)
        {
            if (!await ColumnExists(tableName, columnName))
            {
                var migrationBuilder = new MigrationBuilder("SqlServer");
                addColumnAction(migrationBuilder);

                foreach (var command in migrationBuilder.Operations)
                {
                    var sqlGenerator = _dbContext.GetService<IMigrationsSqlGenerator>();
                    var commands = sqlGenerator.Generate(new[] { command }, _dbContext.Model);
                    foreach (var cmd in commands)
                    {
                        await _dbContext.Database.ExecuteSqlRawAsync(cmd.CommandText);
                    }
                }
            }
        }

        private async Task<bool> TableExists(string tableName)
        {
            var conn = _dbContext.Database.GetDbConnection();

            await using var command = conn.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";

            var param = command.CreateParameter();
            param.ParameterName = "@tableName";
            param.Value = tableName;
            command.Parameters.Add(param);

            if (conn.State == System.Data.ConnectionState.Closed)
                await conn.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        private async Task<bool> ColumnExists(string tableName, string columnName)
        {
            var conn = _dbContext.Database.GetDbConnection();

            await using var command = conn.CreateCommand();
            command.CommandText = @"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName";

            var tableParam = command.CreateParameter();
            tableParam.ParameterName = "@tableName";
            tableParam.Value = tableName;
            command.Parameters.Add(tableParam);

            var columnParam = command.CreateParameter();
            columnParam.ParameterName = "@columnName";
            columnParam.Value = columnName;
            command.Parameters.Add(columnParam);

            if (conn.State == System.Data.ConnectionState.Closed)
                await conn.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        #endregion
    }
}
