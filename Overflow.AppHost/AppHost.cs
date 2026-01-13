using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithDataVolume("keycloak-data")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin");

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithPgAdmin();

var questionDb = postgres.AddDatabase("questionDb");

var questionService = builder.AddProject<QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WaitFor(keycloak)
    .WaitFor(questionDb);

builder.Build().Run();
