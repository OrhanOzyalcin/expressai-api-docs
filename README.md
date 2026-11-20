# ExpressAI External API Dokümantasyonu

ExpressAI veritabanındaki barkodları programatik olarak alın. Hangi çıktının döneceği kargoya göre değişir.

## Desteklenen barkodlar

| Kargo | pdf | image | rawJson | zpl | Not |
|-------|-----|-------|---------|-----|-----|
| Kolay Gelsin | var | var | var | var | Etiket alanları ayrıştırılır. |
| DHL eCommerce | var | var | yok | var | Görsel doğrudan ZPL'den üretilir. |
| Yurtiçi | var | var | yok | var | Görsel ZPL'den birleşik etikettir. |
| HepsiJet | var | var | yok | yok | Etiket JPG olarak gelir. ZPL üretilmez. |

`rawJson` veya `zpl` bu kargoda yoksa `pdf` ya da `image` isteyin. Aynı tipi tekrar denemeyin.

## Özellikler

| Özellik | Açıklama |
|---------|----------|
| **Hızlı Entegrasyon** | Tek bir API çağrısı ile 500'e kadar barkodu aynı anda alın |
| **Çıktı** | Kargo tablosundaki tipler. Hepsi her kargoda dönmez. |
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

Hangi tipin döneceği [Desteklenen barkodlar](#desteklenen-barkodlar) tablosundadır.

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

## Hata yanıtları

Başarılı cevap HTTP `200` döner. `errors` ve `errorRefs` dolu olabilir. Bu, isteğin tamamının başarısız olduğu anlamına gelmez; yalnızca listedeki referanslar atlanmıştır.

Her kodun gövdesi aşağıdaki şekillerden biridir.

### Şekil A — kod ve mesaj

`INVALID_API_KEY`, `INVALID_API_SECRET`, `CONNECTION_DISABLED`, `ACCOUNT_MISMATCH`, `INTERNAL_ERROR`, `INVALID_FILE_ID`, `FILE_NOT_FOUND`.

```json
{
  "success": false,
  "error": "Geçersiz API Key",
  "code": "INVALID_API_KEY"
}
```

`INSUFFICIENT_CREDITS` aynı şekildedir. Gövdeye `shortages` dizisi de eklenebilir.

### Şekil B — doğrulama

```json
{
  "success": false,
  "error": "Geçersiz istek formatı",
  "code": "VALIDATION_ERROR",
  "details": []
}
```

### Şekil C — hız sınırı

```json
{
  "success": false,
  "error": "Rate limit aşıldı. Dakikada maksimum 100 istek yapabilirsiniz.",
  "code": "RATE_LIMIT_EXCEEDED",
  "retryAfter": 42
}
```

Yanıtta `Retry-After`, `X-RateLimit-Limit`, `X-RateLimit-Remaining` ve `X-RateLimit-Reset` header'ları da bulunur.

### Şekil D — boş sonuç

`NO_PRINTABLE_ORDERS`, `NO_JSON_DATA`, `NO_ZPL_DATA`.

```json
{
  "success": false,
  "error": "JSON verisi olan sipariş bulunamadı",
  "code": "NO_JSON_DATA",
  "totalCount": 2,
  "successCount": 0,
  "tokenUsed": 0,
  "tokenBalance": 148,
  "errors": ["REF001: Barkod JSON verisi bulunamadı"],
  "errorRefs": ["REF001"]
}
```

### Şekil E — üretim hatası

```json
{
  "success": false,
  "error": "PDF oluşturulamadı",
  "code": "PDF_GENERATION_FAILED",
  "errors": []
}
```

### Şekil F — süresi dolmuş indirme

```json
{
  "success": false,
  "error": "İndirme bağlantısının süresi dolmuş",
  "code": "DOWNLOAD_EXPIRED",
  "expiredAt": "2024-12-23T16:30:00.000Z"
}
```

### Kodlar

| Kod | HTTP | Gövde | İstemci |
|-----|------|-------|---------|
| `VALIDATION_ERROR` | 400 | B | Gövdeyi düzeltin. Aynı isteği tekrarlamayın. |
| `INVALID_API_KEY` | 401 | A | Anahtarı düzeltin. Aynı kimlikle tekrarlamayın. |
| `INVALID_API_SECRET` | 401 | A | Secret'ı düzeltin. Aynı kimlikle tekrarlamayın. |
| `CONNECTION_DISABLED` | 401 | A | Bağlantı panelde kapalıdır. Açılmadan tekrarlamayın. |
| `ACCOUNT_MISMATCH` | 403 | A | URL'deki `accountId` anahtarın hesabı değil. URL'yi düzeltin. |
| `RATE_LIMIT_EXCEEDED` | 429 | C | `Retry-After` kadar bekleyip aynı isteği yeniden gönderin. |
| `INSUFFICIENT_CREDITS` | 400 | A | Kredi ekleyin. Döngüye girmeyin. |
| `NO_PRINTABLE_ORDERS` | 400 | D | `errors` içindeki referansların barkodu yok. Barkod oluşunca yeniden isteyin. |
| `NO_JSON_DATA` | 400 | D | Bu siparişlerde `rawJson` yok. `pdf` veya `image` isteyin. |
| `NO_ZPL_DATA` | 400 | D | Bu siparişlerde `zpl` yok. `pdf` veya `image` isteyin. |
| `PDF_GENERATION_FAILED` | 500 | E | Kısa aralıklarla sınırlı tekrar deneyin. |
| `ZIP_GENERATION_FAILED` | 500 | E | Kısa aralıklarla sınırlı tekrar deneyin. |
| `INTERNAL_ERROR` | 500 | A | Kısa aralıklarla sınırlı tekrar deneyin. |
| `INVALID_FILE_ID` | 400 | A | İndirme adresi bozuk. Aynı adresi tekrarlamayın. |
| `FILE_NOT_FOUND` | 404 | A | Dosya yok. Barkod isteğini yenileyip yeni `downloadUrl` alın. |
| `DOWNLOAD_EXPIRED` | 410 | F | Süre dolmuş. Barkod isteğini yenileyip yeni `downloadUrl` alın. |

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

