using sport_test.Data;
using sport_test.Models;
using sport_test.Responses;

namespace sport_test.Services;

public class UserService
{
    private readonly MySqlDbContext _context;

    public UserService(MySqlDbContext context) => _context = context;

    public ServiceResponse<IEnumerable<User>> GetAllUsers()
    {
        var users = _context.users.ToList();
        return new ServiceResponse<IEnumerable<User>> { Success = true, Data = users };
    }

    public ServiceResponse<User> GetUserById(int id)
    {
        var user = _context.users.FirstOrDefault(u => u.Id == id);
        if (user != null)
            return new ServiceResponse<User> { Success = true, Data = user, Message = "Usuario encontrado" };

        return new ServiceResponse<User> { Success = false, Data = null, Message = "Usuario no encontrado" };
    }

    public ServiceResponse<User> SaveUser(User user)
    {
        try
        {
            var documentExists = _context.users.FirstOrDefault(u => u.Document == user.Document);
            if (documentExists != null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Ya existe un usuario con ese documento de identidad" };

            var emailExists = _context.users.FirstOrDefault(u => u.Email == user.Email);
            if (emailExists != null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Ya existe un usuario con ese correo electrónico" };

            _context.users.Add(user);
            _context.SaveChanges();
            return new ServiceResponse<User> { Success = true, Data = user, Message = "Usuario registrado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User> { Success = false, Data = user, Message = $"Error al registrar usuario: {ex.Message}" };
        }
    }

    public ServiceResponse<User> UpdateUser(User user)
    {
        try
        {
            var userDb = _context.users.Find(user.Id);
            if (userDb == null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Usuario no encontrado" };

            var documentExists = _context.users.FirstOrDefault(u => u.Document == user.Document && u.Id != user.Id);
            if (documentExists != null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Ya existe otro usuario con ese documento de identidad" };

            var emailExists = _context.users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (emailExists != null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Ya existe otro usuario con ese correo electrónico" };

            userDb.Name = user.Name;
            userDb.Document = user.Document;
            userDb.Phone = user.Phone;
            userDb.Email = user.Email;
            _context.SaveChanges();
            return new ServiceResponse<User> { Success = true, Data = user, Message = "Usuario actualizado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User> { Success = false, Data = user, Message = $"Error al actualizar usuario: {ex.Message}" };
        }
    }

    public ServiceResponse<User> DeleteUser(User user)
    {
        try
        {
            var userDb = _context.users.Find(user.Id);
            if (userDb == null)
                return new ServiceResponse<User> { Success = false, Data = user, Message = "Usuario no encontrado" };

            _context.users.Remove(userDb);
            _context.SaveChanges();
            return new ServiceResponse<User> { Success = true, Data = user, Message = "Usuario eliminado exitosamente" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User> { Success = false, Data = user, Message = $"Error al eliminar usuario: {ex.Message}" };
        }
    }
}
