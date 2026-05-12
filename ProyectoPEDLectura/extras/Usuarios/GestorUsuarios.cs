using System;
using System.IO;
using System.Text;

namespace ProyectoPEDLectura.extras.Usuarios
{
    public static class GestorUsuarios
    {
        private static ListaUsuarios usuarios = new ListaUsuarios();

        static GestorUsuarios()
        {
            GestorRutasUsuario.CrearEstructuraBase();
            CargarUsuarios();
        }

        public static Usuario CrearUsuario(string nombreUsuario, string contrasena, string genero, string rutaFotoOrigen)
        {
            nombreUsuario = nombreUsuario.Trim();
            genero = genero.Trim();

            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new Exception("El nombre de usuario no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(contrasena))
                throw new Exception("La contraseña no puede estar vacía.");

            if (string.IsNullOrWhiteSpace(genero))
                throw new Exception("Debe seleccionar un género.");

            if (string.IsNullOrWhiteSpace(rutaFotoOrigen) || !File.Exists(rutaFotoOrigen))
                throw new Exception("Debe seleccionar una foto de perfil válida.");

            CargarUsuarios();

            if (usuarios.ExisteNombre(nombreUsuario))
                throw new Exception("Ya existe un usuario con ese nombre.");

            string codigo = GenerarCodigoUsuario();

            GestorRutasUsuario.CrearEstructuraUsuario(codigo);

            string rutaFotoFinal = CopiarFotoPerfil(codigo, rutaFotoOrigen);

            Usuario nuevoUsuario = new Usuario
            {
                Codigo = codigo,
                NombreUsuario = nombreUsuario,
                ContrasenaHash = SeguridadUsuario.CrearHashContrasena(contrasena),
                Genero = genero,
                RutaFotoPerfil = rutaFotoFinal,
                FechaCreacion = DateTime.Now
            };

            usuarios.Agregar(nuevoUsuario);
            GuardarUsuarios();

            return nuevoUsuario;
        }

        public static Usuario? ValidarLogin(string nombreUsuario, string contrasena)
        {
            CargarUsuarios();

            Usuario? usuario = usuarios.BuscarPorNombre(nombreUsuario.Trim());

            if (usuario == null)
                return null;

            bool contrasenaCorrecta = SeguridadUsuario.VerificarContrasena(
                contrasena,
                usuario.ContrasenaHash
            );

            if (!contrasenaCorrecta)
                return null;

            return usuario;
        }

        public static Usuario? BuscarPorNombre(string nombreUsuario)
        {
            CargarUsuarios();
            return usuarios.BuscarPorNombre(nombreUsuario.Trim());
        }

        public static Usuario? BuscarPorCodigo(string codigo)
        {
            CargarUsuarios();
            return usuarios.BuscarPorCodigo(codigo.Trim());
        }

        public static bool ActualizarUsuario(Usuario usuarioActualizado)
        {
            if (usuarioActualizado == null)
                return false;

            CargarUsuarios();

            bool actualizado = usuarios.Actualizar(usuarioActualizado);

            if (actualizado)
            {
                GuardarUsuarios();
            }

            return actualizado;
        }

        public static bool ExisteOtroUsuarioConEseNombre(string codigoUsuarioActual, string nuevoNombreUsuario)
        {
            CargarUsuarios();

            Usuario? usuarioEncontrado = usuarios.BuscarPorNombre(nuevoNombreUsuario.Trim());

            if (usuarioEncontrado == null)
                return false;

            return !string.Equals(
                usuarioEncontrado.Codigo,
                codigoUsuarioActual,
                StringComparison.OrdinalIgnoreCase
            );
        }

        public static string ActualizarFotoPerfil(string codigoUsuario, string rutaFotoOrigen)
        {
            if (string.IsNullOrWhiteSpace(codigoUsuario))
                throw new Exception("No hay usuario activo.");

            if (string.IsNullOrWhiteSpace(rutaFotoOrigen) || !File.Exists(rutaFotoOrigen))
                throw new Exception("La imagen seleccionada no existe.");

            return CopiarFotoPerfil(codigoUsuario, rutaFotoOrigen);
        }

        private static string GenerarCodigoUsuario()
        {
            int numero = usuarios.Cantidad + 1;
            string codigo = $"USR{numero:000}";

            while (usuarios.ExisteCodigo(codigo))
            {
                numero++;
                codigo = $"USR{numero:000}";
            }

            return codigo;
        }

        private static string CopiarFotoPerfil(string codigoUsuario, string rutaFotoOrigen)
        {
            string extension = Path.GetExtension(rutaFotoOrigen);

            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";

            string carpetaUsuario = GestorRutasUsuario.ObtenerCarpetaUsuario(codigoUsuario);
            Directory.CreateDirectory(carpetaUsuario);

            string rutaFotoFinal = Path.Combine(carpetaUsuario, "fotoPerfil" + extension);

            if (!string.Equals(rutaFotoOrigen, rutaFotoFinal, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(rutaFotoOrigen, rutaFotoFinal, true);
            }

            return rutaFotoFinal;
        }

        private static string LimpiarTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return texto.Replace("|", "/").Replace(Environment.NewLine, " ");
        }

        private static void GuardarUsuarios()
        {
            StringBuilder contenido = new StringBuilder();

            usuarios.Recorrer(usuario =>
            {
                contenido.AppendLine(
                    $"{LimpiarTexto(usuario.Codigo)}|" +
                    $"{LimpiarTexto(usuario.NombreUsuario)}|" +
                    $"{LimpiarTexto(usuario.ContrasenaHash)}|" +
                    $"{LimpiarTexto(usuario.Genero)}|" +
                    $"{LimpiarTexto(usuario.RutaFotoPerfil)}|" +
                    $"{usuario.FechaCreacion:yyyy-MM-dd HH:mm:ss}"
                );
            });

            File.WriteAllText(GestorRutasUsuario.ArchivoUsuarios, contenido.ToString(), Encoding.UTF8);
        }

        private static void CargarUsuarios()
        {
            usuarios.Limpiar();

            GestorRutasUsuario.CrearEstructuraBase();

            if (!File.Exists(GestorRutasUsuario.ArchivoUsuarios))
                return;

            string[] lineas = File.ReadAllLines(GestorRutasUsuario.ArchivoUsuarios, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 6)
                    continue;

                Usuario usuario = new Usuario
                {
                    Codigo = datos[0],
                    NombreUsuario = datos[1],
                    ContrasenaHash = datos[2],
                    Genero = datos[3],
                    RutaFotoPerfil = datos[4],
                    FechaCreacion = DateTime.TryParse(datos[5], out DateTime fecha)
                        ? fecha
                        : DateTime.Now
                };

                usuarios.Agregar(usuario);

                if (!string.IsNullOrWhiteSpace(usuario.Codigo))
                {
                    GestorRutasUsuario.CrearEstructuraUsuario(usuario.Codigo);
                }
            }
        }
    }
}