# Cifrado del flujo para Release

En `Debug`, la aplicación lee `Data\Flujo.xml` directamente. En `Release`, lee
`Data\Flujo.dat`, que se descifra solo en memoria con AES-256-GCM.

## Generar una clave

```powershell
 $bytes = New-Object byte[] 32
  $generador = [System.Security.Cryptography.RandomNumberGenerator]::Create()
  $generador.GetBytes($bytes)
  $generador.Dispose()
  [Convert]::ToBase64String($bytes)
```

Guarde el resultado en un lugar seguro. No lo incorpore al repositorio ni al XML.

## Publicar

En la misma consola defina la clave y publique:

```powershell
$env:F29_FLUJO_KEY = "kLWenkWtYXT64R/V7qOStMs7Eq3NmGieBz9K5N/oxcA="
dotnet publish -c Release
```

La publicación genera `Data\Flujo.dat` y excluye `Data\Flujo.xml`.
La variable `F29_FLUJO_KEY` también debe existir en el equipo donde se ejecute la
versión Release. Sin ella, el modo de flujo XML abrirá las credenciales vacías y
reportará que no pudo descifrar el flujo al iniciar la prueba.

La publicación Release también instala automáticamente Chromium en
`ms-playwright` dentro de la carpeta publicada. Distribuya toda esa carpeta,
incluyendo `ms-playwright`; la primera publicación requiere conexión a Internet.
