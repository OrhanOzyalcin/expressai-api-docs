<?php
/**
 * ExpressAI External Barcodes API - PHP cURL Örneği
 * 
 * Gereksinimler:
 *   - PHP 7.4+
 *   - cURL extension
 * 
 * Kullanım:
 *   php example-curl.php
 */

// =============================================================================
// İSTEMCİ SINIFI
// =============================================================================

class ExpressAIBarcodeClient
{
    private string $baseUrl = 'https://expressai.com.tr';
    private string $accountId;
    private string $apiKey;
    private string $apiSecret;

    public function __construct(string $accountId, string $apiKey, string $apiSecret)
    {
        $this->accountId = $accountId;
        $this->apiKey = $apiKey;
        $this->apiSecret = $apiSecret;
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
        $url = $this->baseUrl . '/api/external-barcodes/' . $this->accountId;
        
        $data = [
            'apiKey' => $this->apiKey,
            'apiSecret' => $this->apiSecret,
            'referenceCodes' => $referenceCodes,
            'type' => $type
        ];

        $ch = curl_init($url);
        curl_setopt_array($ch, [
            CURLOPT_POST => true,
            CURLOPT_POSTFIELDS => json_encode($data),
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_HTTPHEADER => [
                'Content-Type: application/json',
                'Accept: application/json'
            ],
            CURLOPT_TIMEOUT => 30
        ]);

        $response = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $error = curl_error($ch);
        curl_close($ch);

        if ($error) {
            throw new Exception('cURL Error: ' . $error);
        }

        $result = json_decode($response, true);

        if ($httpCode !== 200) {
            throw new Exception($result['error'] ?? 'API request failed', $httpCode);
        }

        return $result;
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
        $url = $this->baseUrl . $downloadUrl;
        
        $ch = curl_init($url);
        curl_setopt_array($ch, [
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_FOLLOWLOCATION => true,
            CURLOPT_TIMEOUT => 60
        ]);

        $content = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);

        if ($httpCode !== 200) {
            return false;
        }

        return file_put_contents($outputPath, $content) !== false;
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

$client = new ExpressAIBarcodeClient($accountId, $apiKey, $apiSecret);

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

    // ----- rawJson olarak kargo bilgilerini alma -----
    echo "\nrawJson olarak kargo bilgileri alınıyor...\n";
    
    $jsonResult = $client->getBarcodes(['REF001', 'REF002'], 'rawJson');
    
    if ($jsonResult['success']) {
        echo "\nAPI Response:\n";
        echo json_encode($jsonResult, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE) . "\n";
    }

    // ----- ZPL kodlarını alma (Zebra yazıcı için) -----
    echo "\nZPL kodları alınıyor...\n";
    
    $zplResult = $client->getBarcodes(['REF001'], 'zpl');
    
    if ($zplResult['success']) {
        foreach ($zplResult['data'] as $item) {
            echo "Referans: " . $item['referenceCode'] . "\n";
            echo "ZPL uzunluğu: " . strlen($item['zpl']) . " karakter\n";
            
            // Zebra yazıcıya gönderme örneği (socket)
            // $socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);
            // socket_connect($socket, '192.168.1.100', 9100);
            // socket_write($socket, $item['zpl']);
            // socket_close($socket);
        }
    }

    // ----- Token bilgisi -----
    echo "\n--- Token Bilgisi ---\n";
    echo "Kullanılan token: " . $jsonResult['tokenUsed'] . "\n";
    echo "Kalan bakiye: " . $jsonResult['tokenBalance'] . "\n";

} catch (Exception $e) {
    echo "Hata: " . $e->getMessage() . "\n";
}

