﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Swaggatherer
{
    internal static class Template
    {
        public static string Execute(IReadOnlyList<RouteEntry> entries)
        {
            var setupEndpointsLines = new List<string>();
            for (var i = 0; i < entries.Count; i++)
           {
                var entry = entries[i];
                var httpMethodText = entry.Method == null ? string.Empty : $", \"{entry.Method.ToUpperInvariant()}\"";
                setupEndpointsLines.Add($"            _endpoints[{i}] = CreateEndpoint(\"{entry.Template.TemplateText}\"{httpMethodText});");
            }

            var setupRequestsLines = new List<string>();
            for (var i = 0; i < entries.Count; i++)
            {
                setupRequestsLines.Add($"            _requests[{i}] = new DefaultHttpContext();");
                setupRequestsLines.Add($"            _requests[{i}].RequestServices = CreateServices();");
                setupRequestsLines.Add($"            _requests[{i}].Request.Method = \"{entries[i].Method.ToUpperInvariant()}\";");
                setupRequestsLines.Add($"            _requests[{i}].Request.Path = \"{entries[i].RequestUrl}\";");
            }

            var setupMatcherLines = new List<string>();
            for (var i = 0; i < entries.Count; i++)
            {
                setupMatcherLines.Add($"            builder.AddEndpoint(_endpoints[{i}]);");
            }

            return string.Format(@"
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Matching
{{
    // This code was generated by the Swaggatherer
    public partial class GeneratedBenchmark : MatcherBenchmarkBase
    {{
        private const int EndpointCount = {3};

        private void SetupEndpoints()
        {{
            _endpoints = new MatcherEndpoint[{3}];
{0}
        }}

        private void SetupRequests()
        {{
            _requests = new HttpContext[{3}];
{1}
        }}

        private Matcher SetupMatcher(MatcherBuilder builder)
        {{
{2}
            return builder.Build();
        }}
    }}
}}", 
    string.Join(Environment.NewLine, setupEndpointsLines),
    string.Join(Environment.NewLine, setupRequestsLines),
    string.Join(Environment.NewLine, setupMatcherLines),
    entries.Count);
        }
    }
}
