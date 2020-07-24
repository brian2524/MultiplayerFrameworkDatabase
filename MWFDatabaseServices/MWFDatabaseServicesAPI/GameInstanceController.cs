using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MWFDataLibrary.BuisnessLogic;
using System.Text.Json;
using MWFModelsLibrary.Models;

namespace MWFDatabaseServicesAPI
{
    public static class GameInstanceController
    {
        [FunctionName("CreateGameInstanceAndReturnId")]
        public static async Task<IActionResult> CreateGameInstanceAndReturnId(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("processing CreateGameInstanceAndReturnId endpoint");

            // get the body in json format (there may be a better way to do this)
            string requestBody = await req.ReadAsStringAsync();
            JsonElement jsonBody = JsonSerializer.Deserialize<JsonElement>(requestBody);
            /*GameInstanceModel gameInstance = JsonSerializer.Deserialize<GameInstanceModel>(requestBody);*/

            // get all of the values we need for the GameInstanceProcessor (maybe deserialize json to a GameInstanceModel instead? Or maybe just pass a GameInstanceModel with a null Id to this endpoint)
            int reqGame = jsonBody.GetProperty("Game").GetInt32();
            string reqArgs = jsonBody.GetProperty("Args").GetString();
            string reqAssociatedHost = jsonBody.GetProperty("AssociatedHost").GetString();

            // hard coded connection string for now
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MultiplayerWebFrameworkDB;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            // call on the data processor and store the returned Id
            int retVal = GameInstanceProcessor.CreateGameInstanceAndReturnId(connectionString, reqGame, reqArgs, reqAssociatedHost);

            return new OkObjectResult(retVal);
        }

        [FunctionName("DeleteGameInstanceById")]
        public static async Task<IActionResult> DeleteGameInstanceById(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("gameInstanceID = " /*+ gameinstanceID.ToString()*/);


            // get the body in json format (there may be a better way to do this)
            string requestBody = await req.ReadAsStringAsync();
            int gameInstanceId = int.Parse(requestBody);

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MultiplayerWebFrameworkDB;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            int rowsDeleted = GameInstanceProcessor.DeleteGameInstanceById(connectionString, gameInstanceId);

            // Passing an int into the OkObjectResult will put the int in the body
            return new OkObjectResult(rowsDeleted);
        }
    }
}