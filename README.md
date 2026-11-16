# OT Obliczenia Trakcyjne SKPZ 1989 – Aplikacja do obliczeń trakcyjnych i symulacji czasu jazdy pociągów (PKP)

**PL:** **OT Obliczenia Trakcyjne SKPZ 1989** to darmowa aplikacja desktopowa (Windows / C#) do wykonywania obliczeń trakcyjnych oraz symulacji czasu jazdy pociągów na sieci kolejowej PKP. Program wykorzystuje historyczną bazę taboru z 1989 roku oraz pozwala na łatwe dodawanie własnych tras i współczesnych pojazdów trakcyjnych (w tym z napędem asynchronicznym).

**EN:** **OT Traction Calculations SKPZ 1989** – a free Windows desktop application for railway traction calculations and train running time simulations across the Polish PKP network using 1989 rolling stock, with support for adding custom routes and modern traction vehicles.

## Pobieranie i uruchomienie / Download & How to Run
* **Gotowy program (Windows):** Pobierz najnowszą paczkę `.zip` z zakładki **[Releases](../../releases)**, rozpakuj wszystkie pliki do jednego folderu i uruchom `OTSKPZ1989.exe`.
* **Ready-to-run app (Windows):** Download the latest `.zip` archive from **[Releases](../../releases)**, extract all files into one folder, and run `OTSKPZ1989.exe`.


---

## English – Rolling Stock File Format & Rules

The rolling stock database is stored as a semicolon-separated (`;`) text/CSV file. Each line represents a single traction vehicle.
<img width="1917" height="1020" alt="Zrzut ekranu 2026-11-16 081549" src="https://github.com/user-attachments/assets/bae847ba-0bb7-478b-a97d-e635d2e2b896" />
### File Structure (Columns)
`Seria;Typ(LOK/ZT);Opory(E/S/P/Klasyczne/Nowoczesne/HS/HS+);STA(T/N/P);P_Parowa;P_Spalinowa;P_3kV;P_25kV;Masa_Prozny_t;Masa_Ladowny_t;Tender_t;Dlugosc_m;L_osi;N_napednych;Vmax_kmh;PsiA`

* **Seria:** Vehicle class/series name (e.g., `EP09`, `EN57`, `Ty2`).
* **Typ:** Vehicle type (`LOK` – locomotive, `ZT` – multiple unit).
* **Opory:** Rolling resistance profile (`E` – electric, `S` – diesel, `P` – steam, `Klasyczne`, `Nowoczesne`, `HS`, `HS+`).
* **STA:** Automatic Train Control / signaling system (`T` – Yes, `N` – No, `P` – Steam/Other).
* **P_Parowa / P_Spalinowa / P_3kV / P_25kV:** Power output [kW] for the respective traction type (leave empty if not applicable, keep the `;` separators).
* **Masa_Prozny_t / Masa_Ladowny_t / Tender_t:** Empty mass, loaded mass, and tender mass in metric tons.
* **Dlugosc_m:** Vehicle length in meters.
* **L_osi / N_napednych:** Total number of axles and number of powered axles.
* **Vmax_kmh:** Maximum speed in km/h.
* **PsiA:** Adhesion/traction curve parameter.

### Important Rules for Adding New Vehicles
1. **Do NOT fill in `PsiA` for classic vehicles:** The `PsiA` parameter is reserved **strictly for modern traction vehicles with asynchronous motors** (AC traction). For steam locomotives, classic diesel/electric locomotives, and older multiple units (e.g., EN57, EU07), leave this field blank (keep the trailing semicolon `;` at the end of the line).
2. **Use dot (`.`) as decimal separator:** For example, `78.2` instead of `78,2`.
3. **Preserve all semicolons:** Even if a column is empty (like unused power columns), do not delete the `;` delimiters.

---

## Polski – Format pliku taboru i zasady edycji

Baza taboru jest zapisana w pliku tekstowym/CSV z separatorem w postaci średnika (`;`). Każdy wiersz odpowiada jednemu pojazdowi trakcyjnemu.

### Struktura pliku (kolumny)
`Seria;Typ(LOK/ZT);Opory(E/S/P/Klasyczne/Nowoczesne/HS/HS+);STA(T/N/P);P_Parowa;P_Spalinowa;P_3kV;P_25kV;Masa_Prozny_t;Masa_Ladowny_t;Tender_t;Dlugosc_m;L_osi;N_napednych;Vmax_kmh;PsiA`

* **Seria:** Oznaczenie serii pojazdu (np. `EP09`, `EN57`, `Ty2`).
* **Typ:** Rodzaj pojazdu (`LOK` – lokomotywa, `ZT` – zespół trakcyjny).
* **Opory:** Charakterystyka oporów ruchu (`E` – elektrowóz, `S` – spalinowóz, `P` – parowóz, `Klasyczne`, `Nowoczesne`, `HS`, `HS+`).
* **STA:** Wyposażenie w systemy bezpieczeństwa/sterowania (`T` – Tak, `N` – Nie, `P` – Parowe/Inne).
* **P_Parowa / P_Spalinowa / P_3kV / P_25kV:** Moc [kW] dla danego rodzaju napędu (pola niedotyczące pojazdu zostawiamy puste, zachowując średniki).
* **Masa_Prozny_t / Masa_Ladowny_t / Tender_t:** Masa służbowa/próżnego, masa ładowny oraz masa tendra w tonach.
* **Dlugosc_m:** Długość pojazdu w metrach.
* **L_osi / N_napednych:** Całkowita liczba osi oraz liczba osi napędnych.
* **Vmax_kmh:** Prędkość maksymalna w km/h.
* **PsiA:** Współczynnik przyczepności / charakterystyki trakcyjnej.

### Ważne zasady przy dopisywaniu taboru
1. **Nie wpisuj wartości `PsiA` dla klasycznego taboru:** Parametr `PsiA` jest przeznaczony **wyłącznie dla nowoczesnych pojazdów trakcyjnych z napędem asynchronicznym**. Dla parowozów, klasycznych lokomotyw spalinowych/elektrycznych oraz starszych EZT (np. EU07, EN57) pole to musi pozostać puste (należy jednak zachować średnik `;` na końcu wiersza).
2. **Używaj kropki (`.`) w ułamkach:** Wartości dziesiętne zapisuj z kropką, np. `78.2`.
3. **Zachowuj komplet średników:** Nawet jeśli pojazd nie korzysta z danej kolumny mocy, nie usuwaj rozdzielających ją średników.
## Źródła danych taborowych / Rolling Stock Data Sources

## Źródła danych taborowych / Rolling Stock Data Sources

**PL:**
Aktualne dane techniczne oraz parametry trakcyjne współczesnych pojazdów zarejestrowanych na sieci kolejowej w Polsce (niezbędne do uzupełnienia bazy taboru) znajdują się w **Załączniku nr 13** (*Wykaz zarejestrowanych pojazdów trakcyjnych w aplikacji ISZTP i SKRJ – Obliczenia trakcyjne*) do aktualnego Regulaminu sieci PKP Polskie Linie Kolejowe S.A.:
👉 [Regulamin sieci PKP PLK S.A. – Załącznik nr 13 (XLSX)](https://www.plk-sa.pl/klienci-i-kontrahenci/warunki-udostepniania-infrastruktury-i-regulaminy/regulamin-sieci/)

**EN:**
Up-to-date technical data and traction parameters for modern traction units registered on the Polish railway network (to be used when expanding the rolling stock database) can be found in **Annex 13** (*List of registered traction units in the ISZTP and SKRJ application – Traction Assessment*) to the current PKP Polskie Linie Kolejowe S.A. Network Statement:
👉 [PKP PLK S.A. Network Statement – Annex 13 (XLSX)](https://www.plk-sa.pl/klienci-i-kontrahenci/warunki-udostepniania-infrastruktury-i-regulaminy/regulamin-sieci/)

