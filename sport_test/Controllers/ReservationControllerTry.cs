using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sport_test.Models;
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
        try
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
        catch (Exception ex)
        {
            TempData["message"] = $"Error al cargar reservas: {ex.Message}";
            TempData["success"] = "False";
            ViewBag.Users = new SelectList(Enumerable.Empty<User>(), "Id", "Name");
            ViewBag.Spaces = new SelectList(Enumerable.Empty<SportSpace>(), "Id", "Name");
            return View(Enumerable.Empty<Reservation>());
        }
    }

    public IActionResult Create()
    {
        try
        {
            PopulateDropdowns();
            return View();
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error al cargar el formulario: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Store(Reservation reservation)
    {
        try
        {
            var result = _reservationService.SaveReservation(reservation);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al crear reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Create");
        }
    }

    public IActionResult Show(int id)
    {
        try
        {
            var result = _reservationService.GetReservationById(id);
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
            TempData["message"] = $"Error al obtener reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var result = _reservationService.GetReservationById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            PopulateDropdowns();
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error al cargar reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Update(Reservation reservation)
    {
        try
        {
            var result = _reservationService.UpdateReservation(reservation);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al actualizar reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Cancel(int id)
    {
        try
        {
            var result = _reservationService.CancelReservation(id);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al cancelar reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Destroy(Reservation reservation)
    {
        try
        {
            var result = _reservationService.DeleteReservation(reservation);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error inesperado al eliminar reserva: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    private void PopulateDropdowns()
    {
        ViewBag.Users = new SelectList(_userService.GetAllUsers().Data, "Id", "Name");
        ViewBag.Spaces = new SelectList(_spaceService.GetAllSpaces().Data, "Id", "Name");
        ViewBag.Statuses = new SelectList(new[] { "Activa", "Cancelada", "Finalizada" });
    }
}
