using Microsoft.AspNetCore.Mvc;
using Microsoft.Office365.Discovery;
using Microsoft.Graph;
using hackatontest.Helper;
using System;
using System.Collections.Generic;
using RestSharp;
using System.Threading.Tasks;

namespace hackatontest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController: ControllerBase
    {

        // [HttpGet]
        // public async Task<ActionResult<string>> Get()
        // {
        //     var client = new RestClient("https://graph.microsoft.com");
        //     var request = new RestRequest("/v1.0/me/Events", Method.GET);
        //     var token =  await TokenFactory.Create();

        //     request.AddHeader("Authorization", "Bearer " +token);
        //     request.AddHeader("Content-Type", "application/json");
        //     request.AddHeader("Accept", "application/json");
            
        //     var response = client.Execute(request);
        //     var content = response.Content;
        //     return new JsonResult(content);
        // }

        [HttpGet]
          public async Task<ActionResult<string>> Get()
        {
            var results = new List<Event>();

            var client = ClientFactory.Create();
            var events = await client.Me.Events.Request()
            .Select(x => new {name = x.Subject})
            .GetAsync();
            
            results.AddRange(events.CurrentPage);
            

            return new JsonResult(results);
        }
        
    }
}