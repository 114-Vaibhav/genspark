using Npgsql;

namespace backend.Data
{
    public class PostgresSqlRunner
    {
        private readonly string _connectionString;

        public PostgresSqlRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> QueryAsync<T>(string sql, Func<NpgsqlDataReader, T> map, params NpgsqlParameter[] parameters)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = CreateCommand(connection, sql, null, parameters);
            await using var reader = await command.ExecuteReaderAsync();

            var results = new List<T>();

            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, Func<NpgsqlDataReader, T> map, params NpgsqlParameter[] parameters)
        {
            var results = await QueryAsync(sql, map, parameters);
            return results.FirstOrDefault();
        }

        public async Task<int> ExecuteAsync(string sql, params NpgsqlParameter[] parameters)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = CreateCommand(connection, sql, null, parameters);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string sql, params NpgsqlParameter[] parameters)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = CreateCommand(connection, sql, null, parameters);
            return await command.ExecuteScalarAsync();
        }

        public async Task<T> WithTransactionAsync<T>(Func<NpgsqlConnection, NpgsqlTransaction, Task<T>> operation)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await operation(connection, transaction);
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public NpgsqlCommand CreateCommand(
            NpgsqlConnection connection,
            string sql,
            NpgsqlTransaction? transaction = null,
            params NpgsqlParameter[] parameters)
        {
            var command = new NpgsqlCommand(sql, connection, transaction);

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            return command;
        }
    }
}
