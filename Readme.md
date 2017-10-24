Fungsinya adalah sebagai peluncur perantara komunikasi antara [ViolaJonesHandler](https://github.com/esabook/Viola-Jones-Handler) dengan aplikasi [utama](https://github.com/esabook/Viola-Jones-Windows-Login) dan menghentikan proses *buit-in windows login user interface*.

Hal ini tidak disatukan dengan `ViolaJonesHandler.exe` karena Windows service berjalan dalam *delay* setelah system siap.

Sementara aplikasi utama `ViolaJones.exe` merupakan aplikasi dektop--yaitu yang dapat digunakan untuk mengatur konfigurasinya sendiri. 
 