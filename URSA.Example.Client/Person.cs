//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a URSA HTTP client proxy generation tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Dynamic;
using URSA.Web.Http;

namespace URSA.Example.WebApplication.Data
{
    [System.CodeDom.Compiler.GeneratedCode("URSA HTTP client proxy generation tool", "1.0")]
    public interface IPerson : RomanticWeb.Entities.IEntity
    {
        System.Collections.Generic.ICollection<String> Roles { get; set; }

        URSA.Example.WebApplication.Data.Person Spouse { get; set; }

        System.Collections.Generic.IList<String> FavouriteDishes { get; set; }

        System.Collections.Generic.IList<Person> Superiors { get; set; }

        System.String Firstname { get; set; }

        System.Collections.Generic.ICollection<Person> Friends { get; set; }

        System.Guid Key { get; set; }

        System.String Lastname { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCode("URSA HTTP client proxy generation tool", "1.0")]
    public partial class Person : IPerson
    {
        private RomanticWeb.Entities.EntityId _id = new RomanticWeb.Entities.EntityId(new System.Uri("urn:guid:" + System.Guid.NewGuid()));

        RomanticWeb.IEntityContext RomanticWeb.Entities.IEntity.Context { get { return null; } }

        RomanticWeb.Entities.EntityId RomanticWeb.Entities.IEntity.Id { get { return _id; } }

        public System.Collections.Generic.ICollection<String> Roles { get; set; }

        public URSA.Example.WebApplication.Data.Person Spouse { get; set; }

        public System.Collections.Generic.IList<String> FavouriteDishes { get; set; }

        public System.Collections.Generic.IList<Person> Superiors { get; set; }

        public System.String Firstname { get; set; }

        public System.Collections.Generic.ICollection<Person> Friends { get; set; }

        public System.Guid Key { get; set; }

        public System.String Lastname { get; set; }
    }
}