using System.Security.Cryptography;
using System.Text;

namespace F29Downloader.Services
{
    internal static class FlujoCryptoService
    {
        private const int LongitudNonce = 12;
        private const int LongitudEtiqueta = 16;

        public static string Descifrar(byte[] datosCifrados)
        {
            if (datosCifrados.Length <= LongitudNonce + LongitudEtiqueta)
                throw new InvalidOperationException("El archivo cifrado no tiene un formato válido.");

            byte[] clave = ObtenerClave();
            byte[] nonce = datosCifrados[..LongitudNonce];
            byte[] etiqueta = datosCifrados[LongitudNonce..(LongitudNonce + LongitudEtiqueta)];
            byte[] contenidoCifrado = datosCifrados[(LongitudNonce + LongitudEtiqueta)..];
            byte[] contenidoPlano = new byte[contenidoCifrado.Length];

            using AesGcm aes = new(clave, LongitudEtiqueta);
            aes.Decrypt(nonce, contenidoCifrado, etiqueta, contenidoPlano);

            return Encoding.UTF8.GetString(contenidoPlano);
        }

        private static byte[] ObtenerClave()
        {
            string? claveBase64 = Environment.GetEnvironmentVariable("F29_FLUJO_KEY");

            if (string.IsNullOrWhiteSpace(claveBase64))
                throw new InvalidOperationException("No está definida la variable de entorno F29_FLUJO_KEY.");

            byte[] clave;

            try
            {
                clave = Convert.FromBase64String(claveBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("F29_FLUJO_KEY debe estar codificada en Base64.", ex);
            }

            if (clave.Length != 32)
                throw new InvalidOperationException("F29_FLUJO_KEY debe contener exactamente 32 bytes para AES-256.");

            return clave;
        }
    }
}
