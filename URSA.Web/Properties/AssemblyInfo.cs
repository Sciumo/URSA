﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using URSA.Web;

[assembly: AssemblyTitle("URSA.Web")]
[assembly: AssemblyDescription("URSA ASP.net hookup library.")]
[assembly: ComVisible(false)]
[assembly: Guid("cce00167-f623-4c22-9057-c1072530a6f2")]
[assembly: PreApplicationStartMethod(typeof(HttpApplicationExtesions), "SetupModule")]