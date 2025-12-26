# ExpressAI API - PHP

PHP için ExpressAI External Barcodes API entegrasyonu.

## Gereksinimler

- PHP 7.4+
- cURL extension (genellikle varsayılan olarak yüklüdür)
- JSON extension (genellikle varsayılan olarak yüklüdür)

### Guzzle için (Opsiyonel)

```bash
composer require guzzlehttp/guzzle
```

## Kullanım

### cURL ile

```php
<?php
$accountId = '{accountId}';
$apiKey = 'EAC_xxxxxxxxxxxx';
$apiSecret = 'your-secret-key';

$url = "https://expressai.com.tr/api/external-barcodes/{$accountId}";

$data = [
    'apiKey' => $apiKey,
    'apiSecret' => $apiSecret,
    'referenceCodes' => ['REF001', 'REF002'],
    'type' => 'pdf'
];

$ch = curl_init($url);
curl_setopt_array($ch, [
    CURLOPT_POST => true,
    CURLOPT_POSTFIELDS => json_encode($data),
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_HTTPHEADER => [
        'Content-Type: application/json',
        'Accept: application/json'
    ]
]);

$response = curl_exec($ch);
$result = json_decode($response, true);
curl_close($ch);

if ($result['success']) {
    echo "Download URL: " . $result['downloadUrl'] . "\n";
}
```

### Guzzle ile

```php
<?php
use GuzzleHttp\Client;

$client = new Client([
    'base_uri' => 'https://expressai.com.tr',
    'timeout' => 30
]);

$response = $client->post("/api/external-barcodes/{$accountId}", [
    'json' => [
        'apiKey' => $apiKey,
        'apiSecret' => $apiSecret,
        'referenceCodes' => ['REF001', 'REF002'],
        'type' => 'rawJson'
    ]
]);

$result = json_decode($response->getBody()->getContents(), true);
print_r($result['data']);
```

### Laravel ile

```php
<?php
use Illuminate\Support\Facades\Http;

$response = Http::post("https://expressai.com.tr/api/external-barcodes/{$accountId}", [
    'apiKey' => $apiKey,
    'apiSecret' => $apiSecret,
    'referenceCodes' => ['REF001'],
    'type' => 'rawJson'
]);

$data = $response->json();
```

## Örnek Dosyalar

| Dosya | Açıklama |
|-------|----------|
| [example-curl.php](./example-curl.php) | cURL ile tam istemci sınıfı |
| [example-guzzle.php](./example-guzzle.php) | Guzzle ile tam istemci sınıfı |

## Önemli Notlar

- İndirme bağlantıları **1 saat** geçerlidir
- Maksimum **500** referans kodu gönderilebilir
- Rate limit: **100 istek/dakika**

## Hata Yönetimi

```php
<?php
try {
    $result = $client->getBarcodes(['REF001'], 'pdf');
    // Başarılı işlem
} catch (Exception $e) {
    echo "Hata: " . $e->getMessage();
    // Hata kodları: INVALID_API_KEY, RATE_LIMIT_EXCEEDED, vb.
}
```

## Zebra Yazıcıya Gönderme (ZPL)

```php
<?php
// ZPL kodlarını al
$result = $client->getBarcodes(['REF001'], 'zpl');

if ($result['success']) {
    foreach ($result['data'] as $item) {
        $zpl = $item['zpl'];
        
        // Socket ile Zebra yazıcıya gönder
        $socket = socket_create(AF_INET, SOCK_STREAM, SOL_TCP);
        socket_connect($socket, '192.168.1.100', 9100);
        socket_write($socket, $zpl);
        socket_close($socket);
    }
}
```

## Daha Fazla Bilgi

Genel API dokümantasyonu için [ana README](../README.md) dosyasına bakın.

