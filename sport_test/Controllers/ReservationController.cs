using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sport_test.Models;
using sport_test.Responses;
using sport_test.Services;

namespace sport_test.Controllers;

public class ReservationController : Controller
{
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;
    private readonly SportSpaceService _spaceService;

    public ReservationController(ReservationService reservationService, UserService userService, SportSpaceService spaceService)
    {
        _reservationService = reservationService;
        _userService = userService;
        _spaceService = spaceService;
    }

    public IActionResult Index(int? userId, int? spaceId)
    {
        ServiceResponse<IEnumerable<Reservation>> response;

        if (userId.HasValue)
            response = _reservationService.GetReservationsByUser(userId.Value);
        else if (spaceId.HasValue)
            response = _reservationService.GetReservationsBySpace(spaceId.Value);
        else
            response = _reservationService.GetAllReservations();

        ViewBag.Users = new SelectList(_userService.GetAllUsers().Data, "Id", "Name");
        ViewBag.Spaces = new SelectList(_spaceService.GetAllSpaces().Data, "Id", "Name");
        ViewBag.SelectedUserId = userId;
        ViewBag.SelectedSpaceId = spaceId;
        return View(response.Data);
    }

    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public IActionResult Store(Reservation reservation)
    {
        var result = _reservationService.SaveReservation(reservation);
        TempData["message"] = result.Message;
        TempData["success"] = result.Success.ToString();
        if (result.Success)
            return RedirectToAction("Index");
        PopulateDropdowns();
        return RedirectToAction("Create");
    }

    public IActionResult Show(int id)
    {
        var result = _reservationService.GetReservationById(id);
        if (!result.Success)
        {
            TempData["message"] = result.Message;
            return RedirectToAction("Index");
        }
        return View(result.Data);
    }

    public IActionResult Edit(int id)
    {
        var result = _reservationService.GetReservationById(id);
        if (!result.Success)
        {
            TempData["message"] = result.Message;
            return RedirectToAction("Index");
        }
        PopulateDropdowns();
        return View(result.Data);
    }

    [HttpPost]
    public IActionResult Update(Reservation reservation)
    {
        var result = _reservationService.UpdateReservation(reservation);
        TempData["message"] = result.Message;
        TempData["success"] = result.Success.ToString();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Cancel(int id)
    {
        var result = _reservationService.CancelReservation(id);
        TempData["message"] = result.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Destroy(Reservation reservation)
    {
        var result = _reservationService.DeleteReservation(reservation);
        TempData["message"] = result.Message;
        return RedirectToAction("Index");
    }

    private void PopulateDropdowns()
    {
        ViewBag.Users = new SelectList(_userService.GetAllUsers().Data, "Id", "Name");
        ViewBag.Spaces = new SelectList(_spaceService.GetAllSpaces().Data, "Id", "Name");
        ViewBag.Statuses = new SelectList(new[] { "Activa", "Cancelada", "Finalizada" });
    }
}
