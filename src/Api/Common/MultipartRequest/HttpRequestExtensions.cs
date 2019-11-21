using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using MimeTypes.Core;

namespace Api.Common.MultipartRequest
{
    public static class HttpRequestExtensions
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        public static async Task<FormValueProvider> FetchFormData(
            this HttpRequest request,
            Stream[] targetStreams,
            CancellationToken cancellationToken)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                throw new Exception($"Expected a multipart request, but got {request.ContentType}");
            }

            // Used to accumulate all the form url encoded key value pairs in the request.
            var formAccumulator = new KeyValueAccumulator();

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType), DefaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            var filesCount = 0;

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        if (filesCount < targetStreams.Length)
                        {
                            await section.Body.CopyToAsync(targetStreams[filesCount], cancellationToken);

                            var fieldName = contentDisposition.Name;
                            formAccumulator.Append($"contentType_{fieldName}", MimeTypeMap.GetMimeType(Path.GetExtension(contentDisposition.FileName.Value)));
                            formAccumulator.Append($"fileName_{fieldName}", contentDisposition.FileName.Value);

                            filesCount++;
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = MultipartRequestHelper.GetEncoding(section);

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            true,
                            1024,
                            true))
                        {
                            var value = await streamReader.ReadToEndAsync();

                            if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key.Value, value);

                            if (formAccumulator.ValueCount > DefaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {DefaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            foreach (var targetStream in targetStreams)
            {
                // Set position to 0 so stream is able to read
                targetStream.Seek(0, SeekOrigin.Begin);
            }

            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            return formValueProvider;
        }
    }
}