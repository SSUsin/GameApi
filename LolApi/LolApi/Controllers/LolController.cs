using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LolApi.Models;
using LolApi.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;

namespace LolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LolController : ControllerBase
    {
        private readonly LolApiContext _context;
        private IConfiguration _configuration;

        public LolController(LolApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Lol
        /// <summary>
        /// To show a list of champions
        /// </summary>
        [HttpGet]
        public IEnumerable<LolItem> GetLolItem()
        {
            return _context.LolItem;
        }

        // get lol champ by ID
        // GET: api/Lol/5
        /// <summary>
        /// To get a champion by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLolItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lolItem = await _context.LolItem.FindAsync(id);

            if (lolItem == null)
            {
                return NotFound();
            }

            return Ok(lolItem);
        }

        // update champ details by ID
        // PUT: api/Lol/5
        /// <summary>
        /// Update a champion information by ID
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLolItem([FromRoute] int id, [FromBody] LolItem lolItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lolItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(lolItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LolItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Create a champ
        // POST: api/Lol
        /// <summary>
        /// Create a new champion detail.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostLolItem([FromBody] LolItem lolItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LolItem.Add(lolItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLolItem", new { id = lolItem.Id }, lolItem);
        }

        // Delete a champ by ID
        // DELETE: api/Lol/5
        /// <summary>
        /// Delete a champion by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLolItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lolItem = await _context.LolItem.FindAsync(id);
            if (lolItem == null)
            {
                return NotFound();
            }

            _context.LolItem.Remove(lolItem);
            await _context.SaveChangesAsync();

            return Ok(lolItem);
        }

        private bool LolItemExists(int id)
        {
            return _context.LolItem.Any(e => e.Id == id);
        }

        // Get champ by tags
        // GET: api/Lol/Tags
        /// <summary>
        /// Get champion's skins by tags
        /// </summary>
        [Route("tags")]
        [HttpGet]
        public async Task<List<string>> GetTags()
        {
            var lolchamps = (from m in _context.LolItem
                         select m.Tags).Distinct();

            var returned = await lolchamps.ToListAsync();

            return returned;
        }

        //Search champ by tags
        // GET: api/Lol/Tags
        /// <summary>
        /// Search champion's skins by tags
        /// </summary>
        [HttpGet]
        [Route("SearchByTag")]
        public async Task<List<LolItem>> GetTagsItem([FromQuery] string tags)
        {
            var memes = from m in _context.LolItem
                        select m; //get all the memes


            if (!String.IsNullOrEmpty(tags)) //make sure user gave a tag to search
            {
                memes = memes.Where(s => s.Tags.ToLower().Equals(tags.ToLower())); // find the entries with the search tag and reassign
            }

            var returned = await memes.ToListAsync(); //return the memes

            return returned;
        }

        //Upload skin/image of champs 
        /// <summary>
        /// Upload a new skin for a champion.
        /// </summary>
        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm]LolImageItem meme)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = meme.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(meme.Image.FileName, null, stream);
                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    LolItem lolItem = new LolItem();
                    lolItem.ChampionName = meme.ChampionName;
                    lolItem.Tags = meme.Tags;

                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                   // lolItem.Height = image.Height.ToString();
                  //  lolItem.Width = image.Width.ToString();
                    lolItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    lolItem.Uploaded = DateTime.Now.ToString();

                    _context.LolItem.Add(lolItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {meme.ChampionName} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("images");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

       
       

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }


    }
}