# Tutorial: crear un flujo XML

El archivo de ejemplo se encuentra en `Data/Flujo.xml`. Representa el proceso que actualmente realiza la aplicación para iniciar sesión en el SII y descargar un Formulario 29.

## Estructura básica

Un flujo contiene una configuración opcional y una lista de pasos ordenados:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Flujo nombre="Mi flujo" version="1.0">
  <Configuracion>
    <ReintentosLogin maximos="2" esperaMilisegundos="2000" />
  </Configuracion>
  <Pasos>
    <Paso orden="1" tipo="navegar" url="https://ejemplo.cl" esperarHasta="DOMContentLoaded" />
    <Paso orden="2" tipo="esperar" milisegundos="1000" />
  </Pasos>
</Flujo>
```

Cada `Paso` debe tener un número `orden` único y consecutivo. La aplicación debe ejecutarlos desde el número menor al mayor.

## Variables

Se pueden utilizar estas variables en atributos, URLs y scripts:

| Variable | Valor |
| --- | --- |
| `{rut}` | RUT ingresado para iniciar sesión. |
| `{clave}` | Contraseña ingresada para iniciar sesión. |
| `{periodo}` | Período solicitado, por ejemplo `05/2025`. |
| `{anio}` | Año extraído del período, por ejemplo `2025`. |
| `{mes}` | Nombre del mes extraído del período, por ejemplo `Mayo`. |

No se debe escribir una contraseña real en el XML. Use `{clave}` y marque `ocultarValor="true"` cuando corresponda, para evitar que se escriba en registros.

## Tipos de pasos

### Navegar

Abre una URL. `esperarHasta` puede ser `DOMContentLoaded` o `NetworkIdle`.

```xml
<Paso orden="1" tipo="navegar"
      url="https://sitio.ejemplo.cl/login"
      esperarHasta="NetworkIdle" />
```

### Rellenar un campo

`selector` es un selector CSS y `valor` es el texto que se ingresará.

```xml
<Paso orden="2" tipo="rellenar" selector="#rut" valor="{rut}" />
<Paso orden="3" tipo="rellenar" selector="#clave" valor="{clave}" ocultarValor="true" />
```

### Click y espera de visibilidad

Para botones con un texto específico, indique `texto`. Cuando no sea necesario, basta con `selector`.

```xml
<Paso orden="4" tipo="esperarVisible" selector="button" texto="Ingresar" />
<Paso orden="5" tipo="click" selector="button" texto="Ingresar" />
```

### Esperar un tiempo

El tiempo se expresa en milisegundos.

```xml
<Paso orden="6" tipo="esperar" milisegundos="2000" />
```

### Seleccionar una opción

Selecciona el valor de una lista HTML (`select`).

```xml
<Paso orden="7" tipo="seleccionar" selector="select.gwt-ListBox" valor="0" />
```

### Ejecutar JavaScript

Use este tipo solo cuando los pasos anteriores no sean suficientes. El código se ejecuta dentro de la página actual. Inclúyalo en `CDATA` para no tener que escapar caracteres XML.

```xml
<Paso orden="8" tipo="script" descripcion="Cambiar valor">
  <Codigo><![CDATA[
const campo = document.querySelector('#miCampo');
campo.value = '{periodo}';
campo.dispatchEvent(new Event('change', { bubbles: true }));
  ]]></Codigo>
</Paso>
```

### Popup y PDF

Para el flujo F29, primero se espera el popup, luego se realiza el click que lo abre y finalmente se guarda su contenido.

```xml
<Paso orden="9" tipo="esperarPopup" />
<Paso orden="10" tipo="click" selector="button" texto="Formulario Compacto" />
<Paso orden="11" tipo="guardarPdfPopup"
      destino="Downloads\{rut}\F29_{rut}_{periodo}.pdf" />
<Paso orden="12" tipo="cerrarPopup" />
```

## Validación y pruebas

1. Guarde el archivo con extensión `.xml` y codificación UTF-8.
2. Compruebe que todas las etiquetas tengan cierre y que los atributos estén entre comillas.
3. Revise que los valores de `orden` no se repitan.
4. Pruebe primero con un flujo corto que navegue a una página y espere un elemento.
5. Antes de ejecutar un flujo con credenciales reales, verifique sus selectores en el navegador.

## Flujo F29 de referencia

Use `Data/Flujo.xml` como plantilla. Incluye todos los pasos actuales: autenticación, navegación a Consulta F29, selección de formulario/año/mes, generación del popup, descarga del PDF y cierre de sesión.

> Nota: este documento define el formato utilizado por el archivo de ejemplo. La importación y ejecución dinámica de XML requiere que se implemente el lector de flujos en la aplicación.
