﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using URSA.Web;

namespace URSA.Example.WebApplication.Data
{
    /// <summary>Describes a person.</summary>
    public class Person : IControlledEntity<Guid>
    {
        private string _firstName = String.Empty;
        private string _lastName = String.Empty;
        private IList<string> _favouriteDishes = new List<string>();
        private ICollection<string> _roles = new List<string>();
        private ICollection<Person> _friends = new List<Person>();
        private IList<Person> _superiors = new List<Person>();

        /// <summary>Gets or sets the person's identifier.</summary>
        [Key]
        public Guid Key { get; set; }

        /// <summary> Gets or sets the person's firstname.</summary>
        public string Firstname { get { return _firstName; } set { _firstName = value ?? String.Empty; } }

        /// <summary> Gets or sets the person's lastname.</summary>
        public string Lastname { get { return _lastName; } set { _lastName = value ?? String.Empty; } }

        /// <summary> Gets or sets the person's roles.</summary>
        public ICollection<string> Roles { get { return _roles; } set { _roles = value ?? new List<string>(); } }

        /// <summary>Gets or sets the favourite dishes.</summary>
        public IList<string> FavouriteDishes { get { return _favouriteDishes; } set { _favouriteDishes = value ?? new List<string>(); } }

        /// <summary>Gets or sets the spouse.</summary>
        public Person Spouse { get; set; }

        /// <summary>Gets or sets the friends. </summary>
        public ICollection<Person> Friends { get { return _friends; } set { _friends = value ?? new List<Person>(); } }

        /// <summary>Gets or sets the superiors.</summary>
        public IList<Person> Superiors { get { return _superiors; } set { _superiors = value ?? new List<Person>(); } }
    }
}