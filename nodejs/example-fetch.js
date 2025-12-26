/**
 * ExpressAI External Barcodes API - Node.js fetch API Örneği
 * 
 * Gereksinimler: Node.js 18+
 * 
 * Kullanım:
 *   node example-fetch.js
 */

const fs = require("fs");

const BASE_URL = "https://expressai.com.tr";

// =============================================================================
// YAPILANDIRMA - Bu değerleri kendi bilgilerinizle değiştirin
// =============================================================================

const CONFIG = {
  accountId: "{accountId}",           // ExpressAI hesap ID'niz
  apiKey: "EAC_xxxxxxxxxxxx",         // API anahtarınız
  apiSecret: "your-secret-key",       // API gizli anahtarınız
};

// =============================================================================
// API FONKSİYONLARI
// =============================================================================

/**
 * Barkod verilerini al
 * @param {string} accountId - Hesap ID
 * @param {Object} request - İstek parametreleri
 * @param {string} request.apiKey - API anahtarı
 * @param {string} request.apiSecret - API gizli anahtarı
 * @param {string[]} request.referenceCodes - Referans kodları
 * @param {string} request.type - Çıktı tipi: pdf, image, rawJson, zpl
 * @returns {Promise<Object>} API yanıtı
 */
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

/**
 * Dosya indir
 * @param {string} downloadUrl - İndirme URL'i
 * @param {string} outputPath - Kayıt yolu
 */
async function downloadFile(downloadUrl, outputPath) {
  const response = await fetch(BASE_URL + downloadUrl);
  
  if (!response.ok) {
    throw new Error("Dosya indirilemedi");
  }
  
  const buffer = await response.arrayBuffer();
  fs.writeFileSync(outputPath, Buffer.from(buffer));
  console.log(`Dosya indirildi: ${outputPath}`);
}

// =============================================================================
// ÖRNEK KULLANIMLAR
// =============================================================================

async function main() {
  const { accountId, apiKey, apiSecret } = CONFIG;
  
  try {
    // ----- PDF olarak barkod alma -----
    console.log("PDF olarak barkod alınıyor...");
    
    const pdfResult = await getBarcodes(accountId, {
      apiKey,
      apiSecret,
      referenceCodes: ["REF001", "REF002"],
      type: "pdf"
    });
    
    if (pdfResult.success && pdfResult.downloadUrl) {
      console.log("PDF URL:", BASE_URL + pdfResult.downloadUrl);
      console.log("Geçerlilik:", pdfResult.expiresAt);
      
      // PDF'i indir
      await downloadFile(pdfResult.downloadUrl, "barkodlar.pdf");
    }
    
    // ----- rawJson olarak kargo bilgilerini alma -----
    console.log("\nrawJson olarak kargo bilgileri alınıyor...");
    
    const jsonResult = await getBarcodes(accountId, {
      apiKey,
      apiSecret,
      referenceCodes: ["REF001", "REF002"],
      type: "rawJson"
    });
    
    if (jsonResult.success) {
      console.log("\nAPI Response:");
      console.log(JSON.stringify(jsonResult, null, 2));
    }
    
    // ----- ZPL kodlarını alma (Zebra yazıcı için) -----
    console.log("\nZPL kodları alınıyor...");
    
    const zplResult = await getBarcodes(accountId, {
      apiKey,
      apiSecret,
      referenceCodes: ["REF001"],
      type: "zpl"
    });
    
    if (zplResult.success && zplResult.data) {
      for (const item of zplResult.data) {
        console.log("Referans:", item.referenceCode);
        console.log("ZPL:", item.zpl?.substring(0, 100) + "...");
        // Zebra yazıcıya göndermek için:
        // await sendToZebraPrinter(item.zpl);
      }
    }
    
    // ----- Token bilgisi -----
    console.log("\n--- Token Bilgisi ---");
    console.log("Kullanılan token:", jsonResult.tokenUsed);
    console.log("Kalan bakiye:", jsonResult.tokenBalance);
    
  } catch (error) {
    console.error("Hata:", error.message);
  }
}

main();

