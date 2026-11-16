# OTObliczeniaTrakcyjneSKPZ1989
OT Traction Calculations SKPZ 1989 – calculating train running times across the Polish PKP network using 1989 rolling stock, with support for adding modern traction vehicles. / OT Obliczenia Trakcyjne SKPZ 1989 obliczanie czasu jazdy pociagów na terenie Polski PKP z taborem z 1989 roku + możliwość dodania współczesnych pojazdów trakcyjnych.
# OTObliczeniaTrakcyjneSKPZ1989

**EN:** OT Traction Calculations SKPZ 1989 – a tool for calculating train running times across the Polish PKP railway network using historical 1989 rolling stock, with support for adding modern traction vehicles.  
**PL:** OT Obliczenia Trakcyjne SKPZ 1989 – program do obliczania czasu jazdy pociągów na sieci kolejowej PKP z wykorzystaniem historycznego taboru z 1989 roku oraz możliwością dodawania współczesnych pojazdów trakcyjnych.

---

## English – Rolling Stock File Format & Rules

The rolling stock database is stored as a semicolon-separated (`;`) text/CSV file. Each line represents a single traction vehicle.

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
