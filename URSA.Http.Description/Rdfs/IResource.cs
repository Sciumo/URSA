﻿using RDeF.Entities;
using RDeF.Mapping.Attributes;

namespace URSA.Web.Http.Description.Rdfs
{
    /// <summary>Represents an resource.</summary>
    [Class("rdfs", "Resource")]
    public interface IResource : IEntity
    {
        /// <summary>Gets or sets the label.</summary>
        [Property("rdfs", "label")]
        string Label { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [Property("rdfs", "comment")]
        string Description { get; set; }
    }
}