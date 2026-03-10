# 📅 Calendario Inteligente

Sistema de gestión de calendarios y eventos con múltiples vistas, importación de formatos externos, verificación de disponibilidad y predicción meteorológica.

## 🌟 Características Principales

### 📊 Gestión de Calendarios
- **Múltiples calendarios**: Crea y gestiona varios calendarios independientes
- **Personalización**: Asigna colores y nombres personalizados a cada calendario
- **Organización por categorías**: Trabajo, Personal, Deportes, Social, etc.
- **Sin base de datos externa**: Almacenamiento en archivo JSON local

### 🔍 Búsqueda y Verificación
- **Verificador rápido** en la página principal
- **Búsqueda por fecha y hora**: Encuentra eventos en momentos específicos
- **Filtro por calendario**: Busca en todos o en calendarios específicos
- **Vista detallada de resultados**: Tabla con información completa de eventos

### 📅 Vistas de Calendario
- **Vista Diaria**: Agenda completa del día seleccionado
- **Vista Semanal**: Eventos organizados por columnas de días
- **Vista Mensual**: Cuadrícula tradicional de calendario
- **Vista Anual**: Minicalendarios de los 12 meses

### 📥 Importación de Datos
- **Formato iCalendar (.ics)**: Compatible con Google Calendar, Outlook, Apple Calendar
- **Formato CSV**: Archivos personalizados con columnas flexibles
- **Formato JSON**: Integración con APIs y aplicaciones externas
- **Festivos**: Importación automática desde API pública (Nager.Date)

### 🌦️ Integración Meteorológica
- **Predicción del tiempo**: Consulta el pronóstico para fechas futuras
- **API OpenWeatherMap**: Datos meteorológicos precisos
- **Planificación inteligente**: Verifica el clima antes de eventos

### ⚙️ Tipos de Eventos
- **Puntual**: Evento único en fecha específica
- **Semanal**: Se repite cada semana el mismo día
- **Mensual**: Se repite cada mes el mismo día
- **Anual**: Se repite cada año (cumpleaños, aniversarios)
- **Lunes a Viernes**: Eventos laborales o rutinas semanales

### ✏️ Gestión de Eventos
- **Creación rápida**: Formulario intuitivo con validación
- **Edición flexible**: Modifica eventos existentes
- **Excepciones**: Elimina o modifica instancias específicas de eventos recurrentes
- **Exportación**: Descarga calendarios en formato .ICS

## 🏗️ Arquitectura

### Tecnologías Utilizadas
- **Framework**: ASP.NET Core 8.0 (Blazor Server)
- **UI**: MudBlazor 8.15.0
- **Lenguaje**: C# 12
- **Almacenamiento**: Archivo JSON local
- **API Externa**: Swagger/OpenAPI documentación

### Estructura del Proyecto

```
Calendario/
├── Components/
│   ├── Layout/           # Layouts y navegación
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/            # Páginas de la aplicación
│       ├── Home.razor              # Página principal con buscador
│       ├── CalendarioOficial.razor # Vistas de calendario
│       ├── GestionCalendarios.razor # Gestión de calendarios
│       ├── Clima.razor             # Predicción meteorológica
│       └── Dialogs/                # Diálogos modales
├── Controllers/
│   └── ControladorCalendario.cs    # API REST
├── Modelos/
│   ├── EntidadesCalendario.cs      # Modelos de datos
│   ├── ClimaDto.cs                 # DTOs del clima
│   └── FestivosDto.cs              # DTOs de festivos
├── Servicios/
│   ├── RepositorioCalendario.cs    # Persistencia de datos
│   ├── MotorCalendario.cs          # Lógica de negocio
│   ├── ClimaService.cs             # Integración meteorológica
│   └── FestivosService.cs          # Importación de festivos
└── wwwroot/                        # Recursos estáticos
```

### Patrones de Diseño
- **Repository Pattern**: `RepositorioCalendario` abstrae el almacenamiento
- **Service Layer**: Lógica de negocio separada en servicios
- **DTO Pattern**: Objetos de transferencia para APIs externas
- **Singleton**: `RepositorioCalendario` como servicio único

## 🚀 Instalación y Ejecución

### Requisitos Previos
- .NET 8.0 SDK o superior
- Navegador web moderno (Chrome, Firefox, Edge)

### Pasos de Instalación

1. **Clonar o descargar el proyecto**
   ```bash
   cd Calendario
   ```

2. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

3. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

5. **Abrir en el navegador**
   ```
   http://localhost:5275
   ```

### Configuración

La aplicación no requiere configuración adicional. Los datos se guardan automáticamente en:
```
%AppData%\Calendario\datos.json
```

## 📖 Guía de Uso

### 1. Crear un Calendario

1. Ve a **"Configuración y Datos"**
2. Click en **"Nuevo Calendario"**
3. Asigna un **nombre** y **color**
4. Click en **"Actualizar"**

### 2. Agregar Eventos

1. Selecciona un calendario de la lista
2. Rellena el formulario:
   - **Título**: Nombre del evento
   - **Fecha**: Día del evento
   - **Hora Inicio/Fin**: Horario del evento
   - **Categoría**: Tipo de evento
   - **Color**: Color personalizado (opcional)
   - **Repetición**: Tipo de recurrencia
3. Click en **"Crear Evento"**

### 3. Importar Calendarios

#### Desde Google Calendar:
1. En Google Calendar: Configuración → Exportar
2. Descargar archivo .ics
3. En la aplicación: "Importar Calendario"
4. Seleccionar el archivo descargado

#### Desde archivo CSV:
Crear un archivo con este formato:
```csv
Titulo,Fecha,HoraInicio,HoraFin,Categoria
Reunión,2026-01-27,09:00,10:30,Trabajo
Cumpleaños,2026-03-15,00:00,23:59,Personal
```

#### Desde archivo JSON:
```json
[
  {
    "titulo": "Evento",
    "fechaInicio": "2026-01-25T14:00:00",
    "categoria": "Personal"
  }
]
```

### 4. Verificar Disponibilidad

1. En la **página principal**
2. Selecciona **fecha**
3. (Opcional) Selecciona **hora**
4. (Opcional) Selecciona **calendario específico**
5. Click en **"Buscar"**
6. Ve la tabla con todos los eventos encontrados

### 5. Ver Calendario

1. Ve a **"Calendario Completo"**
2. Selecciona la vista: **Día**, **Semana**, **Mes** o **Año**
3. Filtra calendarios usando los chips superiores
4. Navega con las flechas o botón **"Hoy"**

### 6. Gestionar Eventos Recurrentes

#### Eliminar una instancia específica:
1. En vista de calendario, click en el evento
2. Selecciona **"Eliminar solo esta fecha"**

#### Eliminar serie completa:
1. Click en el evento recurrente
2. Selecciona **"Eliminar toda la serie"**

### 7. Exportar Calendario

1. En **"Configuración y Datos"**
2. Click en el icono de **descarga** junto al calendario
3. Se descarga un archivo .ics compatible con otros calendarios

### 8. Consultar el Clima

1. Ve a **"Previsión Meteorológica"**
2. Introduce una **ciudad**
3. Selecciona la **fecha** (hasta 5 días)
4. Ve la predicción completa

### 9. Importar Festivos

1. En **"Configuración y Datos"**
2. Click en **"Importar Festivos"**
3. Selecciona **año** y **país**
4. Se crea un calendario con todos los festivos

## 🎨 Características de la Interfaz

### Indicadores Visuales
- **Colores personalizados**: Cada calendario y evento tiene su color
- **Chips informativos**: Categorías y horarios con etiquetas
- **Estados visuales**: Días pasados en gris, hoy destacado
- **Iconos intuitivos**: Botones con iconos Material Design

### Interactividad
- **Hover effects**: Tarjetas y eventos con efecto al pasar el cursor
- **Click en eventos**: Abre opciones de edición/eliminación
- **Navegación rápida**: Botones de navegación en todas las vistas
- **Responsive**: Adaptado a móviles, tablets y escritorio

### Temas y Colores
- Paleta de colores profesional
- Diseño limpio y moderno
- Alto contraste para legibilidad
- Iconos consistentes

## 🔧 API REST

La aplicación expone una API REST documentada con Swagger.

### Endpoints Disponibles

#### GET /api/Calendario/verificar
Verifica si hay eventos en una fecha/hora específica.

**Parámetros:**
- `fecha` (DateTime): Fecha a consultar
- `calendarioId` (int): ID del calendario
- `hora` (TimeSpan, opcional): Hora específica

**Respuesta:**
```json
{
  "fecha": "25/01/2026",
  "hora": "09:00",
  "tieneEventos": true,
  "eventos": [
    {
      "titulo": "Reunión",
      "horaInicio": "09:00:00",
      "color": "#2196F3",
      "categoria": "Trabajo"
    }
  ]
}
```

#### GET /api/Calendario/exportar/{id}
Exporta un calendario en formato iCalendar (.ics).

**Parámetros:**
- `id` (int): ID del calendario

**Respuesta:** Archivo .ics

### Documentación Swagger

Accede a la documentación interactiva en:
```
http://localhost:5275/swagger
```

## 💾 Modelo de Datos

### DefinicionCalendario
```csharp
{
  "Id": int,
  "Nombre": string,
  "Color": string,
  "Reglas": List<ReglaCalendario>
}
```

### ReglaCalendario (Evento)
```csharp
{
  "Id": int,
  "CalendarioId": int,
  "Titulo": string,
  "Categoria": string,
  "Color": string,
  "Tipo": TipoRegla,
  "FechaInicio": DateTime,
  "FechaFin": DateTime?,
  "HoraInicio": TimeSpan,
  "HoraFin": TimeSpan,
  "DiasSemana": List<int>,
  "Excepciones": List<ExcepcionRegla>
}
```

### ExcepcionRegla
```csharp
{
  "Id": int,
  "ReglaCalendarioId": int,
  "FechaOriginal": DateTime,
  "Tipo": TipoExcepcion,
  "NuevaHoraInicio": TimeSpan?,
  "NuevaHoraFin": TimeSpan?,
  "NuevoTitulo": string?
}
```

### Enumeraciones

**TipoRegla:**
- `Puntual`: Evento único
- `Rango`: Periodo continuo
- `Semanal`: Repetición semanal
- `Mensual`: Repetición mensual
- `Anual`: Repetición anual
- `LunesViernes`: Días laborales

**TipoExcepcion:**
- `Modificar`: Modifica una instancia
- `Eliminar`: Elimina una instancia

## 📊 Almacenamiento

### Ubicación del Archivo
```
Windows: %AppData%\Calendario\datos.json
```

### Formato del Archivo
El archivo JSON contiene toda la estructura de calendarios, eventos y excepciones.

### Backup Manual
Para hacer backup de tus datos:
1. Localiza el archivo `datos.json`
2. Cópalo a una ubicación segura
3. Para restaurar, reemplaza el archivo

### Portabilidad
Los datos son completamente portables:
- Copia el archivo `datos.json` a otro equipo
- Colócalo en la misma ubicación
- La aplicación lo detectará automáticamente

## 🐛 Solución de Problemas

### La aplicación no inicia
```bash
# Verificar la instalación de .NET
dotnet --version

# Limpiar y recompilar
dotnet clean
dotnet build
```

### No se guardan los cambios
- Verifica permisos de escritura en `%AppData%\Calendario\`
- Comprueba el espacio disponible en disco

### Errores de importación
- Verifica que el archivo tenga el formato correcto
- Los archivos CSV deben tener encabezados
- Los archivos JSON deben ser válidos

### El clima no se carga
- Verifica la conexión a Internet
- Comprueba que el nombre de la ciudad sea correcto
- Las predicciones solo están disponibles para 5 días

## 🚦 Limitaciones Conocidas

- **Predicción meteorológica**: Máximo 5 días
- **Tamaño de archivos**: Importación limitada a 10MB
- **Eventos simultáneos**: No hay límite técnico, pero la UI puede saturarse
- **Historial**: No hay "deshacer" global (solo por sesión)

## 🔄 Actualizaciones Futuras

### Planificado
- [ ] Sincronización con Google Calendar
- [ ] Notificaciones de eventos próximos
- [ ] Vista de tareas (To-Do)
- [ ] Compartir calendarios entre usuarios
- [ ] Temas personalizables (claro/oscuro)
- [ ] Exportación en PDF
- [ ] Búsqueda avanzada de eventos

### En Consideración
- [ ] Aplicación móvil
- [ ] Integración con Outlook
- [ ] Recordatorios por email
- [ ] Estadísticas de uso del tiempo

## 📞 Soporte

Para reportar problemas o sugerencias:
- Revisa esta documentación primero
- Verifica la sección de solución de problemas
- Consulta la documentación de APIs (Swagger)

## 📄 Licencia

Este proyecto es de código abierto y está disponible para uso personal y educativo.

---

**Versión:** 1.0  
**Última actualización:** Enero 2026  
**Compatible con:** .NET 8.0+

---

## 📚 Referencias Técnicas

### Dependencias NuGet
```xml
<PackageReference Include="Ical.Net" Version="5.2.0" />
<PackageReference Include="MudBlazor" Version="8.15.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

### Comandos Útiles
```bash
# Compilar en modo Release
dotnet build -c Release

# Publicar aplicación
dotnet publish -c Release -o ./publish

# Ejecutar tests (si existen)
dotnet test

# Limpiar archivos temporales
dotnet clean
```

### Configuración de Desarrollo
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {  
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

¡Gracias por usar Calendario Inteligente! 📅✨
