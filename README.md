
# GestorUsuariosWebTokens API

Esta API REST permite gestionar usuarios y autenticar clientes mediante JSON Web Tokens (JWT). La API se implementa con ASP.NET Core y Entity Framework Core (Code First) y cumple con los siguientes requisitos:

- **Configuración de JWT:** Se han instalado y configurado los paquetes necesarios y el middleware de autenticación JWT.
- **Validación de Datos:** Los modelos usan DataAnnotations para validar propiedades (Required, MaxLength, MinLength, EmailAddress, etc.).
- **Método de Autenticación:** Se ha creado un endpoint (`POST /api/auth/login`) que recibe las credenciales (NombreUsuario y Password). La contraseña se encripta usando SHA-256 y se compara con el valor almacenado en la base de datos. Si son correctas, se genera y devuelve un token JWT.
- **Generación de Tokens JWT:** El token incluye información relevante (NombreUsuario, ID y otros claims) y tiene una fecha de expiración definida.
- **Endpoint para Refrescar el Token:** Se implementa un endpoint (`POST /api/auth/refresh`) que permite renovar el token JWT antes de que expire.
- **Protección de Endpoints:** Los endpoints protegidos requieren autenticación y usan el atributo `[Authorize]`. Las solicitudes sin un token válido son rechazadas.

---

## Requisitos del Sistema

- [.NET 8 o superior](https://dotnet.microsoft.com/download)
- [SQL Server Express / LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [Postman](https://www.postman.com/downloads/) 

---

## Instalación y Configuración

### 1. Clonar el Repositorio


    git clone [<URL-del-repositorio>](https://github.com/Pedro-JRC/GestorUsuariosWebTokens.git)
    cd GestorUsuariosWebTokens


### 2. Configurar la cadena de conexión

Actualiza el archivo `appsettings.json` con la cadena de conexión y la configuración de JWT. Por ejemplo:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GestorUsuariosWebDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      },
    }

### 3. Aplicar las migraciones

Abre la Consola del Administrador de Paquetes y ejecuta:

  

    Add-Migration InitialCreate
    Update-Database


### 4. Ejecutar la aplicación

    
      dotnet run        


# Pruebas de Postman

Esta sección describe los casos de prueba que se deben realizar utilizando Postman para verificar la funcionalidad completa de la API GestorUsuariosWebTokens. Cada caso de prueba incluye la URL, el método HTTP y un ejemplo de cuerpo.

---

1. **Registro de Usuario**

   - **URL:** https://localhost:7263/api/usuarios
   - **Método:** POST
   - **Cuerpo (raw, JSON):**
     ```json
     {
       "nombre": "Pedro Julio Rosario",
       "nombreUsuario": "pjrosario",
       "correo": "correo@outlook.com",
       "fechaDeNacimiento": "1998-06-15",
       "password": "Prueba.1234"
     }
     ```
   - **Nota:** Este endpoint permite registrar un nuevo usuario. La API recibe la contraseña en texto plano, calcula el hash (SHA-256) y almacena el resultado en la base de datos.
   - **Captura de pantalla:**

     ![registrar usuario](https://github.com/user-attachments/assets/9e9eaa85-a170-4e98-8c7d-01d3f9035c11)


2. **Login (Autenticación)**

   - **URL:** https://localhost:7263/api/auth/login
   - **Método:** POST
   - **Cuerpo (raw, JSON):**
     ```json
     {
       "nombreUsuario": "pjrosario",
       "password": "Prueba.1234"
     }
     ```
   - **Nota:** Este endpoint recibe las credenciales del usuario. Si son correctas, se genera y devuelve un token JWT.
   - **Captura de pantalla:
  
     ![login](https://github.com/user-attachments/assets/60e11002-481b-46ba-a2a5-b13e3d31c5e1)


3. **Refrescar el Token**

   - **URL:** https://localhost:7263/api/auth/refresh
   - **Método:** POST
   - **Nota:** Este endpoint permite renovar el token JWT antes de que expire. Es obligatorio incluir el header:
     ```
     Authorization: Bearer <tu token JWT>
     ```
   - **Captura de pantalla:**
  
     ![RefreshToken](https://github.com/user-attachments/assets/57d68d8d-b777-4d56-bb31-7ea679d93fbd)


4. **Obtener todos los Usuarios**

   - **URL:** https://localhost:7263/api/usuarios
   - **Método:** GET
   - **Nota:** Devuelve la lista completa de usuarios registrados. Se requiere autenticación (envía el header `Authorization: Bearer <tu token JWT>`).
   - **Captura de pantalla error 401:**
  
      ![401 usuarios](https://github.com/user-attachments/assets/f8c3eca4-8e63-4666-b07b-e5ab80e5b5a7)


   - **Captura de pantalla completado:** 

      ![Obtener usuarios](https://github.com/user-attachments/assets/1882da7e-4d99-4e8f-982b-433a2ff712b9)


6. **Obtener un Usuario por ID**

   - **URL:** https://localhost:7263/api/usuarios/{id}
   - **Método:** GET
   - **Nota:** Reemplaza `{id}` por el identificador del usuario. Se requiere autenticación.
   - **Captura de pantalla error 401:**
  
      ![401 usuario id](https://github.com/user-attachments/assets/8a58f05e-f494-4ac7-9c88-fb8a69fb29e3)


   - **Captura de pantalla satisfactorio:** 

      ![Obtener por id](https://github.com/user-attachments/assets/88a07721-800c-420f-833c-788a57341e51)


8. **Actualizar Usuario**

   - **URL:** https://localhost:7263/api/usuarios/{id}
   - **Método:** PUT
   - **Cuerpo (raw, JSON):**
     ```json
     {
       "id": 1,
       "nombre": "Pedro Julio Rosario",
       "nombreUsuario": "pjrosario2",
       "correo": "correonuevo@outlook.com",
       "fechaDeNacimiento": "1998-06-15",
       "password": "nuevaClave123"
     }
     ```
   - **Nota:** Actualiza los datos del usuario. Si se envía el campo `"password"`, la API recalcula el hash y actualiza el campo `PasswordHash`. Se requiere autenticación.
   - **Captura de pantalla error 401:**
  
      ![401 actualizar usuario](https://github.com/user-attachments/assets/ddb86920-3ae2-4166-977a-873b87a22a54)


   - **Captura de pantalla satisfactorio:** 

      ![actualizar usuario](https://github.com/user-attachments/assets/54bd8422-09d0-4bae-a468-020eaf87c606)


9. **Eliminar Usuario**

   - **URL:** https://localhost:7263/api/usuarios/{id}
   - **Método:** DELETE
   - **Nota:** Elimina el usuario especificado por el identificador. Se requiere autenticación.
   - **Captura de pantalla error 401:**
  
      ![401 eliminar usuario](https://github.com/user-attachments/assets/06f57cf0-f393-44a2-b1d5-55bef71f5099)


   - **Captura de pantalla satisfactorio:** 

      ![eliminar usuario](https://github.com/user-attachments/assets/3171c07c-4f68-4be5-8fb3-978652979f67)

