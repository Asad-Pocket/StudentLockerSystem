using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentLockerSystem.Models;

namespace StudentLockerSystem.Controllers
{
    [Authorize]
    public class GallaryController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly string wwwrootDirectory;

        public GallaryController(UserManager<Student> userManager, IHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            wwwrootDirectory = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot");
        }
        public async Task<IActionResult> FileManager()
        {
            var student = await _userManager.FindByNameAsync(User.Identity.Name);

            if (student != null)
            {
                var studentFolder = Path.Combine(wwwrootDirectory, student.FirstName);
                var files = new DirectoryInfo(studentFolder).GetFiles().ToList();
                return View(files);
            }

            return View(new List<FileInfo>());
        }
        [HttpPost]
        public async Task<IActionResult> FileManager([FromForm] IFormFile myFile)
        {
            if (myFile != null && myFile.Length > 0)
            {
                var student = await _userManager.GetUserAsync(User);

                if (student != null)
                {
                    var studentFolder = Path.Combine(wwwrootDirectory, student.FirstName);

                    if (!Directory.Exists(studentFolder))
                    {
                        Directory.CreateDirectory(studentFolder);
                    }

                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(myFile.FileName);
                    var path = Path.Combine(studentFolder, fileName);

                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await myFile.CopyToAsync(stream);
                        }

                        return RedirectToAction("FileManager");
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception (log, display an error message, etc.)
                        ModelState.AddModelError("", "An error occurred while uploading the file.");
                    }
                }
            }

            // If the file upload fails, return to the view
            return View();
        }
        public IActionResult Rename(string currentFileName, string newFileName)
        {
            try
            {
                var student = _userManager.GetUserAsync(User).Result;
                var folderPath = student.FirstName;
                var currentFilePath = Path.Combine("wwwroot", folderPath, currentFileName);
                var newFilePath = Path.Combine("wwwroot", folderPath, newFileName);

                if (System.IO.File.Exists(currentFilePath))
                {
                    System.IO.File.Move(currentFilePath, newFilePath);
                }
                else
                {
                    // Handle the case where the source file doesn't exist
                    // You might want to display an error message or log the issue
                }

                return RedirectToAction("FileManager");
            }
            catch (Exception ex)
            {
                // Handle errors, e.g., file not found, permission issues, etc.
                // You can log the exception or display an error message to the user
                return RedirectToAction("FileManager");
            }
        }



        public IActionResult Download(string filename)
        {
            if (filename != null)
            {
                var student = _userManager.GetUserAsync(User).Result;
                var filePath = Path.Combine(wwwrootDirectory, student.FirstName, filename);

                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", filename);
                }
            }

            // If the file doesn't exist or there's an issue, you might want to handle it accordingly.
            return RedirectToAction("FileManager");
        }

        public IActionResult Delete(string filename)
        {
            // Implement your file deletion logic here
            var student = _userManager.GetUserAsync(User).Result;
            var filePath = Path.Combine(wwwrootDirectory, student.FirstName, filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return RedirectToAction("FileManager");
            }
            else
            {
                // Handle file not found or other errors
                return RedirectToAction("FileManager");
            }
        }

        public async Task<IActionResult> Photos()
        {
            var student = await _userManager.GetUserAsync(User);

            if (student != null)
            {
                var studentFolder = Path.Combine(wwwrootDirectory, student.FirstName);
                var photos = new DirectoryInfo(studentFolder).GetFiles("*.jpg").ToList();
                ViewData["F_Path"] = studentFolder;
                return View(photos);
            }

            return View(new List<FileInfo>());
        }

        public IActionResult PreviewImage(string filename)
        {
            var student = _userManager.GetUserAsync(User).Result;
            var filePath = Path.Combine(wwwrootDirectory, student.FirstName, filename);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/*");
        }
        public IActionResult PreviewPdf(string filename)
        {
            var student = _userManager.GetUserAsync(User).Result;
            var filePath = Path.Combine(wwwrootDirectory, student.FirstName, filename);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "Pdf/*");
        }
    }
}
