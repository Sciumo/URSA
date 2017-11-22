﻿#pragma warning disable 1591
using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using RDeF.Mapping;
using RDeF.Vocabularies;
using RollerCaster;
using URSA.CodeGen;
using URSA.Web.Http.Description.CodeGen;
using URSA.Web.Http.Description.Hydra;

namespace Given_instance_of_the.HydraClassGenerator_class
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class in_simple_name_and_namespace_scenario
    {
        private const string Name = "Class";
        private const string Namespace = "Namespace";

        private Mock<IUriParser> _uriParser;
        private Mock<IResource> _resource;
        private IClassGenerator _generator;

        [Test]
        public void it_should_extract_namespace_from_uri()
        {
            var @namespace = Namespace;
            var result = _generator.CreateNamespace(_resource.Object);

            result.Should().Be(Namespace);
            _uriParser.Verify(instance => instance.IsApplicable(It.IsAny<Uri>()), Times.Once);
            _uriParser.Verify(instance => instance.Parse(It.IsAny<Uri>(), out @namespace), Times.Once);
        }

        [Test]
        public void it_should_extract_name_from_uri()
        {
            var @namespace = Namespace;
            var result = _generator.CreateName(_resource.Object);

            result.Should().Be(Name);
            _uriParser.Verify(instance => instance.IsApplicable(It.IsAny<Uri>()), Times.Once);
            _uriParser.Verify(instance => instance.Parse(It.IsAny<Uri>(), out @namespace), Times.Once);
        }

        [SetUp]
        public void Setup()
        {
            var uri = new Iri(new Uri("some:uri"));
            var @namespace = Namespace;
            _uriParser = new Mock<IUriParser>(MockBehavior.Strict);
            _uriParser.Setup(instance => instance.IsApplicable(It.IsAny<Uri>())).Returns(UriParserCompatibility.ExactMatch);
            _uriParser.Setup(instance => instance.Parse(It.IsAny<Uri>(), out @namespace)).Returns<Uri, string>((id, ns) => Name);
            var classMapping = new Mock<IStatementMapping>(MockBehavior.Strict);
            classMapping.SetupGet(instance => instance.Term).Returns(rdfs.Class);
            var rdfsClassMapping = new Mock<IEntityMapping>(MockBehavior.Strict);
            rdfsClassMapping.SetupGet(instance => instance.Classes).Returns(new[] { classMapping.Object });
            var mappings = new Mock<IMappingsRepository>(MockBehavior.Strict);
            mappings.Setup(instance => instance.FindEntityMappingFor(It.IsAny<URSA.Web.Http.Description.Rdfs.IClass>())).Returns(rdfsClassMapping.Object);
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.SetupGet(instance => instance.Mappings).Returns(mappings.Object);
            var entityMock = new Mock<MulticastObject>(MockBehavior.Strict);
            _resource = entityMock.As<IResource>();
            _resource.SetupGet(instance => instance.Iri).Returns(uri);
            entityMock.SetupGet(instance => instance.CastedTypes).Returns(Type.EmptyTypes);
            _resource.As<IEntity>().SetupGet(instance => instance.Context).Returns(context.Object);
            _generator = new HydraClassGenerator(new IUriParser[] { _uriParser.Object });
        }

        [TearDown]
        public void Teardown()
        {
            _generator = null;
            _uriParser = null;
            _resource = null;
        }
    }
}