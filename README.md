 `ProyectoPEDLectura2026`

````markdown id="dev70b"
# ProyectoPEDLectura2026

ProyectoPEDLectura2026 es una aplicación de escritorio desarrollada en **C# con Windows Forms**, orientada a la gestión personal de lectura. La aplicación permite registrar libros o documentos, llevar control de páginas leídas, crear metas de lectura, agregar anotaciones, visualizar avances y recibir recomendaciones básicas según los libros registrados.

Este proyecto fue desarrollado para la materia **Programación con Estructuras de Datos**, por lo que una parte importante de su funcionamiento se basa en estructuras personalizadas como listas, pilas y colas, evitando depender directamente de estructuras genéricas como `List<T>`, `Stack<T>`, `Queue<T>` o `Dictionary`.

---

## 1. Objetivo del proyecto

El objetivo principal de la aplicación es ayudar al usuario a organizar su proceso de lectura mediante:

- Registro de libros o archivos de lectura.
- Control de páginas totales y páginas leídas.
- Visualización del progreso de lectura.
- Creación de metas diarias.
- Registro de anotaciones por libro y página.
- Seguimiento diario de páginas leídas.
- Recomendaciones simples basadas en los libros agregados.
- Separación de datos por usuario.

---

## 2. Tecnologías utilizadas

El proyecto utiliza las siguientes tecnologías y librerías:

- C#
- Windows Forms
- .NET para aplicaciones de escritorio en Windows
- Guna UI2 para controles visuales modernos
- Guna Charts para gráficos
- Archivos `.txt` como sistema de almacenamiento local
- Estructuras de datos personalizadas

Paquetes NuGet utilizados en el proyecto:

- `Guna.UI2.WinForms`
- `Guna.Charts.WinForms`
- `DocumentFormat.OpenXml`
- `itext`
- `Bogus`
- `ThemeController.ForGunaUI2`

---

## 3. Requisitos para ejecutar la aplicación

Para poder abrir y ejecutar correctamente el proyecto se necesita:

1. Tener instalado **Visual Studio**.
2. Tener instalada una versión compatible de **.NET para Windows Forms**.
3. Restaurar los paquetes NuGet del proyecto.
4. Ejecutar el proyecto desde Visual Studio.
5. Usar Windows, ya que el proyecto está construido con Windows Forms.

No se necesita instalar una base de datos externa, ya que la aplicación guarda la información en archivos `.txt`.

---

## 4. Instalación del repositorio

### Paso 1: Clonar el repositorio

Abrir una terminal o Git Bash y ejecutar:

```bash
git clone https://github.com/NoeArtero/ProyectoPEDLectura2026.git
````

### Paso 2: Abrir el proyecto

Entrar a la carpeta del proyecto y abrir la solución con Visual Studio.

También se puede abrir directamente el archivo:

```text
ProyectoPEDLectura.slnx
```

### Paso 3: Restaurar paquetes NuGet

Visual Studio normalmente restaura los paquetes automáticamente al abrir el proyecto.

Si no lo hace, se puede realizar desde:

```text
Herramientas → Administrador de paquetes NuGet → Restaurar paquetes
```

O desde la terminal de Visual Studio:

```bash
dotnet restore
```

### Paso 4: Ejecutar la aplicación

En Visual Studio:

```text
Compilar → Iniciar depuración
```

O presionar:

```text
F5
```

---

## 5. Funcionamiento general de la aplicación

El sistema inicia mostrando una pantalla de inicio de sesión. Desde ahí el usuario puede iniciar sesión o crear una cuenta nueva.

Después de iniciar sesión, se abre el panel principal de la aplicación. Cada usuario tiene sus propios libros, anotaciones, metas y registros de avance.

La aplicación no utiliza una base de datos tradicional. En su lugar, crea una carpeta de datos local donde guarda archivos `.txt`.

La estructura general de datos es similar a esta:

```text
Datos/
│
├── Usuarios.txt
│
└── Usuarios/
    │
    └── USR001/
        ├── libros.txt
        ├── anotaciones.txt
        ├── metas.txt
        └── lectura_diaria.txt
```

Cada usuario tiene su propia carpeta. Esto permite separar la información y evitar que los libros o anotaciones de un usuario se mezclen con los de otro.

---

## 6. Flujo principal de uso

El flujo recomendado para usar la aplicación es el siguiente:

1. Crear un usuario nuevo o iniciar sesión.
2. Entrar al panel principal.
3. Agregar libros o documentos desde la vista de libros.
4. Registrar el número total de páginas del libro.
5. Actualizar las páginas leídas conforme se avanza.
6. Crear metas de lectura desde la vista de anotaciones.
7. Agregar anotaciones asociadas a páginas específicas.
8. Revisar el progreso desde Inicio, Libros, Perfil o Anotaciones.
9. Consultar recomendaciones desde el perfil.

---

## 7. Vistas principales de la aplicación

## 7.1. Vista de inicio de sesión

Esta vista permite acceder al sistema.

Funciones principales:

* Iniciar sesión con un usuario existente.
* Crear un usuario nuevo.
* Validar credenciales.
* Crear la estructura de carpetas y archivos del usuario.

Para usar la aplicación es obligatorio iniciar sesión, ya que todos los datos se guardan según el usuario activo.

---

## 7.2. Vista de registro de usuario

Desde esta vista se puede crear una cuenta nueva.

El sistema registra la información del usuario en el archivo:

```text
Usuarios.txt
```

Cada usuario recibe un código propio. Ese código se utiliza para crear su carpeta personal dentro de:

```text
Datos/Usuarios/
```

---

## 7.3. Dashboard o panel principal

Después de iniciar sesión, el usuario accede al panel principal.

Desde aquí se puede navegar entre las diferentes secciones de la aplicación:

* Inicio
* Libros
* Anotaciones
* Perfil
* Cerrar sesión

El Dashboard funciona como contenedor principal de las vistas internas.

---

## 7.4. Vista Inicio

La vista Inicio muestra un resumen general del avance del usuario.

En esta sección se pueden visualizar:

* Cantidad de libros registrados.
* Gráficos de progreso.
* Resumen de libros.
* Páginas leídas.
* Avance general de lectura.

Esta vista sirve para tener una idea rápida del estado actual de la lectura del usuario.

---

## 7.5. Vista Libros

La vista Libros permite administrar los libros o documentos registrados.

Funciones principales:

* Agregar libros o archivos.
* Ver el listado de libros registrados.
* Buscar libros por nombre o código.
* Filtrar libros por categoría.
* Ver el número total de páginas.
* Actualizar páginas leídas.
* Ver el progreso de lectura mediante barra visual.
* Abrir archivos registrados.
* Eliminar libros.

Cada libro guarda información como:

```text
Código
Nombre del archivo
Ruta del archivo
Categoría
Número total de páginas
Páginas leídas
Fecha de agregado
```

El progreso se calcula con base en la siguiente fórmula:

```text
Progreso = páginas leídas / número total de páginas * 100
```

Cuando se actualizan las páginas leídas, el sistema también puede registrar el avance diario del usuario.

---

## 7.6. Vista Agregar Libro

Esta vista se utiliza para registrar un nuevo libro o archivo de lectura.

Datos necesarios:

* Código del libro.
* Nombre del archivo.
* Categoría.
* Número total de páginas.
* Archivo seleccionado.

Al agregar un libro, el sistema guarda la información en:

```text
libros.txt
```

El libro inicia con cero páginas leídas.

---

## 7.7. Vista Anotaciones

La vista Anotaciones permite trabajar con metas y anotaciones asociadas a cada libro.

Funciones principales:

* Buscar libros.
* Filtrar libros por categoría.
* Seleccionar un libro.
* Crear o actualizar metas de lectura.
* Agregar anotaciones por página.
* Leer anotaciones existentes.
* Editar anotaciones.
* Eliminar anotaciones.
* Ver el número total de anotaciones.
* Ver el progreso del libro.

Una anotación se asocia a:

```text
Código del libro
Página
Texto de la anotación
Fecha
```

Las anotaciones se guardan en:

```text
anotaciones.txt
```

---

## 7.8. Metas de lectura

Cada libro puede tener una meta de lectura.

La meta puede incluir:

```text
Páginas diarias
Minutos diarios
```

Estas metas se guardan en:

```text
metas.txt
```

Cuando el usuario registra avances en páginas, el sistema verifica si se cumplió la meta diaria.

---

## 7.9. Control diario de lectura

El proyecto incluye un control diario para registrar cuántas páginas lee el usuario en un día.

Este control permite saber si el usuario cumplió la meta diaria de lectura.

Los avances diarios se guardan en:

```text
lectura_diaria.txt
```

Formato general:

```text
CodigoLibro|Fecha|PaginasLeidasHoy|MetaNotificada
```

Ejemplo:

```text
LIB001|2026-05-16|8|True
```

Esto significa:

* El libro con código `LIB001` tuvo 8 páginas leídas en esa fecha.
* La meta diaria ya fue notificada al usuario.
* El mensaje de felicitación no debería repetirse ese mismo día para el mismo libro.

El sistema registra avances cuando:

* El usuario actualiza páginas leídas desde la vista Libros.
* El usuario agrega o edita anotaciones en páginas superiores al avance anterior.

Si el usuario reduce el número de páginas leídas, el sistema puede actualizar el progreso, pero no cuenta esa reducción como avance diario.

---

## 7.10. Vista Perfil

La vista Perfil muestra información general del usuario y su actividad.

Funciones principales:

* Ver información del usuario activo.
* Consultar libros recientes.
* Ver resumen de progreso.
* Consultar recomendaciones de lectura.

Las recomendaciones se generan con base en los libros registrados por el usuario.

---

## 8. Flujo de datos

La aplicación sigue este flujo general:

```text
Usuario inicia sesión
        ↓
Se carga la carpeta del usuario activo
        ↓
Se leen archivos .txt del usuario
        ↓
Se cargan libros, anotaciones, metas y lectura diaria
        ↓
El usuario realiza acciones en la interfaz
        ↓
Los gestores actualizan estructuras en memoria
        ↓
Los datos se guardan nuevamente en archivos .txt
```

---

## 9. Archivos de datos principales

## 9.1. Usuarios.txt

Guarda los usuarios registrados.

Uso principal:

* Validar inicio de sesión.
* Registrar nuevos usuarios.

---

## 9.2. libros.txt

Guarda los libros de cada usuario.

Contiene datos como:

```text
Código
Nombre
Ruta
Categoría
Número de páginas
Fecha
Páginas leídas
```

---

## 9.3. anotaciones.txt

Guarda las anotaciones registradas por el usuario.

Contiene:

```text
Código del libro
Página
Texto
Fecha
```

---

## 9.4. metas.txt

Guarda las metas de lectura de cada libro.

Contiene:

```text
Código del libro
Páginas diarias
Minutos diarios
```

---

## 9.5. lectura_diaria.txt

Guarda el avance diario por libro.

Contiene:

```text
Código del libro
Fecha
Páginas leídas hoy
Meta notificada
```

Este archivo permite controlar si el usuario cumplió su meta diaria y evitar mostrar el mensaje de felicitación varias veces el mismo día.

---

## 10. Estructuras de datos utilizadas

El proyecto utiliza estructuras personalizadas para reforzar los contenidos de Programación con Estructuras de Datos.

Entre ellas se encuentran:

* Lista de libros.
* Pila de libros.
* Cola de libros.
* Lista de usuarios.
* Lista de anotaciones.
* Lista de metas.
* Lista de lectura diaria.

Estas estructuras permiten almacenar y recorrer información sin depender directamente de estructuras genéricas propias de C#.

---

## 11. Gestores principales

El proyecto separa parte de la lógica en clases gestoras.

## 11.1. GestorUsuarios

Se encarga de:

* Registrar usuarios.
* Validar inicio de sesión.
* Cargar usuarios desde archivo.
* Guardar usuarios en archivo.

## 11.2. GestorRutasUsuario

Se encarga de:

* Crear la carpeta base de datos.
* Crear la carpeta de cada usuario.
* Obtener rutas de archivos como `libros.txt`, `anotaciones.txt`, `metas.txt` y `lectura_diaria.txt`.

## 11.3. GestorLibros

Se encarga de:

* Agregar libros.
* Buscar libros por código.
* Recorrer libros.
* Eliminar libros.
* Guardar libros en archivo.
* Cargar libros desde archivo.
* Actualizar páginas leídas.

## 11.4. GestorLecturaDiaria

Se encarga de:

* Registrar avances diarios.
* Sumar páginas leídas en la fecha actual.
* Verificar si se cumplió la meta diaria.
* Evitar mensajes repetidos de felicitación.
* Guardar el control diario en archivo.

## 11.5. RecomendadorLibros

Se encarga de generar recomendaciones básicas con base en los libros registrados por el usuario.

---

## 12. Funcionamiento del mensaje de felicitación

El mensaje de felicitación aparece cuando el usuario cumple la meta diaria de páginas de un libro.

Ejemplo:

```text
Meta diaria: 5 páginas
Primer avance: 3 páginas
Segundo avance: 2 páginas
Total del día: 5 páginas
```

Cuando el total diario alcanza o supera la meta, la aplicación muestra un mensaje de felicitación.

El mensaje no se repite varias veces el mismo día para el mismo libro, porque el sistema guarda si la meta ya fue notificada.

---

## 13. Acciones obligatorias para usar correctamente la app

Para que la aplicación funcione de forma correcta, el usuario debe:

1. Crear una cuenta o iniciar sesión.
2. Agregar al menos un libro.
3. Registrar correctamente el número total de páginas.
4. Seleccionar un libro antes de agregar anotaciones.
5. Crear una meta si desea recibir mensajes de avance diario.
6. Actualizar páginas leídas o agregar anotaciones para registrar progreso.
7. No borrar manualmente los archivos `.txt` mientras la app está abierta.

---

## 14. Recomendaciones de uso

Se recomienda usar la aplicación en este orden:

1. Crear usuario.
2. Iniciar sesión.
3. Agregar libros.
4. Registrar número total de páginas.
5. Crear metas de lectura.
6. Actualizar páginas leídas.
7. Agregar anotaciones.
8. Revisar progreso en Inicio o Perfil.

---

## 15. Posibles errores comunes

## 15.1. No aparecen libros

Posibles causas:

* No hay libros agregados.
* El usuario inició sesión con otra cuenta.
* El archivo `libros.txt` está vacío.

## 15.2. No aparece el mensaje de felicitación

Posibles causas:

* El libro no tiene meta diaria.
* La meta ya fue cumplida y notificada ese día.
* El avance registrado fue menor que la meta.
* Se redujeron páginas leídas, lo cual no cuenta como avance diario.

## 15.3. No se guardan datos

Posibles causas:

* La aplicación no tiene permisos de escritura.
* Se borró la carpeta `Datos`.
* El archivo está abierto o bloqueado por otro programa.

## 15.4. El progreso no se actualiza

Posibles causas:

* El número de páginas leídas es mayor al total de páginas.
* El libro no tiene número total de páginas válido.
* El archivo de datos fue modificado manualmente con formato incorrecto.

---

## 16. Estructura general del proyecto

La estructura principal del proyecto es:

```text
ProyectoPEDLectura/
│
├── Program.cs
│
├── Vistas/
│   ├── LoginPrincipal.cs
│   ├── DashBoard.cs
│   ├── InicioViews/
│   ├── Libros/
│   ├── Anotaciones/
│   └── Perfil/
│
├── extras/
│   ├── EstructurasPersonalizadas/
│   ├── LibrosAgregados/
│   ├── LecturaDiaria/
│   ├── Recomendaciones/
│   ├── Usuarios/
│   └── Mensaje.cs
│
└── Properties/
```

---

## 17. Notas técnicas

* La aplicación usa archivos `.txt` como almacenamiento local.
* Los datos se separan por usuario.
* Las estructuras personalizadas se usan para cumplir el enfoque de la materia.
* El progreso de lectura se calcula según páginas leídas y páginas totales.
* El control diario usa la fecha actual del sistema.
* El mensaje de felicitación depende de la meta diaria registrada por libro.

---

## 18. Limitaciones actuales

El proyecto todavía puede mejorar en varios aspectos:

* No utiliza base de datos relacional.
* Los archivos `.txt` pueden dañarse si se editan manualmente con formato incorrecto.
* El control diario depende de la fecha del sistema.
* Si el usuario borra archivos de datos, la información puede perderse.
* Algunas funcionalidades podrían separarse más en gestores para reducir código en las vistas.

---

## 19. Futuras mejoras

Algunas mejoras posibles son:

* Implementar base de datos.
* Agregar respaldo automático.
* Exportar reportes de lectura.
* Mostrar historial diario en una tabla.
* Agregar estadísticas semanales o mensuales.
* Mejorar el sistema de recomendaciones.
* Separar completamente la lógica de anotaciones y metas en gestores propios.
* Agregar validaciones adicionales para archivos corruptos.

---

## 20. Conclusión

ProyectoPEDLectura2026 es una aplicación de escritorio enfocada en la organización de lectura personal. Permite registrar libros, controlar avances, crear metas, agregar anotaciones y visualizar el progreso del usuario.

Además de su utilidad práctica, el proyecto demuestra el uso de estructuras de datos personalizadas, manejo de archivos, separación básica de responsabilidades y construcción de interfaces de escritorio con Windows Forms.
