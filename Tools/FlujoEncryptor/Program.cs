using System.Security.Cryptography;

if (args.Length != 2)
{
    Console.Error.WriteLine("Uso: FlujoEncryptor <archivo-xml> <archivo-cifrado>");
    return 1;
}

string? claveBase64 = Environment.GetEnvironmentVariable("F29_FLUJO_KEY");

if (string.IsNullOrWhiteSpace(claveBase64))
{
    Console.Error.WriteLine("No está definida la variable de entorno F29_FLUJO_KEY.");
    return 1;
}

byte[] clave;

try
{
    clave = Convert.FromBase64String(claveBase64);
}
catch (FormatException)
{
    Console.Error.WriteLine("F29_FLUJO_KEY debe estar codificada en Base64.");
    return 1;
}

if (clave.Length != 32)
{
    Console.Error.WriteLine("F29_FLUJO_KEY debe contener 32 bytes para AES-256.");
    return 1;
}

byte[] contenidoPlano = await File.ReadAllBytesAsync(args[0]);
byte[] nonce = RandomNumberGenerator.GetBytes(12);
byte[] contenidoCifrado = new byte[contenidoPlano.Length];
byte[] etiqueta = new byte[16];

using (AesGcm aes = new(clave, etiqueta.Length))
{
    aes.Encrypt(nonce, contenidoPlano, contenidoCifrado, etiqueta);
}

string? carpetaDestino = Path.GetDirectoryName(args[1]);

if (!string.IsNullOrWhiteSpace(carpetaDestino))
    Directory.CreateDirectory(carpetaDestino);

byte[] resultado = new byte[nonce.Length + etiqueta.Length + contenidoCifrado.Length];
Buffer.BlockCopy(nonce, 0, resultado, 0, nonce.Length);
Buffer.BlockCopy(etiqueta, 0, resultado, nonce.Length, etiqueta.Length);
Buffer.BlockCopy(contenidoCifrado, 0, resultado, nonce.Length + etiqueta.Length, contenidoCifrado.Length);

await File.WriteAllBytesAsync(args[1], resultado);
Console.WriteLine($"Flujo cifrado generado: {args[1]}");
return 0;
