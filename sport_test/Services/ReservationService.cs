using Microsoft.EntityFrameworkCore;
using sport_test.Data;
using sport_test.Models;
using sport_test.Responses;

namespace sport_test.Services;

public class ReservationService
{
    private readonly MySqlDbContext _context;
    private readonly EmailService _emailService;

    public ReservationService(MySqlDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public ServiceResponse<IEnumerable<Reservation>> GetAllReservations()
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>> { Success = true, Data = reservations };
    }

    public ServiceResponse<IEnumerable<Reservation>> GetReservationsByUser(int userId)
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .Where(r => r.UserId == userId)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>> { Success = true, Data = reservations };
    }

    public ServiceResponse<IEnumerable<Reservation>> GetReservationsBySpace(int spaceId)
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .Where(r => r.SportSpaceId == spaceId)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>> { Success = true, Data = reservations };
    }

    public ServiceResponse<Reservation> GetReservationById(int id)
    {
        var reservation = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .FirstOrDefault(r => r.Id == id);

        if (reservation != null)
            return new ServiceResponse<Reservation> { Success = true, Data = reservation, Message = "Reserva encontrada" };

        return new ServiceResponse<Reservation> { Success = false, Data = null, Message = "Reserva no encontrada" };
    }

    public ServiceResponse<Reservation> SaveReservation(Reservation reservation)
    {
        try
        {
            // Fecha no puede ser en el pasado
            if (reservation.Date.Date < DateTime.Today)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "No se pueden crear reservas en fechas pasadas" };

            // Si es hoy, la hora de inicio no puede ser en el pasado
            if (reservation.Date.Date == DateTime.Today && reservation.StartTime < DateTime.Now.TimeOfDay)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "No se pueden crear reservas en horas pasadas" };

            // Hora fin debe ser mayor que hora inicio
            if (reservation.EndTime <= reservation.StartTime)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "La hora de fin debe ser mayor a la hora de inicio" };

            // Validar solapamiento de espacio deportivo
            bool spaceOverlap = _context.reservations.Any(r =>
                r.SportSpaceId == reservation.SportSpaceId &&
                r.Date.Date == reservation.Date.Date &&
                r.Status == "Activa" &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);

            if (spaceOverlap)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "El espacio deportivo ya tiene una reserva en ese horario" };

            // Validar solapamiento de usuario
            bool userOverlap = _context.reservations.Any(r =>
                r.UserId == reservation.UserId &&
                r.Date.Date == reservation.Date.Date &&
                r.Status == "Activa" &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);

            if (userOverlap)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "El usuario ya tiene una reserva en ese horario" };

            reservation.Status = "Activa";
            reservation.CreatedAt = DateTime.Now;
            _context.reservations.Add(reservation);
            _context.SaveChanges();

            // Enviar correo de confirmación
            var user = _context.users.Find(reservation.UserId);
            var space = _context.sport_spaces.Find(reservation.SportSpaceId);
            if (user?.Email != null)
            {
                string subject = "Reserva Confirmada - Complejo Deportivo";
                string body = $"Estimado/a {user.Name},\n\n" +
                              $"Su reserva ha sido confirmada:\n" +
                              $"Espacio: {space?.Name} ({space?.Type})\n" +
                              $"Fecha: {reservation.Date:dd/MM/yyyy}\n" +
                              $"Horario: {reservation.StartTime:hh\\:mm} - {reservation.EndTime:hh\\:mm}\n\n" +
                              $"Gracias por utilizar nuestro sistema.\nComplejo Deportivo";
                _emailService.SendEmail(user.Email, subject, body);
            }

            return new ServiceResponse<Reservation> { Success = true, Data = reservation, Message = "Reserva creada exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = $"Error al crear reserva: {ex.Message}" };
        }
    }

    public ServiceResponse<Reservation> UpdateReservation(Reservation reservation)
    {
        try
        {
            var reservationDb = _context.reservations.Find(reservation.Id);
            if (reservationDb == null)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "Reserva no encontrada" };

            string previousStatus = reservationDb.Status ?? "";

            reservationDb.UserId = reservation.UserId;
            reservationDb.SportSpaceId = reservation.SportSpaceId;
            reservationDb.Date = reservation.Date;
            reservationDb.StartTime = reservation.StartTime;
            reservationDb.EndTime = reservation.EndTime;
            reservationDb.Status = reservation.Status;
            _context.SaveChanges();

            // Enviar correo si se cancela la reserva
            if (previousStatus != "Cancelada" && reservation.Status == "Cancelada")
            {
                var user = _context.users.Find(reservationDb.UserId);
                var space = _context.sport_spaces.Find(reservationDb.SportSpaceId);
                if (user?.Email != null)
                {
                    string subject = "Reserva Cancelada - Complejo Deportivo";
                    string body = $"Estimado/a {user.Name},\n\n" +
                                  $"Su reserva ha sido cancelada:\n" +
                                  $"Espacio: {space?.Name}\n" +
                                  $"Fecha: {reservationDb.Date:dd/MM/yyyy}\n" +
                                  $"Horario: {reservationDb.StartTime:hh\\:mm} - {reservationDb.EndTime:hh\\:mm}\n\n" +
                                  $"Complejo Deportivo";
                    _emailService.SendEmail(user.Email, subject, body);
                }
            }

            return new ServiceResponse<Reservation> { Success = true, Data = reservation, Message = "Reserva actualizada exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = $"Error al actualizar reserva: {ex.Message}" };
        }
    }

    public ServiceResponse<Reservation> CancelReservation(int id)
    {
        try
        {
            var reservationDb = _context.reservations.Find(id);
            if (reservationDb == null)
                return new ServiceResponse<Reservation> { Success = false, Data = null, Message = "Reserva no encontrada" };

            reservationDb.Status = "Cancelada";
            _context.SaveChanges();

            var user = _context.users.Find(reservationDb.UserId);
            var space = _context.sport_spaces.Find(reservationDb.SportSpaceId);
            if (user?.Email != null)
            {
                string subject = "Reserva Cancelada - Complejo Deportivo";
                string body = $"Estimado/a {user.Name},\n\n" +
                              $"Su reserva ha sido cancelada:\n" +
                              $"Espacio: {space?.Name}\n" +
                              $"Fecha: {reservationDb.Date:dd/MM/yyyy}\n" +
                              $"Horario: {reservationDb.StartTime:hh\\:mm} - {reservationDb.EndTime:hh\\:mm}\n\n" +
                              $"Complejo Deportivo";
                _emailService.SendEmail(user.Email, subject, body);
            }

            return new ServiceResponse<Reservation> { Success = true, Data = reservationDb, Message = "Reserva cancelada exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation> { Success = false, Data = null, Message = $"Error al cancelar reserva: {ex.Message}" };
        }
    }

    public ServiceResponse<Reservation> DeleteReservation(Reservation reservation)
    {
        try
        {
            var reservationDb = _context.reservations.Find(reservation.Id);
            if (reservationDb == null)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "Reserva no encontrada" };

            _context.reservations.Remove(reservationDb);
            _context.SaveChanges();
            return new ServiceResponse<Reservation> { Success = true, Data = reservation, Message = "Reserva eliminada exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = $"Error al eliminar reserva: {ex.Message}" };
        }
    }
}
