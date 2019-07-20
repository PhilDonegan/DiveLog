using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace DiveLog.API.Helpers
{
    public class GzipRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ContentEncodingHeader = "Content-Encoding";
        private const string ContentEncodingGzip = "gzip";
        private const string ContentEncodingDeflate = "deflate";

        public GzipRequestMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(ContentEncodingHeader) 
                && (context.Request.Headers[ContentEncodingHeader] == ContentEncodingGzip 
                || context.Request.Headers[ContentEncodingHeader] == ContentEncodingDeflate))
            {
                var contentEncoding = context.Request.Headers[ContentEncodingHeader];
                var decompressor = contentEncoding == ContentEncodingGzip ? (Stream)new GZipStream(context.Request.Body, CompressionMode.Decompress, true) : (Stream)new DeflateStream(context.Request.Body, CompressionMode.Decompress, true);
                context.Request.Body = decompressor;
            }

            await _next(context);
        }
    }
}
