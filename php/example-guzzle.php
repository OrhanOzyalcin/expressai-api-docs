<?php
/**
 * ExpressAI External Barcodes API - PHP Guzzle Örneği
 * 
 * Kurulum:
 *   composer require guzzlehttp/guzzle
 * 
 * Kullanım:
 *   php example-guzzle.php
 */

require_once __DIR__ . '/vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;

// =============================================================================
// İSTEMCİ SINIFI
// =============================================================================

class ExpressAIClient
{
    private Client $client;
    private string $accountId;
    private string $apiKey;
    private string $apiSecret;

    public function __construct(string $accountId, string $apiKey, string $apiSecret)
    {
        $this->accountId = $accountId;
        $this->apiKey = $apiKey;
        $this->apiSecret = $apiSecret;
        
        $this->client = new Client([
            'base_uri' => 'https://expressai.com.tr',
            'timeout' => 30,
            'headers' => [
                'Content-Type' => 'application/json',
                'Accept' => 'application/json'
            ]
        ]);
    }

    /**
     * Barkod verilerini al
     * 
     * @param array $referenceCodes Referans kodları
     * @param string $type Çıktı tipi: pdf, image, rawJson, zpl
     * @return array API yanıtı
     * @throws Exception
     */
    public function getBarcodes(array $referenceCodes, string $type = 'pdf'): array
    {
        try {
            $response = $this->client->post('/api/external-barcodes/' . $this->accountId, [
                'json' => [
                    'apiKey' => $this->apiKey,
                    'apiSecret' => $this->apiSecret,
                    'referenceCodes' => $referenceCodes,
                    'type' => $type
                ]
            ]);

            return json_decode($response->getBody()->getContents(), true);
        } catch (GuzzleException $e) {
            throw new Exception('API Error: ' . $e->getMessage());
        }
    }

    /**
     * PDF veya ZIP dosyasını indir
     * 
     * @param string $downloadUrl İndirme URL'i
     * @param string $outputPath Kayıt yolu
     * @return bool Başarılı mı
     */
    public function downloadFile(string $downloadUrl, string $outputPath): bool
    {
        try {
            $response = $this->client->get($downloadUrl);
            return file_put_contents($outputPath, $response->getBody()->getContents()) !== false;
        } catch (GuzzleException $e) {
            return false;
        }
    }
}

// =============================================================================
// YAPILANDIRMA - Bu değerleri kendi bilgilerinizle değiştirin
// =============================================================================

$accountId = '{accountId}';           // ExpressAI hesap ID'niz
$apiKey = 'EAC_xxxxxxxxxxxx';         // API anahtarınız
$apiSecret = 'your-secret-key';       // API gizli anahtarınız

// =============================================================================
// ÖRNEK KULLANIMLAR
// =============================================================================

$client = new ExpressAIClient($accountId, $apiKey, $apiSecret);

try {
    // ----- PDF olarak barkod alma -----
    echo "PDF olarak barkod alınıyor...\n";
    
    $pdfResult = $client->getBarcodes(['REF001', 'REF002'], 'pdf');
    
    if ($pdfResult['success']) {
        echo "PDF URL: " . $pdfResult['downloadUrl'] . "\n";
        echo "Geçerlilik: " . $pdfResult['expiresAt'] . "\n";
        
        // PDF'i indir
        if ($client->downloadFile($pdfResult['downloadUrl'], 'barkodlar.pdf')) {
            echo "PDF indirildi: barkodlar.pdf\n";
        }
    }

    // ----- ZIP (Image) olarak barkod alma -----
    echo "\nZIP olarak barkod alınıyor...\n";
    
    $zipResult = $client->getBarcodes(['REF001', 'REF002'], 'image');
    
    if ($zipResult['success']) {
        if ($client->downloadFile($zipResult['downloadUrl'], 'barkodlar.zip')) {
            echo "ZIP indirildi: barkodlar.zip\n";
        }
    }

    // ----- rawJson olarak kargo bilgilerini alma -----
    echo "\nrawJson olarak kargo bilgileri alınıyor...\n";
    
    $jsonResult = $client->getBarcodes(['REF001', 'REF002'], 'rawJson');
    
    if ($jsonResult['success']) {
        echo "\nAPI Response:\n";
        echo json_encode($jsonResult, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE) . "\n";
    }

    // ----- ZPL kodlarını alma -----
    echo "\nZPL kodları alınıyor...\n";
    
    $zplResult = $client->getBarcodes(['REF001'], 'zpl');
    
    if ($zplResult['success']) {
        foreach ($zplResult['data'] as $item) {
            echo "Referans: " . $item['referenceCode'] . "\n";
            echo "ZPL uzunluğu: " . strlen($item['zpl']) . " karakter\n";
        }
    }

    // ----- Token bilgisi -----
    echo "\n--- Token Bilgisi ---\n";
    echo "Kullanılan token: " . $jsonResult['tokenUsed'] . "\n";
    echo "Kalan bakiye: " . $jsonResult['tokenBalance'] . "\n";

    // ----- Hata durumunda -----
    if (!empty($jsonResult['errors'])) {
        echo "\nHatalar: " . implode(', ', $jsonResult['errors']) . "\n";
        echo "Hatalı referanslar: " . implode(', ', $jsonResult['errorRefs']) . "\n";
    }

} catch (Exception $e) {
    echo "Hata: " . $e->getMessage() . "\n";
}

