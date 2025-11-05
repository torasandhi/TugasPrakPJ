Anggota Kelompok : 
- Oscar Javier Abdullah (5223600001)
- Rangga Raisha Syaputra (5223600007)
- Tora Sandhi Kamulian (5223600013)

# Laporan Proyek Praktikum Pemrograman Jaringan Komputer: Integrasi REST API (menggunakan UGS)

### Pendahuluan

Tugas ini bertujuan untuk merancang dan membangun sebuah mini game di
Unity yang terintegrasi dengan REST API dan database. Sesuai dengan
spesifikasi tugas, sistem yang dibangun harus mampu:

-   Merancang REST API kustom.
-   Memanfaatkan API tersebut untuk menyimpan informasi yang dibutuhkan
    (skor, data pemain).
-   Menyimpan status terakhir game (game state).
-   Memuat ulang (resume) status terakhir game saat pemain kembali.

Untuk memenuhi persyaratan ini, proyek ini dikembangkan menggunakan
**Unity Engine** sebagai game client dan **Unity Gaming Services (UGS)**
sebagai backend-as-a-service (BaaS). UGS dipilih karena menyediakan
solusi lengkap yang mencakup autentikasi, database, dan lapisan API
kustom dalam satu ekosistem yang terintegrasi.

### Arsitektur Solusi

Arsitektur sistem ini dibagi menjadi dua komponen utama: **Client (Game
Unity)** dan **Backend (Unity Gaming Services)**.

#### Client (Game Unity)

Bertanggung jawab atas logika game, input pemain, dan visual. Game ini
tidak pernah terhubung langsung ke database.

#### Backend (UGS)

Bertanggung jawab atas semua logika sisi server dan penyimpanan data.

-   **Unity Authentication**: Mengidentifikasi setiap pemain secara unik
    (melalui `AuthenticationManager.cs`).
-   **UGS Cloud Save**: Bertindak sebagai "Database" (basis data
    key-value di cloud).
-   **UGS Cloud Code (C# Modules)**: Bertindak sebagai "REST API" yang
    menjembatani komunikasi antara game (Client) dan database (Cloud
    Save).

#### Alur Komunikasi

1.  Game Client (Unity) memanggil sebuah endpoint di Cloud Code
    (misalnya `PUT_PlayerData`).
2.  Cloud Code (REST API) menerima panggilan tersebut, memvalidasi data,
    dan menjalankan logika C# di server.
3.  Cloud Code menggunakan API internal UGS untuk menyimpan data ke
    Cloud Save (Database).

------------------------------------------------------------------------

### 1: Perancangan "Database" (UGS Cloud Save)

Sebagai pengganti database SQL tradisional, **UGS Cloud Save** digunakan
sebagai database NoSQL berbasis key-value. Berdasarkan analisis file
`PlayerScoreApiModule.cs`, database ini menyimpan data berikut untuk
setiap pemain:

  -------------------------------------------------------------------------------------
  Key                        Deskripsi                      Tipe Data
  -------------------------- ------------------------------ ---------------------------
  `playerScore`              Menyimpan skor tertinggi       int
                             pemain                         

  `lastUsedCharacterIndex`   Indeks karakter terakhir yang  int
                             dipilih                        

  `lastUsedCharacter`        Nama karakter terakhir yang    String
                             dipilih                        
  -------------------------------------------------------------------------------------

Data ini disimpan secara aman dan terikat pada `PlayerId` yang diperoleh
dari layanan Unity Authentication.

------------------------------------------------------------------------

### 2: Perancangan "REST API" (UGS Cloud Code)

Persyaratan untuk "merancang REST API" dipenuhi melalui implementasi
**UGS Cloud Code C# Module**. File `PlayerScoreApiModule.cs`
mendefinisikan logika API di sisi server.

#### Endpoint 1: `[CloudCodeFunction("PUT_PlayerData")]`

-   **Deskripsi:** Endpoint utama untuk menyimpan state game.
-   **Dipanggil oleh:** `GameOver.cs` dan `CharacterSelector.cs`.
-   **Input:** `score`, `characterIndex`, `characterName`.
-   **Proses:** Menulis semua data ke Cloud Save menggunakan
    `SetItemBatchAsync`.

#### Endpoint 2: `[CloudCodeFunction("GET_PlayerData")]`

-   **Deskripsi:** Endpoint utama untuk memuat (resume) state.
-   **Dipanggil oleh:** `Player.cs` dan `CharacterSelector.cs`.
-   **Proses:** Mengambil data `playerScore`, `lastUsedCharacterIndex`,
    dan `lastUsedCharacter` dari Cloud Save menggunakan `GetItemsAsync`.
-   **Output:** Objek `PlayerData` yang berisi data yang tersimpan atau
    nilai default.

Endpoint ini berfungsi sebagai REST API aman yang dikontrol penuh
melalui Cloud Code.

------------------------------------------------------------------------

### 3: Integrasi pada Game Client (Unity)

Game client tidak memanggil HTTP request manual. UGS menyediakan
**Generated Bindings** (`CloudSaveBindings.cs`) yang memungkinkan
pemanggilan API seperti fungsi C# biasa.

#### A. Inisialisasi dan Autentikasi (`AuthenticationManager.cs`)

-   Menginisialisasi UnityServices.
-   Login pemain secara anonim (`SignInAnonymouslyAsync`).
-   Menginisialisasi `CloudSaveBindings` setelah login berhasil.

#### B. Menyimpan Game State

-   **Skor:** Saat `GameOver.cs`, fungsi
    `ScoreManager.Instance.SaveHighScore()` memanggil:

    ``` csharp
    await _cloudModule.PUT_PlayerData(_highScore, characterIndex, characterName);
    ```

-   **Karakter:** Saat memilih karakter di `CharacterSelector.cs`,
    fungsi `SaveSelectedCharacterAsync()` memanggil endpoint yang sama.

#### C. Memuat Game State (Resume)

-   `CharacterSelector.cs` memanggil `LoadSavedCharacterData()`:

    ``` csharp
    var data = await cloudModule.GET_PlayerData();
    if (data != null) {
        selectedIndex = data.LastUsedCharacterIndex;
        selectedName = data.lastUsedCharacterName;
    }
    ```

-   Data ini digunakan untuk menampilkan karakter terakhir pemain di UI
    dan gameplay.
