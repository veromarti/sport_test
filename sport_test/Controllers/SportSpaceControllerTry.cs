using Microsoft.AspNetCore.Mvc;
using sport_test.Models;
using sport_test.Services;

namespace sport_test.Controllers;

public class SportSpaceController : Controller
{
    private readonly SportSpaceService _spaceService;

    public SportSpaceController(SportSpaceService spaceService) => _spaceService = spaceService;

    public IActionResult Index(string? type)
    {
        try
        {
            var response = string.IsNullOrEmpty(type)
                ? _spaceService.GetAllSpaces()
                : _spaceService.GetSpacesByType(type);

            ViewBag.SelectedType = type;
            ViewBag.Types = _spaceService.GetSpaceTypes();
            return View(response.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error al cargar espacios deportivos: {ex.Message}";
            TempData["success"] = "False";
            ViewBag.SelectedType = type;
            ViewBag.Types = new List<string>();
            return View(Enumerable.Empty<SportSpace>());
        }
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Store(SportSpace space)
    {
        try
        {
            var result = _spaceService.SaveSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al registrar espacio: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Create");
        }
    }

    public IActionResult Show(int id)
    {
        try
        {
            var result = _spaceService.GetSpaceById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error al obtener espacio deportivo: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var result = _spaceService.GetSpaceById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error al cargar espacio deportivo: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Update(SportSpace space)
    {
        try
        {
            var result = _spaceService.UpdateSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = space.Id });
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al actualizar espacio: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Edit", new { id = space.Id });
        }
    }

    [HttpPost]
    public IActionResult Destroy(SportSpace space)
    {
        try
        {
            var result = _spaceService.DeleteSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al eliminar espacio: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }
}
