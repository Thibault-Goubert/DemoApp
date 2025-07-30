using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace DemoApp.Functions.Functions;

public class UserEndpoints
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;
    public UserEndpoints(IUserRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequestData req)
    {
        var user = await req.ReadFromJsonAsync<User>();
        await _repo.AddAsync(user);
        var r = req.CreateResponse(HttpStatusCode.Created);
        await r.WriteAsJsonAsync(user);
        return r;
    }

    [Function("GetUsers")]
    public async Task<HttpResponseData> GetAll(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequestData req)
    {
        var users = await _repo.GetAllAsync();
        var r = req.CreateResponse(HttpStatusCode.OK);
        await r.WriteAsJsonAsync(users);
        return r;
    }

    [Function("TestConnection")]
    public async Task<HttpResponseData> TestDbConnection(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var connectionString = _config.GetConnectionString("DefaultConnection");
        
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Connexion à la base de données réussie !");
            return response;
        }
        catch (SqlException ex)
        {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"Échec de connexion : {ex.Message}");
            return response;
        }
    }
}
