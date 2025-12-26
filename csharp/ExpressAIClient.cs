/**
 * ExpressAI External Barcodes API - C# / .NET İstemci Sınıfı
 * 
 * Gereksinimler:
 *   - .NET 6.0+
 * 
 * Kullanım:
 *   dotnet run
 */

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace ExpressAI.BarcodeClient
{
    // =========================================================================
    // MODEL SINIFLARI
    // =========================================================================
    
    public class BarcodeRequest
    {
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }
        
        [JsonPropertyName("apiSecret")]
        public string ApiSecret { get; set; }
        
        [JsonPropertyName("referenceCodes")]
        public List<string> ReferenceCodes { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = "pdf"; // pdf, image, rawJson, zpl
    }

    public class BarcodeResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; }
        
        [JsonPropertyName("expiresAt")]
        public string ExpiresAt { get; set; }
        
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("data")]
        public List<BarcodeDataItem> Data { get; set; }
        
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        
        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }
        
        [JsonPropertyName("tokenUsed")]
        public int TokenUsed { get; set; }
        
        [JsonPropertyName("tokenBalance")]
        public int TokenBalance { get; set; }
        
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }
        
        [JsonPropertyName("errorRefs")]
        public List<string> ErrorRefs { get; set; }
        
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class BarcodeDataItem
    {
        [JsonPropertyName("referenceCode")]
        public string ReferenceCode { get; set; }
        
        [JsonPropertyName("fields")]
        public BarcodeFields Fields { get; set; }
        
        [JsonPropertyName("zpl")]
        public string Zpl { get; set; }
    }

    public class BarcodeFields
    {
        [JsonPropertyName("referenceNumber")]
        public string ReferenceNumber { get; set; }
        
        [JsonPropertyName("senderInfo")]
        public string SenderInfo { get; set; }
        
        [JsonPropertyName("senderName")]
        public string SenderName { get; set; }
        
        [JsonPropertyName("distributionCenter")]
        public string DistributionCenter { get; set; }
        
        [JsonPropertyName("receiverLabel")]
        public string ReceiverLabel { get; set; }
        
        [JsonPropertyName("receiverName")]
        public string ReceiverName { get; set; }
        
        [JsonPropertyName("receiverAddress")]
        public string ReceiverAddress { get; set; }
        
        [JsonPropertyName("receiverAddress2")]
        public string ReceiverAddress2 { get; set; }
        
        [JsonPropertyName("receiverAddress3")]
        public string ReceiverAddress3 { get; set; }
        
        [JsonPropertyName("receiverCityDistrict")]
        public string ReceiverCityDistrict { get; set; }
        
        [JsonPropertyName("receiverPhone")]
        public string ReceiverPhone { get; set; }
        
        [JsonPropertyName("qrCodeData")]
        public string QrCodeData { get; set; }
        
        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }
        
        [JsonPropertyName("shipmentDate")]
        public string ShipmentDate { get; set; }
        
        [JsonPropertyName("paymentType")]
        public string PaymentType { get; set; }
        
        [JsonPropertyName("deliveryType")]
        public string DeliveryType { get; set; }
        
        [JsonPropertyName("desiCount")]
        public string DesiCount { get; set; }
        
        [JsonPropertyName("desiCountText")]
        public string DesiCountText { get; set; }
        
        [JsonPropertyName("lastDeliveryDate")]
        public string LastDeliveryDate { get; set; }
        
        [JsonPropertyName("courierZone")]
        public string CourierZone { get; set; }
        
        [JsonPropertyName("distributionCenter270")]
        public string DistributionCenter270 { get; set; }
        
        [JsonPropertyName("distributionLine")]
        public string DistributionLine { get; set; }
        
        [JsonPropertyName("distributionZone")]
        public string DistributionZone { get; set; }
    }

    // =========================================================================
    // API İSTEMCİ SINIFI
    // =========================================================================
    
    public class ExpressAIBarcodeClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _accountId;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private const string BaseUrl = "https://expressai.com.tr";

        public ExpressAIBarcodeClient(string accountId, string apiKey, string apiSecret)
        {
            _accountId = accountId;
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// Barkod verilerini al
        /// </summary>
        /// <param name="referenceCodes">Referans kodları</param>
        /// <param name="type">Çıktı tipi: pdf, image, rawJson, zpl</param>
        /// <returns>API yanıtı</returns>
        public async Task<BarcodeResponse> GetBarcodesAsync(
            List<string> referenceCodes, 
            string type = "pdf")
        {
            var request = new BarcodeRequest
            {
                ApiKey = _apiKey,
                ApiSecret = _apiSecret,
                ReferenceCodes = referenceCodes,
                Type = type
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(
                $"/api/external-barcodes/{_accountId}", 
                content
            );

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BarcodeResponse>(responseBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(result?.Error ?? "API request failed");
            }

            return result;
        }

        /// <summary>
        /// PDF veya ZIP dosyasını indir
        /// </summary>
        /// <param name="downloadUrl">İndirme URL'i</param>
        /// <param name="outputPath">Kayıt yolu</param>
        public async Task DownloadFileAsync(string downloadUrl, string outputPath)
        {
            var response = await _httpClient.GetAsync(downloadUrl);
            response.EnsureSuccessStatusCode();
            
            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(outputPath, bytes);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // =========================================================================
    // ÖRNEK KULLANIM
    // =========================================================================
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Yapılandırma - Bu değerleri kendi bilgilerinizle değiştirin
            var accountId = "{accountId}";           // ExpressAI hesap ID'niz
            var apiKey = "EAC_xxxxxxxxxxxx";         // API anahtarınız
            var apiSecret = "your-secret-key";       // API gizli anahtarınız

            using var client = new ExpressAIBarcodeClient(accountId, apiKey, apiSecret);

            try
            {
                // ----- PDF olarak barkod alma -----
                Console.WriteLine("PDF olarak barkod alınıyor...");
                
                var pdfResult = await client.GetBarcodesAsync(
                    new List<string> { "REF001", "REF002" }, 
                    "pdf"
                );

                if (pdfResult.Success)
                {
                    Console.WriteLine($"PDF URL: {pdfResult.DownloadUrl}");
                    Console.WriteLine($"Geçerlilik: {pdfResult.ExpiresAt}");
                    
                    // PDF'i indir
                    await client.DownloadFileAsync(pdfResult.DownloadUrl, "barkodlar.pdf");
                    Console.WriteLine("PDF indirildi: barkodlar.pdf");
                }

                // ----- rawJson olarak kargo bilgilerini alma -----
                Console.WriteLine("\nrawJson olarak kargo bilgileri alınıyor...");
                
                var jsonResult = await client.GetBarcodesAsync(
                    new List<string> { "REF001", "REF002" }, 
                    "rawJson"
                );

                if (jsonResult.Success)
                {
                    Console.WriteLine("\nAPI Response:");
                    var prettyJson = JsonSerializer.Serialize(jsonResult, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(prettyJson);
                }

                // ----- ZPL kodlarını alma (Zebra yazıcı için) -----
                Console.WriteLine("\nZPL kodları alınıyor...");
                
                var zplResult = await client.GetBarcodesAsync(
                    new List<string> { "REF001" }, 
                    "zpl"
                );

                if (zplResult.Success && zplResult.Data != null)
                {
                    foreach (var item in zplResult.Data)
                    {
                        Console.WriteLine($"Referans: {item.ReferenceCode}");
                        Console.WriteLine($"ZPL uzunluğu: {item.Zpl?.Length} karakter");
                        
                        // Zebra yazıcıya gönderme (TCP/IP)
                        // using var tcpClient = new TcpClient("192.168.1.100", 9100);
                        // using var stream = tcpClient.GetStream();
                        // var zplBytes = Encoding.UTF8.GetBytes(item.Zpl);
                        // await stream.WriteAsync(zplBytes);
                    }
                }

                // ----- Token bilgisi -----
                Console.WriteLine($"\n--- Token Bilgisi ---");
                Console.WriteLine($"Kullanılan token: {jsonResult.TokenUsed}");
                Console.WriteLine($"Kalan bakiye: {jsonResult.TokenBalance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
    }
}

