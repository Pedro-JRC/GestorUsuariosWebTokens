using System;
using System.ComponentModel.DataAnnotations;

namespace GestorUsuariosWebTokens.Models
{
    public class UsuarioRegisterRequest
    {
        [Required(ErrorMessage = "Favor ingrese un nombre.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [MinLength(1, ErrorMessage = "El nombre no puede estar en blanco")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Favor ingrese un nombre de usuario.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres.")]
        [MinLength(1, ErrorMessage = "El nombre de usuario no puede estar en blanco")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Favor ingrese un correo.")]
        [MaxLength(50, ErrorMessage = "El correo no puede superar los 50 caracteres.")]
        [MinLength(1, ErrorMessage = "El correo no puede estar en blanco")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Favor ingrese una fecha de nacimiento.")]
        public DateTime FechaDeNacimiento { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }
}
