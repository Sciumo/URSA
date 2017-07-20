﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using URSA.Web.Description;
using URSA.Web.Description.Http;
using URSA.Web.Http;
using URSA.Web.Mapping;
using URSA.Web.Tests;

namespace Given_instance_of_the.ControllerDescriptionBuilder_class
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class when_having_dependent_route_controller
    {
        private ControllerDescriptionBuilder<AnotherTestController<TestController>> _builder;

        [Test]
        public void it_should_describe_Some_method_correctly()
        {
            var method = typeof(AnotherTestController<TestController>).GetMethod("Some");
            var details = _builder.BuildDescriptor().Operations.Cast<OperationInfo<Verb>>().FirstOrDefault(operation => operation.UnderlyingMethod == method);

            details.Should().NotBeNull();
            details.ProtocolSpecificCommand.Should().Be(Verb.GET);
            details.UrlTemplate.Should().Be("/api/test");
            details.TemplateRegex.ToString().Should().Be("^/api/test$");
            details.Url.ToString().Should().Be("/api/test");
            details.Arguments.Should().HaveCount(method.GetParameters().Length);
        }

        [SetUp]
        public void Setup()
        {
            Mock<IDefaultValueRelationSelector> defaultSourceSelector = new Mock<IDefaultValueRelationSelector>(MockBehavior.Strict);
            defaultSourceSelector.Setup(instance => instance.ProvideDefault(It.IsAny<ParameterInfo>(), It.IsAny<Verb>()))
                .Returns<ParameterInfo, Verb>((parameter, verb) => FromQueryStringAttribute.For(parameter));
            defaultSourceSelector.Setup(instance => instance.ProvideDefault(It.IsAny<ParameterInfo>()))
                .Returns<ParameterInfo>(parameter => new ToBodyAttribute());
            _builder = new ControllerDescriptionBuilder<AnotherTestController<TestController>>(defaultSourceSelector.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _builder = null;
        }
    }
}