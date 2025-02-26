using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorUsuariosWebTokens.Data;
using GestorUsuariosWebTokens.Helpers;
using GestorUsuariosWebTokens.Models;
using Microsoft.AspNetCore.Authorization;


namespace GestorUsuariosWebTokens.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios (protegido)
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/usuarios/{id} (protegido)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }
            return usuario;
        }

        // POST: api/usuarios (Registro, permite acceso anónimo)
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioRegisterRequest request)
        {
            // Validar que no exista otro usuario con el mismo correo o nombre de usuario
            if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            {
                return BadRequest(new { message = "Ya existe un usuario con este correo." });
            }
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == request.NombreUsuario))
            {
                return BadRequest(new { message = "Ya existe un usuario con este nombre de usuario." });
            }

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                NombreUsuario = request.NombreUsuario,
                Correo = request.Correo,
                FechaDeNacimiento = request.FechaDeNacimiento,
                PasswordHash = SecurityHelper.ComputeSha256Hash(request.Password)
            };

            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error al guardar usuario.", error = ex.Message });
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // PUT: api/usuarios/{id} (Actualizar usuario, protegido)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest(new { message = "El ID del usuario no coincide." });
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Actualizar campos
            usuario.Nombre = request.Nombre;
            usuario.NombreUsuario = request.NombreUsuario;
            usuario.Correo = request.Correo;
            usuario.FechaDeNacimiento = request.FechaDeNacimiento;

            // Si se envía una nueva contraseña, calcular el hash
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                usuario.PasswordHash = SecurityHelper.ComputeSha256Hash(request.Password);
            }

            _context.Entry(usuario).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(new { message = "Usuario no encontrado." });
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el usuario.", error = ex.Message });
            }

            return NoContent();
        }

        // DELETE: api/usuarios/{id} (protegido)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
