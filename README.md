# F29Downloader

Aplicación de escritorio desarrollada en **C# (.NET Windows Forms)** para automatizar la descarga de Formularios 29 (F29) desde el sitio del Servicio de Impuestos Internos (SII) utilizando **Microsoft Playwright**.

---

# Objetivo

Automatizar el proceso de:

- Leer contribuyentes desde un archivo Excel.
- Iniciar sesión automáticamente en el SII.
- Consultar declaraciones F29.
- Descargar el PDF del Formulario Compacto.
- Guardar los PDFs localmente.
- Registrar el resultado en una base de datos SQLite.

---

# Estado actual del proyecto

## Implementado

- Login automático al SII.
- Lectura de Excel.
- Base de datos SQLite.
- Navegación automática.
- Consulta F29.
- Selección del período.
- Apertura del Formulario Compacto.
- Obtención del popup que contiene el PDF.

## En desarrollo

- Descarga correcta del PDF.
- Procesamiento masivo de contribuyentes.
- Manejo de errores.
- Cierre correcto de sesión SII.
- Actualización de estados en SQLite.

---

# Tecnologías utilizadas

- C#
- .NET 8 Windows Forms
- Microsoft Playwright
- SQLite
- EPPlus
- Visual Studio 2022

---

# Requisitos

## Visual Studio

Visual Studio 2022

Workloads:

- Desktop development with .NET

---

## SDK

.NET SDK 8

Verificar:

```bash
dotnet --version
```

Debe mostrar una versión 8.x o superior.

---

## Git

Verificar:

```bash
git --version
```

---

## Node.js

No es obligatorio para ejecutar el proyecto, pero es recomendable tenerlo instalado.

---

# Restaurar paquetes NuGet

Abrir una terminal en la carpeta del proyecto.

Ejecutar:

```bash
dotnet restore
```

o desde Visual Studio:

```
Restore NuGet Packages
```

---

# Instalar Playwright

Después de restaurar los paquetes ejecutar:

```bash
pwsh bin/Debug/net8.0-windows/playwright.ps1 install
```

Si está compilado en Release:

```bash
pwsh bin/Release/net8.0/playwright.ps1 install
```

También puede utilizar:

```bash
playwright install
```

si Playwright está disponible globalmente.

---

# Base de datos

La aplicación crea automáticamente la base SQLite si no existe.

No es necesario crear tablas manualmente.

---

# Dependencias NuGet

Actualmente el proyecto utiliza principalmente:

- Microsoft.Playwright
- Microsoft.Data.Sqlite
- EPPlus

Si algún paquete falta:

```bash
dotnet restore
```

---

# Estructura del proyecto

```
F29Downloader
│
├── Models
│
├── Services
│   ├── BrowserService.cs
│   ├── DatabaseService.cs
│   ├── ExcelService.cs
│   ├── LoginService.cs
│   ├── PdfService.cs
│   └── SiiService.cs
│
├── Form1.cs
│
├── Program.cs
│
├── README.md
│
└── .gitignore
```

---

# Flujo del sistema

```
Excel

↓

SQLite

↓

Login SII

↓

Consulta F29

↓

Formulario Compacto

↓

Popup PDF

↓

Descarga PDF

↓

Guardar Resultado
```

---

# Flujo de desarrollo

```
Form1

↓

SiiService

↓

Playwright

↓

Popup PDF

↓

PdfService

↓

Archivo PDF
```

---

# Configuración inicial

Al abrir el proyecto por primera vez:

1. Restaurar paquetes NuGet.

2. Instalar Playwright.

3. Ejecutar el proyecto.

4. Verificar que abre Chromium.

5. Verificar que el Login funciona.

---

# Próximos desarrollos

## Alta prioridad

- Descargar correctamente el PDF.
- Guardar PDF en disco.
- Nombrar automáticamente los archivos.

---

## Media prioridad

Procesar automáticamente todos los contribuyentes.



---

## Baja prioridad

- Logs.
- Configuración mediante appsettings.json.
- Reintentos automáticos.
- Barra de progreso.

---

# Mejoras futuras

- Exportar resultados.
- Descarga paralela.
- Configuración desde interfaz.
- Soporte para otros formularios.
- Programación automática de tareas.

---

# Notas

El sitio del SII limita la cantidad de sesiones simultáneas.

Siempre se debe cerrar correctamente la sesión al finalizar el proceso.

Este procedimiento aún está pendiente de implementación.

---

# Autor

Cristian Fritz

Proyecto personal de automatización para descarga masiva de Formularios 29.


13 - Botón Forzar Cierre configurado y manejo de excepciones al cerrar. Listo. 
14 - Pasar ProcesarContribuyente a otro archivo y configurar llamados del metodo- 
15 - Probar Excel con contribuyente duplicado y definir comportamiento. 
16 - Tiempo de espera entre contribuyentes. 17 - Barra de progreso real del proceso masivo. 
18 - Manejo completo de errores, reintentos y páginas no disponibles. 
19 - Ejecución con navegador oculto y estado del navegador. 
20 - Refactorización general.



Etapa 6: Robustez y estabilidad
6.1 Detección de PDF existente
Verificar si el PDF ya fue descargado antes de iniciar sesión.
Evitar procesamiento innecesario.
Marcar estado como "Finalizado" o "PDF existente".
6.2 Reintentos automáticos
Máximo 2 reintentos por contribuyente.
Espera entre intentos.
Registrar cantidad de intentos realizados.
6.3 Mejor manejo de errores de inicio de sesión
Detectar contraseña incorrecta.
Detectar mayúsculas activadas.
Detectar usuario bloqueado.
Guardar mensaje específico en la columna Mensaje.
6.4 Validación del archivo Excel
Detectar RUT duplicados.
Detectar RUT + período duplicados.
Mostrar aviso antes de cargar.
Permitir continuar o cancelar.
6.5 Detección de contribuyentes ya procesados
Revisar registros existentes en la base de datos.
Preguntar si desea volver a descargar.
Evitar descargas repetidas.
6.6 Mejor manejo de errores SII
Detectar:
Timeout.
Sesión expirada.
Página no disponible.
Popup PDF no abierto.
Error al descargar PDF.
Registrar mensajes claros.
6.7 Mejoras de registro y auditoría
Crear historial de ejecuciones.
Guardar:
Fecha inicio.
Fecha término.
Cantidad procesada.
Errores.
Cancelaciones.
Generar archivos de log.
Etapa 7: Distribución profesional
7.1 Preparar versión Release
Compilación final.
Eliminación de archivos innecesarios.
Configuración de versión.
7.2 Publicar aplicación
Crear ejecutable distribuible.
Definir instalación:
Dependiente del framework.
Incluyendo runtime .NET.
7.3 Instalador
Crear instalador profesional.
Acceso directo en escritorio.
Carpeta de instalación.
Desinstalador.
7.4 Seguridad
Firmar aplicación (opcional).
Proteger archivos de configuración.
Manejo seguro de credenciales.
Evitar exposición de claves en archivos.
7.5 Configuración inicial
Crear estructura automática:
F29Downloader
│
├── Data
├── Downloads
├── Logs
└── Config
7.6 Mantenimiento y actualización
Sistema de versiones.
Copias de seguridad de base de datos.
Preparar futuras actualizaciones.