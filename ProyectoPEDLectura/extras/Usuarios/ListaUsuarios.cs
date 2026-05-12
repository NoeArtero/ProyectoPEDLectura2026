using System;

namespace ProyectoPEDLectura.extras.Usuarios
{
    // "delegate void" define un tipo que representa métodos que no devuelven valor (void)
    // y que aceptan parámetros específicos. Aquí se usa para poder pasar diferentes
    // acciones (métodos) que operen sobre un Usuario al método Recorrer.
    public delegate void AccionUsuario(Usuario usuario);

    // Lista simplemente enlazada para almacenar objetos Usuario.
    public class ListaUsuarios
    {
        // Referencia al primer nodo de la lista.
        private NodoUsuario? primero;
        // Referencia al último nodo de la lista para facilitar inserciones al final.
        private NodoUsuario? ultimo;
        // Cuenta de elementos en la lista.
        private int cantidad;

        // Devuelve la cantidad de usuarios en la lista.
        public int Cantidad
        {
            get { return cantidad; }
        }

        // Indica si la lista está vacía (no tiene nodos).
        public bool EstaVacia()
        {
            return primero == null;
        }

        // Agrega un nuevo Usuario al final de la lista.
        public void Agregar(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            NodoUsuario nuevoNodo = new NodoUsuario(usuario);

            if (EstaVacia())
            {
                primero = nuevoNodo;
                ultimo = nuevoNodo;
            }
            else
            {
                ultimo!.Siguiente = nuevoNodo;
                ultimo = nuevoNodo;
            }

            cantidad++;
        }

        // Busca y devuelve un Usuario por su código (case-insensitive).
        // Retorna null si no se encuentra o si el código es nulo/espacio.
        public Usuario? BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            NodoUsuario? actual = primero;

            while (actual != null)
            {
                if (string.Equals(actual.Usuario.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Usuario;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        // Busca y devuelve un Usuario por su nombre de usuario (case-insensitive).
        // Retorna null si no se encuentra o si el nombre es nulo/espacio.
        public Usuario? BuscarPorNombre(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return null;

            NodoUsuario? actual = primero;

            while (actual != null)
            {
                if (string.Equals(actual.Usuario.NombreUsuario, nombreUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Usuario;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        // Indica si existe un Usuario con el código dado.
        public bool ExisteCodigo(string codigo)
        {
            return BuscarPorCodigo(codigo) != null;
        }

        // Indica si existe un Usuario con el nombre de usuario dado.
        public bool ExisteNombre(string nombreUsuario)
        {
            return BuscarPorNombre(nombreUsuario) != null;
        }

        // Actualiza los datos de un Usuario existente identificado por su código.
        // Retorna true si se actualizó correctamente, false en caso contrario.
        public bool Actualizar(Usuario usuarioActualizado)
        {
            if (usuarioActualizado == null || string.IsNullOrWhiteSpace(usuarioActualizado.Codigo))
                return false;

            NodoUsuario? actual = primero;

            while (actual != null)
            {
                if (string.Equals(actual.Usuario.Codigo, usuarioActualizado.Codigo, StringComparison.OrdinalIgnoreCase))
                {
                    actual.Usuario.NombreUsuario = usuarioActualizado.NombreUsuario;
                    actual.Usuario.ContrasenaHash = usuarioActualizado.ContrasenaHash;
                    actual.Usuario.Genero = usuarioActualizado.Genero;
                    actual.Usuario.RutaFotoPerfil = usuarioActualizado.RutaFotoPerfil;
                    actual.Usuario.FechaCreacion = usuarioActualizado.FechaCreacion;
                    return true;
                }

                actual = actual.Siguiente;
            }

            return false;
        }

        // Elimina todos los nodos y reinicia la lista.
        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        // Recorre la lista y ejecuta la acción proporcionada para cada Usuario.
        // Se utiliza el delegado AccionUsuario para permitir pasar diferentes comportamientos.
        public void Recorrer(AccionUsuario accion)
        {
            if (accion == null)
                throw new ArgumentNullException(nameof(accion));

            NodoUsuario? actual = primero;

            while (actual != null)
            {
                accion(actual.Usuario);
                actual = actual.Siguiente;
            }
        }
    }
}