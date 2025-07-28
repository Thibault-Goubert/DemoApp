using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace DemoApp.Functions.Functions;

public class UserEndpoints
{
    private readonly IUserRepository _repo;
    public UserEndpoints(IUserRepository repo) => _repo = repo;

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
}
