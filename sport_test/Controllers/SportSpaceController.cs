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
        var response = string.IsNullOrEmpty(type)
            ? _spaceService.GetAllSpaces()
            : _spaceService.GetSpacesByType(type);

        ViewBag.SelectedType = type;
        ViewBag.Types = _spaceService.GetSpaceTypes();
        return View(response.Data);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Store(SportSpace space)
    {
        var result = _spaceService.SaveSpace(space);
        TempData["message"] = result.Message;
        TempData["success"] = result.Success.ToString();
        if (result.Success)
            return RedirectToAction("Index");
        return RedirectToAction("Create");
    }

    public IActionResult Show(int id)
    {
        var result = _spaceService.GetSpaceById(id);
        if (!result.Success)
        {
            TempData["message"] = result.Message;
            return RedirectToAction("Index");
        }
        return View(result.Data);
    }

    public IActionResult Edit(int id)
    {
        var result = _spaceService.GetSpaceById(id);
        if (!result.Success)
        {
            TempData["message"] = result.Message;
            return RedirectToAction("Index");
        }
        return View(result.Data);
    }

    [HttpPost]
    public IActionResult Update(SportSpace space)
    {
        var result = _spaceService.UpdateSpace(space);
        TempData["message"] = result.Message;
        TempData["success"] = result.Success.ToString();
        if (result.Success)
            return RedirectToAction("Index");
        return RedirectToAction("Edit", new { id = space.Id });
    }

    [HttpPost]
    public IActionResult Destroy(SportSpace space)
    {
        var result = _spaceService.DeleteSpace(space);
        TempData["message"] = result.Message;
        return RedirectToAction("Index");
    }
}
