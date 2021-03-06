﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace URSA.Web.Http.Mapping
{
    /// <summary>Instructs the pipeline to map the method to a given HTTP verb.</summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "No testable logic.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnVerbAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="OnVerbAttribute" /> class.</summary>
        /// <param name="verb">Target HTTP verb.</param>
        public OnVerbAttribute(Verb verb)
        {
            if (verb == null)
            {
                throw new ArgumentNullException("verb");
            }

            Verb = verb;
        }

        /// <summary>Gets the HTTP verb.</summary>
        public Verb Verb { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("HTTP {0}", Verb);
        }
    }
}