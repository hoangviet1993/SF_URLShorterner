using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace URLShortenerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        // GET api/links/abc123
        [HttpGet("APIReady")]
        public IActionResult Get()
        {
            return Content("API is Ready");
        }
        // GET api/links/abc123
        [HttpGet("{hashID}")]
        [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string hashID)
        {
            try
            {
                Models.ShortLink hashIDItem = await HashIDDBRepository.GetHashIDItemAsync(hashID);
                return Redirect(hashIDItem.Url);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Models.ErrorResponse notFoundResponse = new Models.ErrorResponse
                {
                    Details = "Not found."
                };
                return NotFound(notFoundResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateHashIDPostedJson([FromBody] Models.UrlString longUrl)
        {
            if (URLValidator.IsUrlValid(longUrl.url))
            {
                Models.ShortLink hashIDItem = await HashIDDBRepository.CreateHashIDItemIfNotExistsAsync(longUrl.url);
                Models.PublicShortLink response = new Models.PublicShortLink
                {
                    Url = hashIDItem.Url,
                    HashID = hashIDItem.HashID,
                    CreatedTime = hashIDItem.CreatedTime
                };
                return Ok(response);
            }
            else
            {
                Models.ErrorResponse invalidURLResponse = new Models.ErrorResponse
                {
                    Details = "Enter a valid URL."
                };
                return BadRequest(invalidURLResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> CreateHashIDPostedForm([FromForm] Models.UrlString longUrl)
        {
            if (URLValidator.IsUrlValid(longUrl.url))
            {
                Models.ShortLink hashIDItem = await HashIDDBRepository.CreateHashIDItemIfNotExistsAsync(longUrl.url);
                Models.PublicShortLink response = new Models.PublicShortLink
                {
                    Url = hashIDItem.Url,
                    HashID = hashIDItem.HashID,
                    CreatedTime = hashIDItem.CreatedTime
                };
                return Ok(response);
            }
            else
            {
                Models.ErrorResponse errorResponse = new Models.ErrorResponse
                {
                    Details = "Enter a valid URL."
                };
                return BadRequest(errorResponse);
            }
        }
    }
}