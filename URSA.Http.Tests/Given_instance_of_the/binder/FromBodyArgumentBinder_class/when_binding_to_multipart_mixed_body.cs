﻿using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;
using URSA;
using URSA.Web;
using URSA.Web.Http;
using URSA.Web.Http.Mapping;
using URSA.Web.Http.Testing;
using URSA.Web.Mapping;

namespace Given_instance_of_the.binder.FromBodyArgumentBinder_class
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class when_binding_to_multipart_mixed_body : ArgumentBinderTest<FromBodyArgumentBinder, FromBodyAttribute, int>
    {
        private const string Boundary = "test";
        private const string Body =
            "--" + Boundary + "\r\nContent-Type: text/plain\r\nContent-Length:3\r\n\r\n1\r\n" +
            "--" + Boundary + "\r\nContent-Type: text/plain\r\nContent-Length:3\r\n\r\n2\r\n--" + Boundary + "--";

        protected override HttpUrl RequestUrl { get { return (HttpUrl)UrlParser.Parse("http://temp.org/api/test/modulo"); } }

        protected override HttpUrl MethodUrl { get { return (HttpUrl)UrlParser.Parse("http://temp.org/api/test/modulo"); } }

        protected override string MethodName { get { return "PostModulo"; } }

        [Test]
        public void it_should_call_converter_provider()
        {
            Binder.GetArgumentValue(GetContext(Body, "POST", "multipart/mixed", Boundary));

            ConverterProvider.Verify(instance => instance.FindBestInputConverter(It.IsAny<Type>(), It.IsAny<IRequestInfo>(), false), Times.Once);
        }

        [Test]
        public void it_should_call_converter()
        {
            Binder.GetArgumentValue((ArgumentBindingContext)GetContext(Body, "POST", "multipart/mixed", Boundary));

            Converter.Verify(instance => instance.ConvertTo(It.IsAny<Type>(), It.IsAny<IRequestInfo>()), Times.Once);
        }
    }
}