namespace Firmeza.Web.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;

[Microsoft.AspNetCore.Components.Route("[controller]/[action]")]

public class ExcelController :Controller
{
    private readonly IExcelService _excelService;

    public ExcelController(IExcelService excelService)
    {
        _excelService = excelService;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Por favor selecciona un archivo Excel válido.";
            return RedirectToAction("Upload");
        }

        var success = await _excelService.ProcessExcelAsync(file);

        TempData["Message"] = success
            ? "Datos cargados correctamente a la base de datos."
            : "Ocurrió un error al procesar el archivo.";

        return RedirectToAction("Upload");
    }
}
