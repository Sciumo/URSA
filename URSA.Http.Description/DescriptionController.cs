﻿using RomanticWeb.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using URSA.Web.Http.Converters;
using URSA.Web.Http.Description.Entities;
using URSA.Web.Http.Description.Hydra;
using URSA.Web.Http.Mapping;
using URSA.Web.Mapping;

namespace URSA.Web.Http.Description
{
    /// <summary>Defines possible documentation output formats.</summary>
    public enum OutputFormats
    {
        /// <summary>Turtle format.</summary>
        Turtle,
        
        /// <summary>RDF/XML format.</summary>
        Rdf,

        /// <summary>OWL/XML format.</summary>
        Owl,

        /// <summary>XML alias for RDF/XML format.</summary>
        Xml,

        /// <summary>JSON-LD format.</summary>
        JsonLd
    }

    /// <summary>Describes an abstract description controller.</summary>
    public abstract class DescriptionController : IController
    {
        /// <summary>Defines an URSA vocabulary base URI.</summary>
        public static readonly Uri VocabularyBaseUri = new Uri("http://alien-mcl.github.io/URSA/vocabulary#");

        /// <summary>Defines a '<![CDATA[application/xml]]>' media type.</summary>
        private const string ApplicationXml = "application/xml";

        private readonly IEntityContextProvider _entityContextProvider;
        private readonly IApiDescriptionBuilder _apiDescriptionBuilder;

        /// <summary>Initializes a new instance of the <see cref="DescriptionController" /> class.</summary>
        /// <param name="entityContextProvider">Entity context provider.</param>
        /// <param name="apiDescriptionBuilder">API description builder.</param>
        protected DescriptionController(IEntityContextProvider entityContextProvider, IApiDescriptionBuilder apiDescriptionBuilder)
        {
            if (entityContextProvider == null)
            {
                throw new ArgumentNullException("entityContextProvider");
            }

            if (apiDescriptionBuilder == null)
            {
                throw new ArgumentNullException("apiDescriptionBuilder");
            }

            _entityContextProvider = entityContextProvider;
            _apiDescriptionBuilder = apiDescriptionBuilder;
        }

        /// <inheritdoc />
        public IResponseInfo Response { get; set; }

        /// <summary>Gets the name of the file.</summary>
        protected internal abstract string FileName { get; }

        /// <summary>Gets the API description builder.</summary>
        protected IApiDescriptionBuilder ApiDescriptionBuilder { get { return _apiDescriptionBuilder; } }

        /// <summary>Gets the entity context provider.</summary>
        protected IEntityContextProvider EntityContextProvider { get { return _entityContextProvider; } }

        /// <summary>Gets the API documentation.</summary>
        /// <param name="format">Optional output format.</param>
        /// <returns><see cref="IApiDocumentation" /> instance.</returns>
        [Route("/")]
        [OnGet]
        public IApiDocumentation GetApiEntryPointDescription(OutputFormats format)
        {
            return GetApiEntryPointDescription(OverrideAcceptedMediaType(format));
        }

        /// <summary>Gets the API documentation.</summary>
        /// <returns><see cref="IApiDocumentation" /> instance.</returns>
        [Route("/")]
        [OnOptions]
        public IApiDocumentation GetApiEntryPointDescription()
        {
            return GetApiEntryPointDescription(OverrideAcceptedMediaType(null));
        }

        private IApiDocumentation GetApiEntryPointDescription(string fileExtension)
        {
            return BuildApiDocumentation(fileExtension, (Uri)((HttpUrl)Response.Request.Url).WithFragment(String.Empty));
        }

        private IApiDocumentation BuildApiDocumentation(string fileExtension, Uri entityId)
        {
            ((ResponseInfo)Response).Headers.ContentDisposition = String.Format("inline; filename=\"{0}.{1}\"", FileName, fileExtension);
            IApiDocumentation result = _entityContextProvider.EntityContext.Create<IApiDocumentation>(new EntityId(entityId));
            _apiDescriptionBuilder.BuildDescription(result, this.GetRequestedMediaTypeProfiles());
            _entityContextProvider.EntityContext.Commit();
            return result;
        }

        //// TODO: Check the default file name is actually a TXT!
        private string OverrideAcceptedMediaType(OutputFormats? format)
        {
            if (format != null)
            {
                switch ((OutputFormats)format)
                {
                    case OutputFormats.JsonLd:
                        return EntityConverter.MediaTypeFileFormats[Response.Request.Headers[Header.Accept] = EntityConverter.ApplicationLdJson];
                    case OutputFormats.Turtle:
                        return EntityConverter.MediaTypeFileFormats[Response.Request.Headers[Header.Accept] = EntityConverter.TextTurtle];
                    case OutputFormats.Xml:
                        Response.Headers[Header.ContentType] = ApplicationXml;
                        return EntityConverter.MediaTypeFileFormats[Response.Request.Headers[Header.Accept] = EntityConverter.ApplicationRdfXml];
                    case OutputFormats.Rdf:
                        return EntityConverter.MediaTypeFileFormats[Response.Request.Headers[Header.Accept] = EntityConverter.ApplicationRdfXml];
                    case OutputFormats.Owl:
                        return EntityConverter.MediaTypeFileFormats[Response.Request.Headers[Header.Accept] = EntityConverter.ApplicationRdfXml];
                }
            }

            var accept = Response.Request.Headers[Header.Accept];
            var fileExtension = "txt";
            if ((accept == null) || (!accept.Contains("*/*")))
            {
                return fileExtension;
            }

            var resultingMediaType = ((RequestInfo)Response.Request).Headers[Header.Accept].Values
                .Join(EntityConverter.MediaTypes, outer => outer.Value, inner => inner, (outer, inner) => inner)
                .FirstOrDefault();
            return ((resultingMediaType == null) || (!EntityConverter.MediaTypeFileFormats.ContainsKey(resultingMediaType)) ? "txt" : EntityConverter.MediaTypeFileFormats[resultingMediaType]);
        }
    }

    /// <summary>Provides a ReST description facility.</summary>
    /// <typeparam name="T">Type of the controller to generate documentation for.</typeparam>
    [DependentRoute]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Suppression is OK - generic and non-generic class.")]
    public class DescriptionController<T> : DescriptionController where T : IController
    {
        /// <summary>Initializes a new instance of the <see cref="DescriptionController{T}" /> class.</summary>
        /// <param name="entityContextProvider">Entity context provider.</param>
        /// <param name="apiDescriptionBuilder">API description builder.</param>
        public DescriptionController(IEntityContextProvider entityContextProvider, IApiDescriptionBuilder<T> apiDescriptionBuilder)
            : base(entityContextProvider, apiDescriptionBuilder)
        {
        }

        /// <inheritdoc />
        protected internal override string FileName { get { return typeof(T).Name; } }
    }
}