# MeteoWeaver

## Opis projektu
MeteoWeaver to aplikacja wykonana w technologii **.NET 8**, która pełni rolę inteligentnego łącznika pomiędzy zewnętrznym publicznym API a lokalną bazą danych.

W ramach zadania rekrutacyjnego aplikacja pobiera dane pogodowe z publicznego API **Open-Meteo**, zapisuje je w bazie **SQLite**, a następnie udostępnia własną funkcjonalność w postaci analizy komfortu pogodowego dla wybranego miasta.

Projekt realizuje założenia zadania **Public API Data Weaver**:
- integruje się z zewnętrznym publicznym API,
- utrwala dane w bazie danych,
- wystawia własne endpointy HTTP,
- nadaje danym dodatkową wartość poprzez autorskie przetwarzanie.

## Wybrane źródło danych
Źródłem danych jest **Open-Meteo** – publiczne API pogodowe, które pozwala pobierać m.in.:
- bieżącą temperaturę,
- prędkość wiatru,
- kod pogody,
- dane godzinowe,
- prognozę dla kolejnych godzin.

Dodatkowo wykorzystywany jest endpoint geokodowania, dzięki któremu aplikacja potrafi odnaleźć współrzędne dla miasta podanego przez użytkownika.

## Główna idea rozwiązania
Użytkownik wywołuje endpoint importu dla wybranego miasta. Aplikacja:
1. wyszukuje miasto w zewnętrznym API,
2. pobiera aktualne dane pogodowe i prognozę godzinową,
3. zapisuje dane jako snapshot w lokalnej bazie SQLite,
4. wylicza autorski **Comfort Score**, czyli uproszczony wskaźnik komfortu pogodowego,
5. udostępnia przetworzone dane przez własne endpointy.

Dzięki temu aplikacja nie jest wyłącznie prostym proxy do zewnętrznego API, ale samodzielnie buduje lokalny zasób danych i dostarcza dodatkową logikę biznesową.

## Autorska funkcjonalność
Najważniejszym elementem autorskim jest **Comfort Score**.

Wskaźnik ten wyliczany jest na podstawie:
- temperatury,
- opadów,
- prędkości wiatru,
- kodu pogody.

Na podstawie tych danych aplikacja potrafi:
- wskazać najlepsze godziny na aktywność na zewnątrz,
- przygotować krótkie podsumowanie komfortu pogodowego,
- porównać najnowszy zapis z poprzednim snapshotem dla tego samego miasta.

## Technologie
Projekt wykorzystuje:
- **.NET 8**
- **ASP.NET Core Minimal API**
- **Entity Framework Core**
- **SQLite**
- **Swagger / OpenAPI**
- **xUnit**

## Struktura projektu
Repozytorium zawiera dwa projekty:

### `src/MeteoWeaver.Api`
Główna aplikacja web API.

### `tests/MeteoWeaver.Tests`
Projekt testowy zawierający przykładowe testy jednostkowe dla logiki wyliczającej Comfort Score.

## Dostępne endpointy
### 1. Import danych pogodowych
**POST** `/api/weather/import/{city}`

Przykład:
```http
POST /api/weather/import/Warsaw
```

Działanie:
- pobiera dane dla miasta,
- zapisuje nowy snapshot w bazie,
- wylicza Comfort Score,
- zwraca podsumowanie importu.

### 2. Pobranie najnowszego podsumowania miasta
**GET** `/api/weather/{city}/summary`

Przykład:
```http
GET /api/weather/Warsaw/summary
```

Działanie:
- zwraca najnowszy snapshot dla miasta,
- pokazuje bieżące dane,
- zwraca trzy najlepsze godziny według Comfort Score,
- pokazuje trend względem poprzedniego zapisu.

### 3. Historia importów dla miasta
**GET** `/api/weather/{city}/history`

Przykład:
```http
GET /api/weather/Warsaw/history
```

Działanie:
- zwraca historię zapisanych snapshotów dla miasta.

### 4. Usunięcie danych miasta
**DELETE** `/api/weather/{city}`

Przykład:
```http
DELETE /api/weather/Warsaw
```

Działanie:
- usuwa wszystkie zapisane snapshoty dla wskazanego miasta.

## Jak uruchomić projekt
### Wymagania
Na komputerze musi być zainstalowane:
- **.NET 8 SDK**

Można to sprawdzić poleceniem:
```bash
dotnet --list-sdks
```

### Uruchomienie aplikacji
Z poziomu katalogu głównego repozytorium uruchom:
```bash
dotnet restore
dotnet run --project .\\src\\MeteoWeaver.Api\\MeteoWeaver.Api.csproj
```

Po uruchomieniu aplikacja sama utworzy plik bazy danych SQLite.

## Testy
Aby uruchomić testy:
```bash
dotnet test
```

## Swagger
Po uruchomieniu aplikacji dokumentacja endpointów dostępna jest pod adresem wygenerowanym przez aplikację, np.:
```text
https://localhost:xxxx/swagger
```

## Przykładowy scenariusz użycia
1. Wywołanie `POST /api/weather/import/Warsaw`
2. Zapis danych pogodowych Warszawy do SQLite
3. Wywołanie `GET /api/weather/Warsaw/summary`
4. Otrzymanie bieżących danych oraz najlepszych godzin według Comfort Score
5. Kolejny import tego samego miasta i porównanie trendu względem poprzedniego zapisu

## Co pokazuje ten projekt
Projekt pokazuje praktyczne użycie:
- komunikacji z zewnętrznym API,
- zapisu danych do bazy,
- projektowania prostego API HTTP,
- wydzielenia logiki biznesowej do osobnych serwisów,
- podstaw testowania jednostkowego,
- przygotowania aplikacji gotowej do uruchomienia po pobraniu z repozytorium.

## Możliwe dalsze rozszerzenia
Projekt można w prosty sposób rozwinąć o:
- cache wyników,
- harmonogram automatycznych importów,
- bardziej rozbudowaną analizę warunków pogodowych,
- filtrowanie historii po dacie,
- konteneryzację w Dockerze.
