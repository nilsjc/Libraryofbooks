using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebAPI.Endpoints
{
    public static class Library
    {
        public static void MapLibraryEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "Welcome to the Library Management API!");
        }
    }
}