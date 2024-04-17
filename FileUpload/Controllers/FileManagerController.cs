using FileUpload.Models;
using FileUpload.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IManageImage _iManageImage;
        public FileManagerController(IManageImage iManageImage)
        {
            _iManageImage = iManageImage;
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _iManageImage.UploadFile(model.File);
                    return Ok(result);
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(string.Join(", ", errors));
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return the exception message in the response body
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading the file: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("downloadfile")]
        public async Task<IActionResult> DownloadFile(string FileName)
        {
            try
            {
                // Check if current time is within allowed time period (e.g., 8:00 AM to 2:00 PM)
                var currentTime = DateTime.Now.TimeOfDay;
                var allowedStartTime = new TimeSpan(8, 0, 0);
                var allowedEndTime = new TimeSpan(23, 59, 0);

                if (currentTime < allowedStartTime || currentTime > allowedEndTime)
                {
                    return StatusCode(403, "File download is only allowed between 8:00 AM and 2:00 PM.");
                }

                if (string.IsNullOrEmpty(FileName))
                {
                    return BadRequest("File name cannot be empty.");
                }

                var result = await _iManageImage.DownloadFile(FileName);
                return File(result.Item1, result.Item2, result.Item3);
            }
            catch (FileNotFoundException)
            {
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the file: {ex.Message}");
            }
        }
    }
}
