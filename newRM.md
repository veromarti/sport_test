# Sport Reservation System

Sistema web en ASP.NET Core MVC para gestionar usuarios, espacios deportivos y reservas de un complejo deportivo. Construido con EF Core, LINQ, MySQL y envío de correos vía SMTP.

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL Server corriendo localmente
- `dotnet-ef` instalado globalmente:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Cómo ejecutar el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/sport_test.git
cd sport_test
```

### 2. Crear el archivo `appsettings.json`

> ⚠️ Este archivo **no está incluido en el repositorio** (está en `.gitignore` por seguridad). Debes crearlo manualmente en la carpeta `sport_test/`.

Crea `sport_test/appsettings.json` con el siguiente contenido:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=sportreservation;user=root;password=TU_CONTRASEÑA_AQUI"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "User": "tu_correo@gmail.com",
    "Password": "tu_contraseña_de_aplicacion"
  }
}
```

> 💡 Si usas Gmail, debes generar una **contraseña de aplicación** en tu cuenta Google (Seguridad → Verificación en dos pasos → Contraseñas de aplicación). El envío de correo es opcional; si no se configura, el sistema seguirá funcionando.

### 3. Restaurar paquetes

```bash
cd sport_test
dotnet restore
```

### 4. Aplicar migraciones y crear la base de datos

Las migraciones ya están incluidas en el repositorio. Solo ejecuta:

```bash
dotnet ef database update
```

Esto creará automáticamente las tablas `users`, `sport_spaces` y `reservations` en MySQL.

### 5. Eliminar archivos de controlador duplicados

> ⚠️ El repositorio contiene archivos de prueba que **deben eliminarse** antes de compilar, ya que definen clases con el mismo nombre y causarán un error de compilación:

```bash
rm Controllers/ReservationControllerTry.cs
rm Controllers/SportSpaceControllerTry.cs
```

### 6. Ejecutar el proyecto

```bash
dotnet run
```

Abre tu navegador en: **`http://localhost:5167`**

---

## Estructura del proyecto

```
sport_test/
├── Controllers/
│   ├── HomeController.cs           → Página principal
│   ├── UserController.cs           → Gestión de usuarios (listar, crear, editar, eliminar)
│   ├── SportSpaceController.cs     → Gestión de espacios deportivos
│   └── ReservationController.cs    → Gestión de reservas con validaciones de negocio
├── Data/
│   └── MySqlDbContext.cs           → Contexto de base de datos (EF Core + MySQL)
├── Migrations/                     → Migraciones generadas por EF Core
├── Models/
│   ├── User.cs                     → Entidad usuario (nombre, documento, teléfono, email)
│   ├── SportSpace.cs               → Entidad espacio deportivo (nombre, tipo, capacidad)
│   ├── Reservation.cs              → Entidad reserva (usuario, espacio, fecha, hora, estado)
│   └── ErrorViewModel.cs
├── Responses/
│   └── ServiceResponse.cs          → Wrapper genérico de respuesta de servicios
├── Services/
│   ├── UserService.cs              → Lógica de negocio de usuarios (CRUD + validaciones)
│   ├── SportSpaceService.cs        → Lógica de negocio de espacios (CRUD + filtrado por tipo)
│   ├── ReservationService.cs       → Lógica de reservas + validaciones de solapamiento + correos
│   └── EmailService.cs             → Envío de correos electrónicos via SMTP
├── Views/
│   ├── Home/
│   ├── User/                       → Index, Create, Edit, Show
│   ├── SportSpace/                 → Index, Create, Edit, Show
│   ├── Reservation/                → Index, Create, Edit, Show
│   └── Shared/                     → _Layout, Error
├── wwwroot/
├── Program.cs
├── appsettings.json                → ⚠️ No incluido en el repo — debes crearlo (ver paso 2)
└── sport_test.csproj
```

---

## Funcionalidades principales

### Usuarios
- Registrar usuario con nombre, documento de identidad, teléfono y correo electrónico
- Editar información de un usuario existente
- Validar unicidad por documento de identidad y correo electrónico (no permite duplicados)
- Listar todos los usuarios registrados
- Eliminar usuario

### Espacios Deportivos
- Registrar espacio deportivo con nombre, tipo (Fútbol, Baloncesto, Piscina, Tenis, Voleibol, Squash, Atletismo, Otro) y capacidad
- Editar información del espacio
- Validar que no existan espacios con el mismo nombre
- Listar todos los espacios y filtrar por tipo mediante botones
- Eliminar espacio deportivo

### Reservas
- Crear una reserva asociando un usuario, un espacio deportivo, fecha, hora de inicio y hora de fin
- Validar que no existan reservas solapadas para el mismo espacio deportivo
- Validar que el mismo usuario no tenga dos reservas en el mismo rango de horario
- Validar que la hora de fin sea mayor a la hora de inicio
- Validar que no se puedan crear reservas en fechas u horas pasadas
- Gestionar estados de reserva: **Activa**, **Cancelada**, **Finalizada**
- Cancelar una reserva (cambia su estado a "Cancelada")
- Filtrar reservas por usuario o por espacio deportivo
- Eliminar reserva

### Notificaciones por correo
- Se envía un correo de confirmación al usuario cuando se crea una reserva exitosamente
- Se envía un correo de notificación cuando una reserva es cancelada

---

## Reglas de negocio

| Regla | Descripción |
|---|---|
| Sin solapamiento por espacio | Un espacio no puede tener dos reservas activas en el mismo rango de fechas y horas |
| Sin solapamiento por usuario | Un usuario no puede tener dos reservas activas en el mismo rango de fechas y horas |
| Hora de fin > hora de inicio | Se valida antes de guardar |
| Sin reservas en el pasado | No se permite crear reservas en fechas anteriores a hoy ni en horas ya transcurridas del día actual |
| Documento único | No pueden existir dos usuarios con el mismo documento de identidad |
| Correo único | No pueden existir dos usuarios con el mismo correo electrónico |
| Nombre de espacio único | No pueden existir dos espacios deportivos con el mismo nombre |

---

## Tecnologías utilizadas

- **ASP.NET Core MVC** (.NET 10) — Framework web
- **Entity Framework Core 9** + **Pomelo.EntityFrameworkCore.MySql** — ORM y acceso a base de datos
- **LINQ** — Consultas sobre datos y validaciones de solapamiento
- **List\<T\>** — Colecciones en servicios y vistas
- **MySQL** — Base de datos relacional
- **SMTP / System.Net.Mail** — Envío de correos electrónicos
- **Bootstrap 5 + Font Awesome 6** — Interfaz de usuario

---

## Manejo de errores

- Todos los controladores implementan bloques `try-catch` que capturan excepciones inesperadas y muestran mensajes claros al usuario sin exponer detalles técnicos.
- Los servicios retornan un `ServiceResponse<T>` con `Success`, `Message` y `Data`, permitiendo que los controladores distingan entre errores de negocio y errores del sistema.
- Las validaciones de negocio se aplican antes de cualquier operación de escritura en la base de datos.

---

## Diagramas

Ver la carpeta `/docs` para:
- `class-diagram.png` — Diagrama de clases
- `use-case-diagram.png` — Diagrama de casos de uso
