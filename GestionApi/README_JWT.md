# API de Gestión con Autenticación JWT

Esta API ahora incluye autenticación JWT (JSON Web Token) para proteger los endpoints.

## Características Implementadas

### 1. Autenticación JWT
- Registro de usuarios
- Login con JWT
- Validación de tokens
- Protección de endpoints

### 2. Endpoints de Autenticación

#### POST /api/auth/register
Registra un nuevo usuario en el sistema.

**Request Body:**
```json
{
  "nombreUsuario": "usuario123",
  "email": "usuario@ejemplo.com",
  "password": "contraseña123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiration": "2024-01-25T15:30:00Z",
  "usuario": {
    "id": 1,
    "nombreUsuario": "usuario123",
    "email": "usuario@ejemplo.com",
    "fechaCreacion": "2024-01-25T14:30:00Z",
    "activo": true
  }
}
```

#### POST /api/auth/login
Autentica un usuario existente.

**Request Body:**
```json
{
  "nombreUsuario": "usuario123",
  "password": "contraseña123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiration": "2024-01-25T15:30:00Z",
  "usuario": {
    "id": 1,
    "nombreUsuario": "usuario123",
    "email": "usuario@ejemplo.com",
    "fechaCreacion": "2024-01-25T14:30:00Z",
    "activo": true
  }
}
```

#### GET /api/auth/validate
Valida un token JWT.

**Headers:**
```
Authorization: Bearer {token}
```

### 3. Endpoints Protegidos

Todos los endpoints de tareas ahora requieren autenticación JWT:

- `GET /api/task` - Obtener todas las tareas
- `GET /api/task/{id}` - Obtener tarea por ID
- `POST /api/task` - Crear nueva tarea
- `PUT /api/task/{id}` - Actualizar tarea
- `DELETE /api/task/{id}` - Eliminar tarea

## Configuración

### 1. Configuración JWT en appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "MiClaveSecretaSuperSeguraParaJWT2024!@#$%^&*()",
    "Issuer": "GestionAPI",
    "Audience": "GestionAPIUsers",
    "ExpirationMinutes": 60
  }
}
```

### 2. Migración de Base de Datos
Para aplicar los cambios de la base de datos, ejecuta:
```bash
dotnet ef database update
```

## Uso con Swagger

1. Ejecuta la aplicación
2. Ve a Swagger UI
3. Usa el endpoint `/api/auth/register` o `/api/auth/login` para obtener un token
4. Haz clic en "Authorize" en Swagger
5. Ingresa el token en el formato: `Bearer {tu_token}`
6. Ahora puedes usar todos los endpoints protegidos

## Uso con Postman/Thunder Client

1. Primero, registra o inicia sesión para obtener un token
2. En las peticiones a endpoints protegidos, agrega el header:
   ```
   Authorization: Bearer {tu_token}
   ```

## Seguridad

- Las contraseñas se hashean usando SHA256
- Los tokens JWT tienen una expiración de 60 minutos (configurable)
- Los endpoints están protegidos con el atributo `[Authorize]`
- Se valida la integridad y validez de los tokens

## Estructura de Archivos Agregados

- `Models/Usuario.cs` - Modelo de usuario
- `Models/AuthResponse.cs` - Modelos para respuestas de autenticación
- `Services/IJwtService.cs` - Interfaz del servicio JWT
- `Services/JwtService.cs` - Implementación del servicio JWT
- `Services/IPasswordService.cs` - Interfaz del servicio de contraseñas
- `Services/PasswordService.cs` - Implementación del servicio de contraseñas
- `Controllers/AuthController.cs` - Controlador de autenticación
- `Migrations/20250125000000_AgregarUsuarios.cs` - Migración para tabla de usuarios
