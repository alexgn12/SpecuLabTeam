using System;

namespace PrototipoApi.Helpers
{
    public static class DateFormatHelper
    {
        // Cambia el formato aquí según lo que necesite el sistema externo
        public static string ToExternalFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
