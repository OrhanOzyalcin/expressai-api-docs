# ExpressAI External API Dokümantasyonu

> **Desteklenen Entegrasyonlar**  
> **Trendyol pazaryeri siparişleri** ve **Kolay Gelsin** barkodları desteklenir. Kolay Gelsin `pdf`, `image`, `rawJson` ve `zpl` üretmeye devam eder.  
> **DHL eCommerce** siparişinde barkod etiketten ayrıştırılmaz. Görsel doğrudan ZPL'den üretilir ve dosya yolu `base64BarcodePath` alanına yazılır. Bu siparişlerde `pdf`, `image` ve `zpl` döner; `rawJson` üretilmez.

ExpressAI barkod API'sini kullanarak sipariş barkodlarınızı programatik olarak alın. PDF, görsel, JSON veya ZPL formatlarında.

## Özellikler

| Özellik | Açıklama |
|---------|----------|
| **Hızlı Entegrasyon** | Tek bir API çağrısı ile 500'e kadar barkodu aynı anda alın |
| **Çoklu Format** | PDF, görsel (ZIP), JSON veya ZPL formatlarında çıktı alın |
| **Güvenli Erişim** | API Key ve Secret ile güvenli kimlik doğrulama |

## Başlarken

1. ExpressAI hesabınıza giriş yapın veya yeni hesap oluşturun
2. Ayarlar sayfasından **External API** bölümüne gidin ve yeni bir API bağlantısı oluşturun
3. API Key ve Secret bilgilerinizi kopyalayın ve uygulamanıza entegre edin
4. Sipariş referans kodlarınızı göndererek barkodlarınızı alın!

## API Endpoint

```
POST https://expressai.com.tr/api/external-barcodes/{accountId}
```

## Request Body

```json
{
  "apiKey": "EAC_xxxxxxxxxxxx",
  "apiSecret": "your-secret-key",
  "referenceCodes": ["REF001", "REF002", "REF003"],
  "type": "pdf"
}
```

### Parametreler

| Parametre | Tip | Açıklama |
|-----------|-----|----------|
| `apiKey` | string | API anahtarınız |
| `apiSecret` | string | API gizli anahtarınız |
| `referenceCodes` | string[] | Barkod alınacak sipariş referans kodları (maksimum 500) |
| `type` | string | Çıktı tipi: `pdf`, `image`, `rawJson`, `zpl` |

### Çıktı Tipleri

| Tip | Açıklama |
|-----|----------|
| `pdf` | Tüm barkodları içeren tek bir PDF dosyası |
| `image` | Her barkod için ayrı görsel içeren ZIP dosyası |
| `rawJson` | Parse edilmiş kargo bilgileri (JSON formatında) |
| `zpl` | Ham ZPL kodları (Zebra yazıcılar için) |

**DHL eCommerce:** Barkod etiketten ayrıştırılmaz. Görsel doğrudan ZPL'den üretilir ve dosya yolu `base64BarcodePath` alanına yazılır. Bu siparişlerde `pdf`, `image` ve `zpl` döner; `rawJson` üretilmez. Kolay Gelsin siparişlerinde `pdf`, `image`, `rawJson` ve `zpl` aynı şekilde çalışmaya devam eder.

## Response Formatları

### PDF Response

```json
{
  "success": true,
  "type": "pdf",
  "downloadUrl": "/api/external-barcodes/download/{accountId}/pdf-3-1703345678-abc123",
  "expiresAt": "2024-12-23T15:30:00.000Z",
  "fileName": "pdf-3-1703345678-abc123.pdf",
  "totalCount": 3,
  "successCount": 3,
  "tokenUsed": 2,
  "tokenBalance": 148,
  "errors": [],
  "errorRefs": []
}
```

### ZIP (Image) Response

```json
{
  "success": true,
  "type": "zip",
  "downloadUrl": "/api/external-barcodes/download/{accountId}/zip-3-1703345678-def456",
  "expiresAt": "2024-12-23T15:30:00.000Z",
  "fileName": "zip-3-1703345678-def456.zip",
  "totalCount": 3,
  "successCount": 3,
  "tokenUsed": 2,
  "tokenBalance": 148,
  "errors": [],
  "errorRefs": []
}
```

> **Not:** ZIP dosyası içinde her barkod görseli referans kodu ile adlandırılır (örn: REF001.jpg, REF002.jpg)

### rawJson Response

```json
{
  "success": true,
  "type": "rawJson",
  "totalCount": 2,
  "successCount": 2,
  "tokenUsed": 1,
  "tokenBalance": 149,
  "data": [
    {
      "referenceCode": "REF001",
      "fields": {
        "referenceNumber": "888123456",
        "senderInfo": "Gönderici Bilgisi",
        "senderName": "Gönderici Adı",
        "distributionCenter": "Dağıtım Merkezi",
        "receiverName": "Alıcı Adı",
        "receiverAddress": "Alıcı Adresi",
        "receiverCityDistrict": "İstanbul / Kadıköy",
        "receiverPhone": "5321234567",
        "trackingNumber": "123456789012",
        "shipmentDate": "2024-12-24",
        "paymentType": "Kapıda Ödeme",
        "desiCount": "1"
      }
    }
  ],
  "errors": [],
  "errorRefs": []
}
```

### rawJson Field Açıklamaları

| Field | Açıklama |
|-------|----------|
| `referenceNumber` | Referans numarası |
| `senderInfo` | Gönderici bilgisi |
| `senderName` | Gönderici adı |
| `distributionCenter` | Dağıtım merkezi |
| `receiverLabel` | Alıcı etiketi |
| `receiverName` | Alıcı adı |
| `receiverAddress` | Alıcı adresi (1. satır) |
| `receiverAddress2` | Alıcı adresi (2. satır) |
| `receiverAddress3` | Alıcı adresi (3. satır) |
| `receiverCityDistrict` | İl/İlçe |
| `receiverPhone` | Alıcı telefon |
| `qrCodeData` | QR kod verisi |
| `trackingNumber` | Takip numarası |
| `shipmentDate` | Gönderim tarihi |
| `paymentType` | Ödeme tipi |
| `deliveryType` | Teslimat tipi |
| `desiCount` | Desi miktarı |
| `desiCountText` | Desi yazısı |
| `lastDeliveryDate` | Son dağıtım tarihi |
| `courierZone` | Kurye bölgesi |
| `distributionCenter270` | Dağıtım merkezi (270) |
| `distributionLine` | Dağıtım hattı |
| `distributionZone` | Dağıtım bölgesi |

### ZPL Response

```json
{
  "success": true,
  "type": "zpl",
  "totalCount": 2,
  "successCount": 2,
  "tokenUsed": 1,
  "tokenBalance": 149,
  "data": [
    {
      "referenceCode": "REF001",
      "zpl": "^XA^FO50,50^A0N,50,50^FD...^FS^XZ"
    }
  ],
  "errors": [],
  "errorRefs": []
}
```

> **Not:** Ham ZPL kodları döndürülür, doğrudan Zebra yazıcılara gönderilebilir.

## Dosya İndirme

PDF ve ZIP dosyaları için response'da dönen `downloadUrl` adresine GET isteği yaparak dosyayı indirebilirsiniz.

```
GET https://expressai.com.tr{downloadUrl}
```

> ⚠️ **Önemli:** İndirme bağlantıları **1 saat** geçerlidir. Süre dolduktan sonra erişilemez.

## Hata Kodları

| Kod | Açıklama |
|-----|----------|
| `INVALID_API_KEY` | Geçersiz API Key |
| `INVALID_API_SECRET` | Geçersiz API Secret |
| `CONNECTION_DISABLED` | Bağlantı pasif durumda |
| `RATE_LIMIT_EXCEEDED` | Rate limit aşıldı (100 istek/dk) |
| `INSUFFICIENT_CREDITS` | Yetersiz kredi bakiyesi |
| `NO_PRINTABLE_ORDERS` | Yazdırılabilir sipariş yok |
| `NO_JSON_DATA` | rawJson verisi olan sipariş yok |
| `NO_ZPL_DATA` | ZPL verisi olan sipariş yok |
| `DOWNLOAD_EXPIRED` | İndirme bağlantısının süresi dolmuş |
| `FILE_NOT_FOUND` | Dosya bulunamadı |

## Rate Limiting

API Key başına dakikada maksimum **100 istek** yapılabilir.

Response header'larında rate limit bilgileri bulunur:

| Header | Açıklama |
|--------|----------|
| `X-RateLimit-Limit` | Toplam limit |
| `X-RateLimit-Remaining` | Kalan istek hakkı |
| `X-RateLimit-Reset` | Reset zamanı (Unix timestamp) |

## Programlama Dili Örnekleri

Kullandığınız programlama diline göre detaylı dokümantasyon ve örnek kodlar:

| Dil | Dokümantasyon | Özellikler |
|-----|---------------|------------|
| [Node.js / TypeScript](./nodejs/) | fetch API, axios örnekleri | TypeScript tip tanımları dahil |
| [PHP](./php/) | cURL, Guzzle örnekleri | Laravel entegrasyonu dahil |
| [C# / .NET](./csharp/) | HttpClient örnekleri | Zebra yazıcı gönderimi dahil |

## Lisans

Bu API ExpressAI tarafından sağlanmaktadır. Kullanım koşulları için [expressai.com.tr](https://expressai.com.tr) adresini ziyaret edin.

