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
pwsh bin/Debug/net8.0/playwright.ps1 install
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