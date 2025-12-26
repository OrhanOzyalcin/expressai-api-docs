/**
 * ExpressAI External Barcodes API - Node.js axios Örneği
 * 
 * Kurulum:
 *   npm install axios
 * 
 * Kullanım:
 *   node example-axios.js
 */

const axios = require("axios");
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
// API İSTEMCİSİ
// =============================================================================

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000, // 30 saniye
});

/**
 * Barkod verilerini al
 * @param {string} accountId - Hesap ID
 * @param {string} apiKey - API anahtarı
 * @param {string} apiSecret - API gizli anahtarı
 * @param {string[]} referenceCodes - Referans kodları
 * @param {string} type - Çıktı tipi: pdf, image, rawJson, zpl
 * @returns {Promise<Object>} API yanıtı
 */
async function getBarcodes(accountId, apiKey, apiSecret, referenceCodes, type = "pdf") {
  try {
    const { data } = await api.post(`/api/external-barcodes/${accountId}`, {
      apiKey,
      apiSecret,
      referenceCodes,
      type,
    });

    return data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const errorData = error.response?.data;
      throw new Error(errorData?.error || error.message);
    }
    throw error;
  }
}

/**
 * Dosya indir
 * @param {string} downloadUrl - İndirme URL'i
 * @param {string} outputPath - Kayıt yolu
 */
async function downloadFile(downloadUrl, outputPath) {
  const response = await api.get(downloadUrl, {
    responseType: "arraybuffer",
  });
  
  fs.writeFileSync(outputPath, response.data);
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
    
    const pdfResult = await getBarcodes(
      accountId, 
      apiKey, 
      apiSecret, 
      ["REF001", "REF002"], 
      "pdf"
    );
    
    if (pdfResult.success && pdfResult.downloadUrl) {
      console.log("PDF URL:", BASE_URL + pdfResult.downloadUrl);
      console.log("Geçerlilik:", pdfResult.expiresAt);
      
      // PDF'i indir
      await downloadFile(pdfResult.downloadUrl, "barkodlar.pdf");
    }
    
    // ----- ZIP (Image) olarak barkod alma -----
    console.log("\nZIP olarak barkod alınıyor...");
    
    const zipResult = await getBarcodes(
      accountId, 
      apiKey, 
      apiSecret, 
      ["REF001", "REF002"], 
      "image"
    );
    
    if (zipResult.success && zipResult.downloadUrl) {
      await downloadFile(zipResult.downloadUrl, "barkodlar.zip");
    }
    
    // ----- rawJson olarak kargo bilgilerini alma -----
    console.log("\nrawJson olarak kargo bilgileri alınıyor...");
    
    const jsonResult = await getBarcodes(
      accountId, 
      apiKey, 
      apiSecret, 
      ["REF001", "REF002"], 
      "rawJson"
    );
    
    if (jsonResult.success) {
      console.log("\nAPI Response:");
      console.log(JSON.stringify(jsonResult, null, 2));
    }
    
    // ----- ZPL kodlarını alma -----
    console.log("\nZPL kodları alınıyor...");
    
    const zplResult = await getBarcodes(
      accountId, 
      apiKey, 
      apiSecret, 
      ["REF001"], 
      "zpl"
    );
    
    if (zplResult.success && zplResult.data) {
      for (const item of zplResult.data) {
        console.log("Referans:", item.referenceCode);
        console.log("ZPL uzunluğu:", item.zpl?.length, "karakter");
      }
    }
    
    // ----- Token bilgisi -----
    console.log("\n--- Token Bilgisi ---");
    console.log("Kullanılan token:", jsonResult.tokenUsed);
    console.log("Kalan bakiye:", jsonResult.tokenBalance);
    
    // ----- Hata durumunda -----
    if (jsonResult.errors?.length > 0) {
      console.log("\nHatalar:", jsonResult.errors);
      console.log("Hatalı referanslar:", jsonResult.errorRefs);
    }
    
  } catch (error) {
    console.error("Hata:", error.message);
  }
}

main();

