using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace FamilyTreeProject.Data.CosmosDB.Tests
{
    public abstract class CosmosDBTestBase
    {
        private IConfigurationRoot _configuration;

        private string CosmosDB_Endpoint => _configuration["CosmosDB:Endpoint"];
        private string CosmosDB_Key => _configuration["CosmosDB:Key"];
        protected string CosmosDB_DatabaseId => _configuration["CosmosDB:DatabaseId"];
        protected string CosmosDB_CollectionId => _configuration["CosmosDB:CollectionId"];
        
        [OneTimeSetUp]
        public void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json");

            _configuration = builder.Build();
        }

        protected DocumentClient GetClient()
        {
            return new DocumentClient(new Uri(CosmosDB_Endpoint), CosmosDB_Key);
        }
    }
}