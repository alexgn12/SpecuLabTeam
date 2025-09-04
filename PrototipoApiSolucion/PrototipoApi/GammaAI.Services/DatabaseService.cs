using Microsoft.EntityFrameworkCore;
using GammaAI.Core.Interfaces;
using GammaAI.Data.Context;
using GammaAI.Data.Seed;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace GammaAI.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly GammaDbContext _context;

        public DatabaseService(GammaDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetDatabaseSchemaAsync()
        {
            var schema = new StringBuilder();
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        t.TABLE_NAME,
                        c.COLUMN_NAME,
                        c.DATA_TYPE,
                        c.IS_NULLABLE,
                        c.CHARACTER_MAXIMUM_LENGTH,
                        tc.CONSTRAINT_TYPE
                    FROM INFORMATION_SCHEMA.TABLES t
                    LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
                    LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON c.TABLE_NAME = kcu.TABLE_NAME AND c.COLUMN_NAME = kcu.COLUMN_NAME
                    LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                    WHERE t.TABLE_TYPE = 'BASE TABLE' 
                    AND t.TABLE_NAME NOT LIKE '__EF%'
                    ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION";

                using var reader = await command.ExecuteReaderAsync();
                string currentTable = "";
                schema.AppendLine("=== ESQUEMA DE LA BASE DE DATOS ===");
                while (await reader.ReadAsync())
                {
                    string tableName = reader.GetString("TABLE_NAME");
                    string columnName = reader.GetString("COLUMN_NAME");
                    string dataType = reader.GetString("DATA_TYPE");
                    string isNullable = reader.GetString("IS_NULLABLE");
                    var maxLength = reader.IsDBNull("CHARACTER_MAXIMUM_LENGTH") ? (int?)null : reader.GetInt32("CHARACTER_MAXIMUM_LENGTH");
                    var constraintType = reader.IsDBNull("CONSTRAINT_TYPE") ? null : reader.GetString("CONSTRAINT_TYPE");

                    if (currentTable != tableName)
                    {
                        if (!string.IsNullOrEmpty(currentTable))
                            schema.AppendLine();
                        schema.AppendLine($"TABLA: {tableName}");
                        currentTable = tableName;
                    }

                    var columnInfo = $"  - {columnName} ({dataType}";
                    if (maxLength.HasValue)
                        columnInfo += $"({maxLength})";
                    columnInfo += isNullable == "YES" ? ", nullable" : ", not null";
                    if (constraintType == "PRIMARY KEY")
                        columnInfo += ", PRIMARY KEY";
                    else if (constraintType == "FOREIGN KEY")
                        columnInfo += ", FOREIGN KEY";
                    columnInfo += ")";
                    schema.AppendLine(columnInfo);
                }
                await connection.CloseAsync();
                schema.AppendLine("\n=== RELACIONES ===");
                schema.AppendLine("Products.CategoryId -> Categories.Id (FK)");
                return schema.ToString();
            }
            catch (Exception ex)
            {
                return $"Error obteniendo esquema: {ex.Message}";
            }
        }

        public async Task<string> ExecuteQueryAsync(string sqlQuery)
        {
            try
            {
                if (!await ValidateQueryAsync(sqlQuery))
                {
                    return "ERROR: Consulta no válida o no permitida";
                }
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = sqlQuery;
                using var reader = await command.ExecuteReaderAsync();
                var results = new StringBuilder();
                var columnNames = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNames.Add(reader.GetName(i));
                }
                if (reader.HasRows)
                {
                    results.AppendLine("RESULTADOS:");
                    results.AppendLine(string.Join(" | ", columnNames));
                    results.AppendLine(new string('-', columnNames.Sum(c => c.Length) + (columnNames.Count - 1) * 3));
                    int rowCount = 0;
                    while (await reader.ReadAsync() && rowCount < 100)
                    {
                        var row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString();
                            row.Add(value ?? "");
                        }
                        results.AppendLine(string.Join(" | ", row));
                        rowCount++;
                    }
                    if (rowCount == 100)
                        results.AppendLine("... (mostrando solo las primeras 100 filas)");
                    results.AppendLine($"\nTotal de filas mostradas: {rowCount}");
                }
                else
                {
                    results.AppendLine("No se encontraron resultados.");
                }
                await connection.CloseAsync();
                return results.ToString();
            }
            catch (Exception ex)
            {
                return $"ERROR ejecutando consulta: {ex.Message}";
            }
        }

        public async Task<bool> ValidateQueryAsync(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                return false;
            string query = sqlQuery.ToLower().Trim();
            if (!query.StartsWith("select"))
                return false;
            var prohibitedCommands = new[]
            {
                "insert", "update", "delete", "drop", "create", "alter", 
                "truncate", "exec", "execute", "sp_", "xp_", "--", "/*", "*/"
            };
            foreach (var command in prohibitedCommands)
            {
                if (Regex.IsMatch(query, $@"\\b{command}\\b"))
                    return false;
            }
            return await Task.FromResult(true);
        }

        public async Task SeedDatabaseAsync()
        {
            await DatabaseSeeder.SeedAsync(_context);
        }
    }
}
