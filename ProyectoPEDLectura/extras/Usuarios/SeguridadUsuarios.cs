using System;
using System.Security.Cryptography;
using System.Text;
    
namespace ProyectoPEDLectura.extras.Usuarios
{
    // Comentario de clase: clase estática que agrupa utilidades de seguridad relacionadas con usuarios,
    // en particular creación y verificación de hashes de contraseñas con salt aleatorio.
    public static class SeguridadUsuario
    {
        // Comentario del método CrearHashContrasena:
        // Genera un hash para almacenar contraseñas. Se crea un salt aleatorio,
        // se calcula el hash sobre (salt + contraseña) usando SHA-256 y se devuelve una cadena
        // que concatena el salt y el hash en hex separados por ':' para almacenamiento.
        public static string CrearHashContrasena(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = CalcularHash(contrasena, salt);

            return $"{Convert.ToHexString(salt)}:{Convert.ToHexString(hash)}";
        }

        // Comentario del método VerificarContrasena:
        // Valida si la contraseña proporcionada coincide con el hash almacenado.
        // Se extrae el salt y el hash original del string guardado, se recalcula el hash con el salt
        // y se comparan de forma temporalmente constante para evitar ataques de tiempo.
        public static bool VerificarContrasena(string contrasena, string hashGuardado)
        {
            if (string.IsNullOrWhiteSpace(contrasena) || string.IsNullOrWhiteSpace(hashGuardado))
                return false;

            // Se espera el formato "saltHex:hashHex"
            string[] partes = hashGuardado.Split(':');

            if (partes.Length != 2)
                return false;

            try
            {
                // Convierte las partes hex a bytes.
                byte[] salt = Convert.FromHexString(partes[0]);
                byte[] hashOriginal = Convert.FromHexString(partes[1]);

                // Recalcula el hash de la contraseña proporcionada usando el salt extraído.
                byte[] hashNuevo = CalcularHash(contrasena, salt);

                // Compara los hashes usando una comparación en tiempo fijo para evitar fuga de información.
                return CryptographicOperations.FixedTimeEquals(hashOriginal, hashNuevo);
            }
            catch
            {
                // En caso de cualquier error (formato inválido, conversión fallida) no validar la contraseña.
                return false;
            }
        }

        // Comentario del método CalcularHash:
        // Esta función toma una contraseña y un salt (bytes) y calcula el hash SHA-256 de la concatenación (salt + contraseña).
        // Se usa para calcular tanto el hash inicial al crear la contraseña como para verificar proporcionando el mismo salt.
        private static byte[] CalcularHash(string contrasena, byte[] salt)
        {
            // Explicación sobre SHA-256:
            // SHA-256 es una función hash criptográfica que produce un valor de 32 bytes (256 bits).
            // Se usa aquí para derivar de forma determinística y segura una huella de la contraseña combinada con el salt.
            using SHA256 sha256 = SHA256.Create();

            // Convierte la contraseña a bytes en UTF-8.
            byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);

            // Crea un buffer que contendrá primero el salt y luego la contraseña en bytes.
            // Esto asegura que el salt influya en todo el proceso de hashing.
            byte[] datos = new byte[salt.Length + contrasenaBytes.Length];

            // Explicación sobre Buffer.BlockCopy:
            // Buffer.BlockCopy copia bloques de bytes entre arrays de forma eficiente.
            // Aquí se usa para concatenar el salt y la contraseña en el buffer 'datos' sin crear arrays intermedios innecesarios.
            // Las ventajas de usar Buffer.BlockCopy: rendimiento y claridad al manipular arrays de bytes directamente.
            Buffer.BlockCopy(salt, 0, datos, 0, salt.Length);
            Buffer.BlockCopy(contrasenaBytes, 0, datos, salt.Length, contrasenaBytes.Length);

            // Calcula y devuelve el hash SHA-256 del buffer (salt || contraseña).
            // Se devuelve el arreglo de bytes resultante (32 bytes).
            return sha256.ComputeHash(datos);
        }
    }
}