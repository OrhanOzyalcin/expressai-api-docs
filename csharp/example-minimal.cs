/**
 * ExpressAI External Barcodes API - C# Minimal Örnek
 * 
 * Bu örnek .NET 6+ top-level statements kullanır.
 * 
 * Kullanım:
 *   1. Yeni bir proje oluşturun: dotnet new console -n ExpressAIExample
 *   2. Bu dosyayı Program.cs olarak kaydedin
 *   3. Çalıştırın: dotnet run
 */

using System.Net.Http;
using System.Text;
using System.Text.Json;

// =============================================================================
// YAPILANDIRMA - Bu değerleri kendi bilgilerinizle değiştirin
// =============================================================================

var accountId = "{accountId}";           // ExpressAI hesap ID'niz
var apiKey = "EAC_xxxxxxxxxxxx";         // API anahtarınız
var apiSecret = "your-secret-key";       // API gizli anahtarınız

// =============================================================================
// API İSTEĞİ
// =============================================================================

using var http = new HttpClient { BaseAddress = new Uri("https://expressai.com.tr") };

// Request body oluştur
var body = new
{
    apiKey = apiKey,
    apiSecret = apiSecret,
    referenceCodes = new[] { "REF001", "REF002" },
    type = "rawJson"  // pdf, image, rawJson, zpl
};

// API'ye istek gönder
Console.WriteLine("API isteği gönderiliyor...\n");

var response = await http.PostAsync(
    $"/api/external-barcodes/{accountId}",
    new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
);

// Yanıtı oku
var result = await response.Content.ReadAsStringAsync();

// JSON'u güzel formatlı yazdır
var jsonDoc = JsonDocument.Parse(result);
var options = new JsonSerializerOptions { WriteIndented = true };
var prettyJson = JsonSerializer.Serialize(jsonDoc, options);

Console.WriteLine("API Yanıtı:");
Console.WriteLine(prettyJson);

// =============================================================================
// PDF İNDİRME ÖRNEĞİ
// =============================================================================

// PDF almak için type'ı "pdf" olarak değiştirin
var pdfBody = new
{
    apiKey = apiKey,
    apiSecret = apiSecret,
    referenceCodes = new[] { "REF001", "REF002" },
    type = "pdf"
};

Console.WriteLine("\n\nPDF isteği gönderiliyor...\n");

var pdfResponse = await http.PostAsync(
    $"/api/external-barcodes/{accountId}",
    new StringContent(JsonSerializer.Serialize(pdfBody), Encoding.UTF8, "application/json")
);

var pdfResult = await pdfResponse.Content.ReadAsStringAsync();
var pdfJson = JsonDocument.Parse(pdfResult);

// downloadUrl'i al
if (pdfJson.RootElement.TryGetProperty("downloadUrl", out var downloadUrlProp))
{
    var downloadUrl = downloadUrlProp.GetString();
    Console.WriteLine($"Download URL: {downloadUrl}");
    
    // Dosyayı indir
    var fileResponse = await http.GetAsync(downloadUrl);
    if (fileResponse.IsSuccessStatusCode)
    {
        var fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync("barkodlar.pdf", fileBytes);
        Console.WriteLine("PDF indirildi: barkodlar.pdf");
    }
}
else if (pdfJson.RootElement.TryGetProperty("error", out var errorProp))
{
    Console.WriteLine($"Hata: {errorProp.GetString()}");
}

