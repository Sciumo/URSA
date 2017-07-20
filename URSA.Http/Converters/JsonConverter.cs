﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using URSA.Web.Converters;

namespace URSA.Web.Http.Converters
{
    /// <summary>Converts JSON messages into objects</summary>
    public class JsonConverter : IConverter
    {
        /// <summary>Defines an <![CDATA[application/json]]> media type.</summary>
        public const string ApplicationJson = "application/json";

        private static readonly string[] MediaTypes = { ApplicationJson };

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "No testable logic.")]
        public IEnumerable<string> SupportedMediaTypes { get { return MediaTypes; } }

        /// <inheritdoc />
        public CompatibilityLevel CanConvertTo<T>(IRequestInfo request)
        {
            return CanConvertTo(typeof(T), request);
        }

        /// <inheritdoc />
        public CompatibilityLevel CanConvertTo(Type expectedType, IRequestInfo request)
        {
            if (expectedType == null)
            {
                throw new ArgumentNullException("expectedType");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var requestInfo = (RequestInfo)request;
            var result = CompatibilityLevel.TypeMatch;
            var contentType = requestInfo.Headers[Header.ContentType];
            if ((contentType != null) && (contentType.Values.Any(value => value.Value == ApplicationJson)))
            {
                result |= CompatibilityLevel.ExactProtocolMatch;
            }

            return result;
        }

        /// <inheritdoc />
        public T ConvertTo<T>(IRequestInfo request)
        {
            object result = ConvertTo(typeof(T), request);
            return (result == null ? default(T) : (T)result);
        }

        /// <inheritdoc />
        public object ConvertTo(Type expectedType, IRequestInfo request)
        {
            if (expectedType == null)
            {
                throw new ArgumentNullException("expectedType");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if ((request.Body.CanSeek) && (request.Body.Length == 0))
            {
                return (expectedType.GetTypeInfo().IsValueType ? Activator.CreateInstance(expectedType) : null);
            }

            using (var reader = new StreamReader(request.Body))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new JsonSerializer().Deserialize(jsonReader, expectedType);
            }
        }

        /// <inheritdoc />
        public T ConvertTo<T>(string body)
        {
            return (T)ConvertTo(typeof(T), body);
        }

        /// <inheritdoc />
        public object ConvertTo(Type expectedType, string body)
        {
            if (expectedType == null)
            {
                throw new ArgumentNullException("expectedType");
            }

            return (body == null ?
                (expectedType.GetTypeInfo().IsValueType ? Activator.CreateInstance(expectedType) : null) :
                JsonConvert.DeserializeObject(body, expectedType));
        }

        /// <inheritdoc />
        public CompatibilityLevel CanConvertFrom<T>(IResponseInfo response)
        {
            return CanConvertFrom(typeof(T), response);
        }

        /// <inheritdoc />
        public CompatibilityLevel CanConvertFrom(Type givenType, IResponseInfo response)
        {
            if (givenType == null)
            {
                throw new ArgumentNullException("givenType");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var result = CompatibilityLevel.TypeMatch;
            var responseInfo = (ResponseInfo)response;
            var accept = responseInfo.Request.Headers[Header.Accept];
            if ((accept != null) && (accept.Values.Any(value => value == ApplicationJson)))
            {
                result |= CompatibilityLevel.ExactProtocolMatch;
            }

            return result;
        }

        /// <inheritdoc />
        public void ConvertFrom<T>(T instance, IResponseInfo response)
        {
            ConvertFrom(typeof(T), instance, response);
        }

        /// <inheritdoc />
        public void ConvertFrom(Type givenType, object instance, IResponseInfo response)
        {
            if (givenType == null)
            {
                throw new ArgumentNullException("givenType");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var responseInfo = (ResponseInfo)response;
            responseInfo.Headers.ContentType = ApplicationJson;
            if ((instance != null) && (!givenType.IsInstanceOfType(instance)))
            {
                throw new InvalidOperationException(String.Format("Instance type '{0}' mismatch from the given '{1}'.", instance.GetType(), givenType));
            }

            using (var writer = new StreamWriter(responseInfo.Body))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                new JsonSerializer().Serialize(jsonWriter, instance);
                jsonWriter.Flush();
            }
        }
    }
}