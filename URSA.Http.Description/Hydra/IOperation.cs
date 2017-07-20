﻿using System.Collections.Generic;
using RDeF.Mapping.Attributes;

namespace URSA.Web.Http.Description.Hydra
{
    /// <summary>Describes an operation.</summary>
    [Class("hydra", "Operation")]
    public interface IOperation : IResource
    {
        /// <summary>Gets the HTTP methods.</summary>
        [Collection("hydra", "method")]
        ICollection<string> Method { get; }

        /// <summary>Gets classes expected in request body.</summary>
        [Collection("hydra", "expects")]
        ICollection<IClass> Expects { get; }

        /// <summary>Gets the classes returned in response body.</summary>
        [Collection("hydra", "returns")]
        ICollection<IClass> Returns { get; }

        /// <summary>Gets the HTTP status codes.</summary>
        [Collection("hydra", "statusCode")]
        ICollection<int> StatusCodes { get; }
    }
}