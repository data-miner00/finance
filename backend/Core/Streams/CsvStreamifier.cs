using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;

namespace Core.Streams
{
    /// <summary>
    /// The converter from object to Csv stream.
    /// </summary>
    public class CsvStreamifier : IDataStreamifier
    {
        public async Task<Stream> StreamifyAsync<T>(T @object, CancellationToken cancellationToken) where T : class
        {
            ArgumentNullException.ThrowIfNull(@object);

            var stream = new MemoryStream();

            // leaveOpen so the underlying MemoryStream survives writer/csv disposal
            var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, leaveOpen: true);

            await using (writer)
            await using (csv)
            {
                // string is IEnumerable<char>, so explicitly exclude it
                if (@object is IEnumerable enumerable && @object is not string)
                {
                    await csv.WriteRecordsAsync(enumerable, cancellationToken);
                }
                else
                {
                    // wrap the single object so the same WriteRecordsAsync path
                    // writes the header + one row
                    await csv.WriteRecordsAsync(new[] { @object }, cancellationToken);
                }
            }

            stream.Position = 0;
            return stream;
        }
    }
}
