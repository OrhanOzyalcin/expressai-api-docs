# ExpressAI API - Node.js / TypeScript

Node.js ve TypeScript için ExpressAI External Barcodes API entegrasyonu.

## Gereksinimler

- Node.js 18+ (fetch API için) veya Node.js 14+ (axios kullanımı için)
- TypeScript 4.5+ (opsiyonel)

## Kurulum

### fetch API (Node.js 18+)

Node.js 18 ve üzeri sürümlerde fetch API yerleşik olarak gelir, ek kurulum gerekmez.

### axios Kullanımı

```bash
npm install axios
```

## TypeScript Tip Tanımları

```typescript
interface BarcodeRequest {
  apiKey: string;
  apiSecret: string;
  referenceCodes: string[];
  type: "pdf" | "image" | "rawJson" | "zpl";
}

interface BarcodeResponse {
  success: boolean;
  type: string;
  downloadUrl?: string;
  expiresAt?: string;
  fileName?: string;
  data?: Array<{
    referenceCode: string;
    fields?: Record<string, string>;
    zpl?: string;
  }>;
  totalCount: number;
  successCount: number;
  tokenUsed: number;
  tokenBalance: number;
  errors: string[];
  errorRefs: string[];
}
```

## Kullanım

### fetch API ile

```javascript
const BASE_URL = "https://expressai.com.tr";

async function getBarcodes(accountId, request) {
  const response = await fetch(
    `${BASE_URL}/api/external-barcodes/${accountId}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    }
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.error || "API request failed");
  }

  return response.json();
}

// PDF olarak barkod alma
const pdfResult = await getBarcodes(accountId, {
  apiKey: "EAC_xxxxxxxxxxxx",
  apiSecret: "your-secret-key",
  referenceCodes: ["REF001", "REF002"],
  type: "pdf"
});

if (pdfResult.success && pdfResult.downloadUrl) {
  console.log("PDF URL:", BASE_URL + pdfResult.downloadUrl);
  
  // PDF'i indir
  const pdfResponse = await fetch(BASE_URL + pdfResult.downloadUrl);
  const pdfBuffer = await pdfResponse.arrayBuffer();
  // fs.writeFileSync("barkodlar.pdf", Buffer.from(pdfBuffer));
}
```

### axios ile

```javascript
import axios from "axios";

const api = axios.create({
  baseURL: "https://expressai.com.tr",
  headers: {
    "Content-Type": "application/json",
  },
});

async function getBarcodes(accountId, apiKey, apiSecret, referenceCodes, type = "pdf") {
  const { data } = await api.post(`/api/external-barcodes/${accountId}`, {
    apiKey,
    apiSecret,
    referenceCodes,
    type,
  });
  return data;
}

// rawJson ile kargo bilgilerini al
const jsonResult = await getBarcodes(accountId, apiKey, apiSecret, ["REF001"], "rawJson");
console.log(jsonResult.data);
```

## Örnek Dosyalar

| Dosya | Açıklama |
|-------|----------|
| [example-fetch.js](./example-fetch.js) | fetch API ile tam örnek |
| [example-axios.js](./example-axios.js) | axios ile tam örnek |

## Önemli Notlar

- İndirme bağlantıları **1 saat** geçerlidir
- Maksimum **500** referans kodu gönderilebilir
- Rate limit: **100 istek/dakika**

## Hata Yönetimi

```javascript
try {
  const result = await getBarcodes(accountId, request);
  // Başarılı işlem
} catch (error) {
  console.error("Hata:", error.message);
  // Hata kodları: INVALID_API_KEY, RATE_LIMIT_EXCEEDED, vb.
}
```

## Daha Fazla Bilgi

Genel API dokümantasyonu için [ana README](../README.md) dosyasına bakın.

