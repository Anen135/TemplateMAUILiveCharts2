namespace TemplateMAUILiveCharts2.Services
{
    public class CsvFilePicker {
        public async Task<string[,]> PickAndParseCsvAsync(char delimiter = ',') {
            try {
                var result = await FilePicker.PickAsync(new PickOptions {
                    PickerTitle = "Выберите CSV файл",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".csv", ".txt" } },
                        { DevicePlatform.MacCatalyst, new[] { ".csv", ".txt" } },
                        { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                        { DevicePlatform.Android, new[] { "text/csv", "text/comma-separated-values", "application/csv" } }
                    })
                });


                if (result == null)
                    return null; // Пользователь отменил выбор

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);

                var lines = new List<string[]>();

                while (!reader.EndOfStream) {
                    var line = await reader.ReadLineAsync();
                    if (line != null) {
                        var values = line.Split(delimiter);
                        lines.Add(values);
                    }
                }

                // Определяем максимальную ширину, чтобы создать двумерный массив
                int maxColumns = 0;
                foreach (var row in lines) {
                    if (row.Length > maxColumns)
                        maxColumns = row.Length;
                }

                var resultArray = new string[lines.Count, maxColumns];

                for (int i = 0; i < lines.Count; i++) {
                    for (int j = 0; j < lines[i].Length; j++) {
                        resultArray[i, j] = lines[i][j];
                    }
                }

                return resultArray;
            } catch (Exception ex) {
                // Логирование или обработка ошибок
                Console.WriteLine($"Ошибка при выборе или парсинге файла: {ex.Message}");
                return null;
            }
        }
    };
}
