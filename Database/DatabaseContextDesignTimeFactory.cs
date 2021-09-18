﻿using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json.Linq;
using System;

namespace Average.Server.Database
{
    public class DatabaseContextDesignTimeFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            return new DatabaseContext("Server=127.0.0.1;Port=3306;Database=average;Uid=root;Pwd=;");
        }
    }
}
