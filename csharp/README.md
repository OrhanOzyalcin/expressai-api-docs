# ExpressAI API - C# / .NET

C# ve .NET için ExpressAI External Barcodes API entegrasyonu.

## Gereksinimler

- .NET 6.0 veya üzeri
- `System.Net.Http` (.NET ile birlikte gelir)
- `System.Text.Json` (.NET ile birlikte gelir)

## Kurulum

Ek NuGet paketi gerekmez. .NET 6+ ile gelen yerleşik kütüphaneler yeterlidir.

## Model Sınıfları

```csharp
public class BarcodeRequest
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public List<string> ReferenceCodes { get; set; }
    public string Type { get; set; } = "pdf"; // pdf, image, rawJson, zpl
}

public class BarcodeResponse
{
    public bool Success { get; set; }
    public string Type { get; set; }
    public string DownloadUrl { get; set; }
    public string ExpiresAt { get; set; }
    public string FileName { get; set; }
    public List<BarcodeDataItem> Data { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int TokenUsed { get; set; }
    public int TokenBalance { get; set; }
    public List<string> Errors { get; set; }
    public List<string> ErrorRefs { get; set; }
}
```

## Kullanım

### Temel Kullanım

```csharp
using var client = new ExpressAIBarcodeClient(accountId, apiKey, apiSecret);

// PDF olarak barkod al
var pdfResult = await client.GetBarcodesAsync(
    new List<string> { "REF001", "REF002" }, 
    "pdf"
);

if (pdfResult.Success)
{
    await client.DownloadFileAsync(pdfResult.DownloadUrl, "barkodlar.pdf");
}

// rawJson olarak kargo bilgilerini al
var jsonResult = await client.GetBarcodesAsync(
    new List<string> { "REF001" }, 
    "rawJson"
);

foreach (var item in jsonResult.Data)
{
    Console.WriteLine($"Alıcı: {item.Fields?.ReceiverName}");
    Console.WriteLine($"Takip No: {item.Fields?.TrackingNumber}");
}
```

### Minimal Örnek

```csharp
using var http = new HttpClient { BaseAddress = new Uri("https://expressai.com.tr") };

var body = new
{
    apiKey = "EAC_xxxxxxxxxxxx",
    apiSecret = "your-secret-key",
    referenceCodes = new[] { "REF001", "REF002" },
    type = "rawJson"
};

var response = await http.PostAsync(
    $"/api/external-barcodes/{accountId}",
    new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
);

var result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
```

## Örnek Dosyalar

| Dosya | Açıklama |
|-------|----------|
| [ExpressAIClient.cs](./ExpressAIClient.cs) | Tam istemci sınıfı ve model tanımları |
| [example-minimal.cs](./example-minimal.cs) | Minimal örnek (top-level statements) |

## Önemli Notlar

- İndirme bağlantıları **1 saat** geçerlidir
- Maksimum **500** referans kodu gönderilebilir
- Rate limit: **100 istek/dakika**

## Hata Yönetimi

```csharp
try
{
    var result = await client.GetBarcodesAsync(referenceCodes, "pdf");
    // Başarılı işlem
}
catch (Exception ex)
{
    Console.WriteLine($"Hata: {ex.Message}");
    // Hata kodları: INVALID_API_KEY, RATE_LIMIT_EXCEEDED, vb.
}
```

## Zebra Yazıcıya Gönderme (ZPL)

```csharp
// ZPL kodlarını al
var zplResult = await client.GetBarcodesAsync(new List<string> { "REF001" }, "zpl");

if (zplResult.Success && zplResult.Data != null)
{
    foreach (var item in zplResult.Data)
    {
        // TCP/IP ile Zebra yazıcıya gönder
        using var tcpClient = new TcpClient("192.168.1.100", 9100);
        using var stream = tcpClient.GetStream();
        var zplBytes = Encoding.UTF8.GetBytes(item.Zpl);
        await stream.WriteAsync(zplBytes);
    }
}
```

## Daha Fazla Bilgi

Genel API dokümantasyonu için [ana README](../README.md) dosyasına bakın.

