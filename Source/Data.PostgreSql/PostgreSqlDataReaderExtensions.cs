using System;
using System.Threading.Tasks;

using Junior.Common;

using Npgsql;

namespace Junior.Persist.Data.PostgreSql
{
	/// <summary>
	/// Extensions for the <see cref="NpgsqlDataReader"/> type.
	/// </summary>
	public static class NpgsqlDataReaderExtensions
	{
		/// <summary>
		/// Gets a value as type <typeparamref name="T"/> from the specified data reader.
		/// </summary>
		/// <param name="reader">A data reader.</param>
		/// <param name="column">A column name.</param>
		/// <typeparam name="T">The data type.</typeparam>
		/// <returns>The column value.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reader"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is null.</exception>
		public static async Task<T> GetValue<T>(this NpgsqlDataReader reader, string column)
		{
			reader.ThrowIfNull("reader");
			column.ThrowIfNull("column");

			return await Task.Run(() =>
				{
					int ordinal = reader.GetOrdinal(column);
					Type type = typeof(T);

					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && reader.IsDBNull(ordinal))
					{
						return default(T);
					}

					return (T)reader.GetValue(ordinal);
				});
		}

		/// <summary>
		/// Gets a binary GUID from the specified data reader.
		/// </summary>
		/// <param name="reader">A data reader.</param>
		/// <param name="column">A column name.</param>
		/// <returns>A precise date-time</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reader"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is null.</exception>
		public static async Task<BinaryGuid> GetBinaryGuid(this NpgsqlDataReader reader, string column)
		{
			reader.ThrowIfNull("reader");
			column.ThrowIfNull("column");

			return await Task.Run(() =>
				{
					int ordinal = reader.GetOrdinal(column);
					var buffer = new byte[16];

					reader.GetBytes(ordinal, 0, buffer, 0, 16);

					return new BinaryGuid(buffer);
				});
		}

		/// <summary>
		/// Gets a precise date-time from the specified data reader.
		/// </summary>
		/// <param name="reader">A data reader.</param>
		/// <param name="column">A column name.</param>
		/// <returns>A precise date-time</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reader"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is null.</exception>
		public static async Task<PreciseDateTime> GetPreciseDateTime(this NpgsqlDataReader reader, string column)
		{
			reader.ThrowIfNull("reader");
			column.ThrowIfNull("column");

			return await Task.Run(() =>
				{
					int ordinal = reader.GetOrdinal(column);

					return new PreciseDateTime(reader.GetInt64(ordinal));
				});
		}
	}
}