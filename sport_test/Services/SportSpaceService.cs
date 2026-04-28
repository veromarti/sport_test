using Microsoft.EntityFrameworkCore;
using sport_test.Data;
using sport_test.Models;
using sport_test.Responses;

namespace sport_test.Services;

public class SportSpaceService
{
    private readonly MySqlDbContext _context;

    public SportSpaceService(MySqlDbContext context) => _context = context;

    public ServiceResponse<IEnumerable<SportSpace>> GetAllSpaces()
    {
        var spaces = _context.sport_spaces.ToList();
        return new ServiceResponse<IEnumerable<SportSpace>> { Success = true, Data = spaces };
    }

    public ServiceResponse<IEnumerable<SportSpace>> GetSpacesByType(string type)
    {
        var spaces = _context.sport_spaces
            .Where(s => s.Type == type)
            .ToList();
        return new ServiceResponse<IEnumerable<SportSpace>> { Success = true, Data = spaces };
    }

    public ServiceResponse<SportSpace> GetSpaceById(int id)
    {
        var space = _context.sport_spaces.FirstOrDefault(s => s.Id == id);
        if (space != null)
            return new ServiceResponse<SportSpace> { Success = true, Data = space, Message = "Espacio encontrado" };

        return new ServiceResponse<SportSpace> { Success = false, Data = null, Message = "Espacio deportivo no encontrado" };
    }

    public ServiceResponse<SportSpace> SaveSpace(SportSpace space)
    {
        try
        {
            var nameExists = _context.sport_spaces.FirstOrDefault(s => s.Name == space.Name);
            if (nameExists != null)
                return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = "Ya existe un espacio deportivo con ese nombre" };

            _context.sport_spaces.Add(space);
            _context.SaveChanges();
            return new ServiceResponse<SportSpace> { Success = true, Data = space, Message = "Espacio deportivo registrado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = $"Error al registrar espacio: {ex.Message}" };
        }
    }

    public ServiceResponse<SportSpace> UpdateSpace(SportSpace space)
    {
        try
        {
            var spaceDb = _context.sport_spaces.Find(space.Id);
            if (spaceDb == null)
                return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = "Espacio deportivo no encontrado" };

            var nameExists = _context.sport_spaces.FirstOrDefault(s => s.Name == space.Name && s.Id != space.Id);
            if (nameExists != null)
                return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = "Ya existe otro espacio deportivo con ese nombre" };

            spaceDb.Name = space.Name;
            spaceDb.Type = space.Type;
            spaceDb.Capacity = space.Capacity;
            _context.SaveChanges();
            return new ServiceResponse<SportSpace> { Success = true, Data = space, Message = "Espacio deportivo actualizado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = $"Error al actualizar espacio: {ex.Message}" };
        }
    }

    public ServiceResponse<SportSpace> DeleteSpace(SportSpace space)
    {
        try
        {
            var spaceDb = _context.sport_spaces.Find(space.Id);
            if (spaceDb == null)
                return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = "Espacio deportivo no encontrado" };

            _context.sport_spaces.Remove(spaceDb);
            _context.SaveChanges();
            return new ServiceResponse<SportSpace> { Success = true, Data = space, Message = "Espacio deportivo eliminado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace> { Success = false, Data = space, Message = $"Error al eliminar espacio: {ex.Message}" };
        }
    }

    public List<string> GetSpaceTypes()
    {
        return _context.sport_spaces
            .Where(s => s.Type != null)
            .Select(s => s.Type!)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
    }
}
