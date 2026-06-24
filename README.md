# Rejestr pojazdów

Aplikacja desktopowa WPF z wykorzystaniem MVVM i EF Core do zarządzania rejestrem pojazdów.

## Funkcjonalności

- Pełny CRUD pojazdów
- Zarządzanie przeglądami i ubezpieczeniami
- Historia zmian pojazdu
- Wyszukiwanie po wszystkich kolumnach
- Automatyczny zapis edycji w DataGrid
- Walidacja danych

## Technologie

- C# / .NET 8
- WPF + XAML
- MVVM
- Entity Framework Core + SQLite
- LINQ

## Uruchomienie

Otwórz rozwiązanie w Visual Studio, skompiluj i uruchom. Baza SQLite zostanie utworzona automatycznie.

## Struktura

- Models – klasy domenowe i logika biznesowa
- ViewModels – ViewModele i komendy
- Views – okna XAML
- Styles – globalne style