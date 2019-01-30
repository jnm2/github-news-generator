using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitHubNewsGenerator
{
    public static class JsonReaderExtensions
    {
        public static JsonReaderException CreateException(this JsonReader positionInfo, string message)
        {
            var lineInfo = positionInfo as IJsonLineInfo;

            return new JsonReaderException(
                message,
                positionInfo?.Path,
                lineInfo?.LineNumber ?? 0,
                lineInfo?.LinePosition ?? 0,
                innerException: null);
        }

        public static async ValueTask ReadStartArrayAsync(this JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (!await reader.ReadAsync().ConfigureAwait(false) || reader.TokenType != JsonToken.StartArray)
                throw reader.CreateException("Expected start of array.");
        }

        public static async ValueTask ReadStartObjectAsync(this JsonReader reader, CancellationToken cancellationToken)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false) || reader.TokenType != JsonToken.StartObject)
                throw reader.CreateException("Expected start of object.");
        }

        public static async ValueTask<bool> ReadStartObjectUnlessEndArrayAsync(this JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        return true;
                    case JsonToken.EndArray:
                        return false;
                }
            }

            throw reader.CreateException("Expected start of object or end of array.");
        }

        public static async ValueTask<string> ReadPropertyNameUnlessEndClassAsync(this JsonReader reader, CancellationToken cancellationToken)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        if (reader.Value is string propertyName)
                            return propertyName;
                        break;
                    case JsonToken.EndObject:
                        return null;
                }
            }

            throw reader.CreateException("Expected property name.");
        }

        public static async ValueTask<string> ReadStringValueAsync(this JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return await reader.ReadAsStringAsync().ConfigureAwait(false)
                   ?? throw reader.CreateException("Expected string value.");
        }

        public static async ValueTask<int> ReadInt32ValueAsync(this JsonReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return await reader.ReadAsInt32Async().ConfigureAwait(false)
                   ?? throw reader.CreateException("Expected integer value.");
        }
    }
}
