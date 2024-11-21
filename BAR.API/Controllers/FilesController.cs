using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Bar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _imagePath;

        // Внедрение IWebHostEnvironment для получения пути к папке wwwroot
        public FilesController(IWebHostEnvironment webHost)
        {
            _imagePath = Path.Combine(webHost.WebRootPath, "Images");
        }

        // Метод для сохранения файла
        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("No file uploaded.");
            }

            // Создаем путь для сохранения файла
            var filePath = Path.Combine(_imagePath, file.FileName);
            var fileInfo = new FileInfo(filePath);

            // Если файл существует, удаляем его
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            // Сохраняем файл в указанной папке
            using (var fileStream = fileInfo.Create())
            {
                await file.CopyToAsync(fileStream);
            }

            // Формируем URL для доступа к файлу
            var host = HttpContext.Request.Host;
            var fileUrl = $"https://{host}/Images/{file.FileName}";
            return Ok(fileUrl);
        }

        // Метод для удаления файла
        [HttpDelete]
        public IActionResult DeleteFile(string fileName)
        {
            var filePath = Path.Combine(_imagePath, fileName);
            var fileInfo = new FileInfo(filePath);

            // Проверяем существует ли файл
            if (!fileInfo.Exists)
            {
                return NotFound("File not found.");
            }

            // Удаляем файл
            fileInfo.Delete();
            return Ok("File deleted successfully.");
        }
    }
}
