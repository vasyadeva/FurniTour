﻿using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api;
using FurniTour.Server.Models.Profile;
using GroqApiLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Nodes;

namespace FurniTour.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {

        private readonly IProfileService profileService;
        private readonly IConfiguration configuration;

        public ProfileController(IProfileService profileService, IConfiguration configuration)
        {
            this.profileService = profileService;
            this.configuration = configuration;
        }

        [HttpGet("getmaster/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMaster(string id)
        {
            var profile = await profileService.GetMasterProfile(id);
            return Ok(profile);
        }

        [HttpPost("addmaster")]
        [Authorize]
        public IActionResult AddMaster([FromBody] AddMasterReview model)
        {
            var state = profileService.MakeMasterReview(model);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpGet("getmanufacturer/{name}")]
        [Authorize]
        public async Task<IActionResult> GetManufacturer(string name)
        {
            var profile = await profileService.GetManufacturerProfile(name);
            return Ok(profile);
        }

        [HttpPost("addmanufacturer")]
        [Authorize]
        public IActionResult AddManufacturer([FromBody] AddManufacturerReview model)
        {
            var state = profileService.MakeManufacturerReview(model);
            if (state.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest(state);
        }

        [HttpGet("ai/master/review/{id}")]
        public async Task<IActionResult> GetMasterReview(string id)
        {
            var review = await profileService.GetMasterProfile(id);
            if (review.Reviews.Count < 1)
            {
                return Ok(new AIReviewModel { review = string.Empty });
            }
            string reviewstext = "";
            foreach (var rev in review.Reviews)
            {
                reviewstext += rev.Comment;
            }
            var api = configuration["key:api"];
            var groqApi = new GroqApiClient(api);

            var request = new JsonObject
            {
                ["model"] = "gemma2-9b-it",
                ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = $"Summarize the reviews without for master only in Ukraininan language {id}: {reviewstext}"
                }
            }
            };

            var result = await groqApi.CreateChatCompletionAsync(request);
            var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            if (aiResponse != null)
            {
                aiResponse = aiResponse.Replace("\"", "")
                                       .Replace("\n", " ")
                                       .Replace("\r", " ")
                                       .Trim();
            }

            return Ok(new AIReviewModel { review = aiResponse });
        }

        [HttpGet("ai/manufacturer/review/{id}")]
        public async Task<IActionResult> GetManufacturerReview(string id)
        {
            var review = await profileService.GetManufacturerProfile(id);
            if (review.Reviews.Count < 1)
            {
                return Ok(new AIReviewModel { review = string.Empty });
            }
            string reviewstext = "";
            foreach (var rev in review.Reviews)
            {
                reviewstext += "," + rev.Comment;
            }
            var api = configuration["key:api"];
            var groqApi = new GroqApiClient(api);

            var request = new JsonObject
            {
                ["model"] = "meta-llama/llama-4-maverick-17b-128e-instruct",
                ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = $"Summarize the reviews without for manufacturer from 3-rd person only in Ukraininan language {id}: {reviewstext}"
                }
            }
            };

            var result = await groqApi.CreateChatCompletionAsync(request);
            var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            if (aiResponse != null)
            {
                aiResponse = aiResponse.Replace("\"", "")
                                       .Replace("\n", " ")
                                       .Replace("\r", " ")
                                       .Trim();
            }

            return Ok(new AIReviewModel { review = aiResponse });
        }


        [HttpGet("ai/master/search2/{description}/{category}/{pricePolicy}")]
        public async Task<IActionResult> GetMasterByDescription(string description, int category, int pricePolicy)
        {
            var profile = await profileService.GetMasterByDescription(description, category, pricePolicy);
            var serialized = System.Text.Json.JsonSerializer.Serialize(profile);
            return Content(serialized, "text/plain");
        }

        [HttpGet("public/{username}")]
        [Authorize]
        public async Task<IActionResult> GetPublicProfile(string username)
        {
            var profile = await profileService.GetPublicProfile(username);
            if (profile == null)
            {
                return NotFound($"User with username '{username}' not found");
            }
            return Ok(profile);
        }
    }
}

