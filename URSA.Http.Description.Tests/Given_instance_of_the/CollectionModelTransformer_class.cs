﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;
using URSA;
using URSA.Security;
using URSA.Web;
using URSA.Web.Http;
using URSA.Web.Http.Description;
using URSA.Web.Http.Description.Hydra;
using URSA.Web.Http.Description.Tests;
using URSA.Web.Http.Description.Tests.Data;
using URSA.Web.Http.Tests.Testing;

namespace Given_instance_of_the
{
    [TestFixture]
    public class CollectionModelTransformer_class
    {
        private static readonly HttpUrl RequestUrl = (HttpUrl)UrlParser.Parse("http://temp.uri/");
        private static readonly RequestInfo Request = new RequestInfo(Verb.GET, RequestUrl, new MemoryStream(), new BasicClaimBasedIdentity(), new Header("Accept", "*/*"));

        private Mock<IRequestMapping> _mapping;
        private Mock<IEntityContext> _entityContext;
        private IResponseModelTransformer _responseModelTransformer;

        [Test]
        public async Task should_inject_hydra_Collection_details()
        {
            var result = new List<IProduct>() { new Mock<MulticastObject>(MockBehavior.Strict).Object.ActLike<IProduct>() };
            var arguments = new[] { 1, 0, 0, (object)null };
            var collection = SetupCollection(result.Count);
            _entityContext.Setup(instance => instance.Load<ICollection>((Uri)RequestUrl)).Returns(collection.Object);

            await _responseModelTransformer.Transform(_mapping.Object, Request, result, arguments);

            _entityContext.Verify(instance => instance.Load<ICollection>((Uri)RequestUrl), Times.Once);
            collection.Object.Members.Should().HaveCount(result.Count);
        }

        [Test]
        public async Task should_inject_hydra_PartialCollectionView_details()
        {
            var result = Enumerable.Range(0, 20).Select(index => new Mock<MulticastObject>(MockBehavior.Strict).Object.ActLike<IProduct>()).ToList();
            var arguments = new[] { 20, 0, 10, (object)null };
            var view = new Mock<IPartialCollectionView>(MockBehavior.Strict);
            var collection = SetupCollection(result.Count, 10, view);
            _entityContext.Setup(instance => instance.Load<ICollection>((Uri)RequestUrl)).Returns(collection.Object);
            _entityContext.Setup(instance => instance.Load<IPartialCollectionView>(It.IsAny<Iri>())).Returns(view.Object);

            await _responseModelTransformer.Transform(_mapping.Object, Request, result, arguments);

            _entityContext.Verify(instance => instance.Load<ICollection>((Uri)RequestUrl), Times.Once);
            collection.Object.Members.Should().HaveCount(result.Count);
        }

        [SetUp]
        public void Setup()
        {
            var argumentValueSources = new Dictionary<int, ArgumentValueSources>()
                {
                    { 0, ArgumentValueSources.Neutral },
                    { 1, ArgumentValueSources.Bound },
                    { 2, ArgumentValueSources.Bound }
                };
            _mapping = new Mock<IRequestMapping>(MockBehavior.Strict);
            _mapping.SetupGet(instance => instance.Operation).Returns(typeof(TestController).GetTypeInfo().GetMethod("List").ToOperationInfo("/", Verb.GET));
            _mapping.SetupGet(instance => instance.Target).Returns(new TestController());
            _mapping.SetupGet(instance => instance.ArgumentSources).Returns(argumentValueSources);
            _entityContext = new Mock<IEntityContext>(MockBehavior.Strict);
            _entityContext.Setup(instance => instance.Commit());
            _responseModelTransformer = new CollectionResponseModelTransformer(_entityContext.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _responseModelTransformer = null;
            _mapping = null;
            _entityContext = null;
        }

        private Mock<ICollection> SetupCollection(int totalItems, int take = 0, Mock<IPartialCollectionView> view = null)
        {
            var collection = new Mock<ICollection>(MockBehavior.Strict);
            collection.SetupGet(instance => instance.Iri).Returns(new Iri((Uri)RequestUrl));
            collection.SetupSet(instance => instance.TotalItems = totalItems);
            var members = new List<IResource>();
            collection.SetupGet(instance => instance.Members).Returns(members);
            if (view == null)
            {
                return collection;
            }

            collection.SetupSet(instance => instance.View = view.Object);
            collection.SetupGet(instance => instance.Context).Returns(_entityContext.Object);
            view.SetupSet(instance => instance.ItemsPerPage = take);
            return collection;
        }
    }
}