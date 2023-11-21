namespace StudentLockerSystem.Models
{
    public static class ExtensionReader
    {
        public static bool IsImage(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return imageExtensions.Contains(extension.ToLower());
        }

        public static bool IsPdf(string extension)
        {
            return extension.ToLower() == ".pdf";
        }

        public static bool IsExcel(string extension)
        {
            string[] excelExtensions = { ".xls", ".xlsx" };
            return excelExtensions.Contains(extension.ToLower());
        }

        public static bool IsWord(string extension)
        {
            string[] wordExtensions = { ".doc", ".docx" };
            return wordExtensions.Contains(extension.ToLower());
        }

        public static bool IsZip(string extension)
        {
            return extension.ToLower() == ".zip";
        }

        public static bool IsText(string extension)
        {
            string[] textExtensions = { ".txt", ".log" };
            return textExtensions.Contains(extension.ToLower());
        }
    }
}
