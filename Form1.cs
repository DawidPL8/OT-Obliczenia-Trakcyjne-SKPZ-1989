using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private ComboBox cmbPrzewoznik;
        private ComboBox cmbPojazd1;
        private ComboBox cmbTrakcja1;
        private ComboBox cmbPojazd2;
        private ComboBox cmbRola2;
        private ComboBox cmbTrakcja2;
        private ComboBox cmbPojazd3;
        private ComboBox cmbRola3;
        private ComboBox cmbTrakcja3;
        private ComboBox cmbKodPociagu;
        private TextBox txtMasaBrutto;
        private TextBox txtDlugoscWagonow;
        private ComboBox cmbTypWagonow;
        private TextBox txtLiczbaOsi;
        private DataGridView dgvTrasa;
        private DataGridView dgvWyniki;
        private Button btnOblicz;
        private Label lblPodsumowanie;
        private Label lblNaglowekTrasa;
        private string aktualnyPlikTrasy = "trasa.txt";

        private PojazdTrakcyjny[] tablicaTaboru = Array.Empty<PojazdTrakcyjny>();
        private KategoriaPociagu[] tablicaKategorii = Array.Empty<KategoriaPociagu>();

        private string sciezkaPrzewoznicy = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "przewoznicy.txt");
        private string sciezkaTabor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tabor.txt");
        private string sciezkaTrasa = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trasa.txt");

        public Form1()
        {
            InitializeComponent();
            ZbudujSlownikKategorii();
            UtworzDomyslnePlikiJesliBrak(false);
            ZbudujInterfejs();
            WczytajPrzewoznikowZPliku(sciezkaPrzewoznicy);
            WczytajTaborZPliku(sciezkaTabor);
            WczytajTraseZPliku(sciezkaTrasa);
            OdswiezKoloryPol();
        }

        private void ZbudujSlownikKategorii()
        {
            tablicaKategorii = new KategoriaPociagu[]
            {
                // A. Pociągi pasażerskie - Parowe (ah=0,50)
                new KategoriaPociagu("E - Ekspresowy parowy", 0.50, "PAS"),
                new KategoriaPociagu("P - Pośpieszny parowy", 0.50, "PAS"),
                new KategoriaPociagu("D - Osobowy dalekobieżny parowy", 0.50, "PAS"),
                new KategoriaPociagu("M - Osobowy miejscowy parowy", 0.50, "PAS"),
                new KategoriaPociagu("O - Osobowo-towarowy parowy", 0.50, "PAS"),

                // A. Pociągi pasażerskie - Elektryczne lokomotywy
                new KategoriaPociagu("EE - Ekspresowy elektryczny", 1.00, "PAS"),
                new KategoriaPociagu("PE - Pośpieszny elektryczny", 0.85, "PAS"),
                new KategoriaPociagu("DE - Osobowy dalekobieżny elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("ME - Osobowy miejscowy elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("OE - Osobowo-towarowy elektryczny", 0.60, "PAS"),

                // A. Pociągi pasażerskie - Elektryczne ZT (Masa_Ladowny_t)
                new KategoriaPociagu("EJ - Ekspresowy EZT", 1.00, "ZT"),
                new KategoriaPociagu("PJ - Pośpieszny EZT", 0.85, "ZT"),
                new KategoriaPociagu("DJ - Osobowy dalekobieżny EZT", 0.75, "ZT"),
                new KategoriaPociagu("MJ - Osobowy miejscowy EZT", 0.75, "ZT"),

                // A. Pociągi pasażerskie - Spalinowe lokomotywy
                new KategoriaPociagu("ES - Ekspresowy spalinowy", 1.00, "PAS"),
                new KategoriaPociagu("PS - Pośpieszny spalinowy", 0.85, "PAS"),
                new KategoriaPociagu("DS - Osobowy dalekobieżny spalinowy", 0.75, "PAS"),
                new KategoriaPociagu("MS - Osobowy miejscowy spalinowy", 0.75, "PAS"),
                new KategoriaPociagu("OS - Osobowo-towarowy spalinowy", 0.60, "PAS"),

                // A. Pociągi pasażerskie - Spalinowe ZT (Masa_Ladowny_t)
                new KategoriaPociagu("EM - Ekspresowy SZT", 1.00, "ZT"),
                new KategoriaPociagu("PM - Pośpieszny SZT", 0.85, "ZT"),
                new KategoriaPociagu("DM - Osobowy dalekobieżny SZT", 0.75, "ZT"),
                new KategoriaPociagu("MM - Osobowy miejscowy SZT", 0.75, "ZT"),
                new KategoriaPociagu("OM - Osobowo-towarowy SZT", 0.60, "ZT"),

                // B. Pociągi towarowe - Parowe
                new KategoriaPociagu("TO - Towarowo-osobowy parowy", 0.45, "TOW"),
                new KategoriaPociagu("TX - Przesyłkowo-ekspresowy parowy", 0.45, "TOW"),
                new KategoriaPociagu("TP - Towarowy pośpieszny parowy", 0.45, "TOW"),
                new KategoriaPociagu("TN - Towarowy niemasowy parowy", 0.40, "TOW"),
                new KategoriaPociagu("TM - Towarowy masowy parowy", 0.40, "TOW"),
                new KategoriaPociagu("TB - Towarowy odcinkowy parowy", 0.40, "TOW"),
                new KategoriaPociagu("TZ - Towarowy zbiorowy parowy", 0.40, "TOW"),
                new KategoriaPociagu("TK - Towarowy zdawczy parowy", 0.40, "TOW"),
                new KategoriaPociagu("TKL - Towarowy zdawczy na linii parowy", 0.40, "TOW"),
                new KategoriaPociagu("TG - Towarowy gospodarczy parowy", 0.40, "TOW"),

                // B. Pociągi towarowe - Elektryczne
                new KategoriaPociagu("TOE - Towarowo-osobowy elektryczny", 0.60, "TOW"),
                new KategoriaPociagu("TXE - Przesyłkowo-ekspresowy elektryczny", 0.60, "TOW"),
                new KategoriaPociagu("TPE - Towarowy pośpieszny elektryczny", 0.60, "TOW"),
                new KategoriaPociagu("TNE - Towarowy niemasowy elektryczny", 0.50, "TOW"),
                new KategoriaPociagu("TME - Towarowy masowy elektryczny", 0.45, "TOW"),
                new KategoriaPociagu("TBE - Towarowy odcinkowy elektryczny", 0.50, "TOW"),
                new KategoriaPociagu("TZE - Towarowy zbiorowy elektryczny", 0.50, "TOW"),
                new KategoriaPociagu("TKE - Towarowy zdawczy elektryczny", 0.45, "TOW"),
                new KategoriaPociagu("TKLE - Towarowy zdawczy na linii elektryczny", 0.45, "TOW"),
                new KategoriaPociagu("TGE - Towarowy gospodarczy elektryczny", 0.42, "TOW"),

                // B. Pociągi towarowe - Spalinowe
                new KategoriaPociagu("TOS - Towarowo-osobowy spalinowy", 0.60, "TOW"),
                new KategoriaPociagu("TOM - Towarowo-osobowy SZT", 0.60, "ZT"),
                new KategoriaPociagu("TXS - Przesyłkowo-ekspresowy spalinowy", 0.60, "TOW"),
                new KategoriaPociagu("TPS - Towarowy pośpieszny spalinowy", 0.60, "TOW"),
                new KategoriaPociagu("TNS - Towarowy niemasowy spalinowy", 0.50, "TOW"),
                new KategoriaPociagu("TMS - Towarowy masowy spalinowy", 0.45, "TOW"),
                new KategoriaPociagu("TBS - Towarowy odcinkowy spalinowy", 0.50, "TOW"),
                new KategoriaPociagu("TZS - Towarowy zbiorowy spalinowy", 0.50, "TOW"),
                new KategoriaPociagu("TKS - Towarowy zdawczy spalinowy", 0.45, "TOW"),
                new KategoriaPociagu("TKLS - Towarowy zdawczy na linii spalinowy", 0.45, "TOW"),
                new KategoriaPociagu("TGS - Towarowy gospodarczy spalinowy", 0.42, "TOW"),

                // C. Pociągi służbowe - Parowe
                new KategoriaPociagu("R - Ratunkowy parowy", 0.45, "PAS"),
                new KategoriaPociagu("C - Próbny i doświadczalny parowy", 0.45, "PAS"),
                new KategoriaPociagu("N - Inspekcyjny parowy", 0.45, "PAS"),
                new KategoriaPociagu("SDŁ - Pocztowo-bagażowy parowy", 0.50, "PAS"),
                new KategoriaPociagu("SD - Próżny osobowy (>200 km) parowy", 0.50, "PAS"),
                new KategoriaPociagu("SP - Próżny osobowy (<200 km) parowy", 0.50, "PAS"),
                new KategoriaPociagu("S - Próżny pasażerski węzłowy parowy", 0.50, "PAS"),
                new KategoriaPociagu("W - Pracowniczy parowy", 0.50, "PAS"),
                new KategoriaPociagu("L - Parowóz luzem", 0.50, "LUZ"),

                // C. Pociągi służbowe - Elektryczne lokomotywy
                new KategoriaPociagu("RE - Ratunkowy elektryczny", 0.55, "PAS"),
                new KategoriaPociagu("CE - Próbny i doświadczalny elektryczny", 0.60, "PAS"),
                new KategoriaPociagu("NE - Inspekcyjny elektryczny", 0.60, "PAS"),
                new KategoriaPociagu("SDŁE - Pocztowo-bagażowy elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("SDE - Próżny osobowy (>200 km) elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("SPE - Próżny osobowy (<200 km) elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("SE - Próżny pasażerski węzłowy elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("WE - Pracowniczy elektryczny", 0.75, "PAS"),
                new KategoriaPociagu("LE - Lokomotywa elektryczna luzem", 0.75, "LUZ"),

                // C. Pociągi służbowe - Elektryczne ZT (próżne -> Masa_Prozny_t, pracowniczy WJ -> Masa_Ladowny_t)
                new KategoriaPociagu("RJ - Ratunkowy EZT (próżny)", 0.55, "ZT_LUZ"),
                new KategoriaPociagu("CJ - Próbny i doświadczalny EZT (próżny)", 0.60, "ZT_LUZ"),
                new KategoriaPociagu("NJ - Inspekcyjny EZT (próżny)", 0.60, "ZT_LUZ"),
                new KategoriaPociagu("SDJ - Próżny osobowy (>200 km) EZT", 0.75, "ZT_LUZ"),
                new KategoriaPociagu("SPJ - Próżny osobowy (<200 km) EZT", 0.75, "ZT_LUZ"),
                new KategoriaPociagu("SJ - Próżny pasażerski węzłowy EZT", 0.75, "ZT_LUZ"),
                new KategoriaPociagu("WJ - Pracowniczy EZT", 0.75, "ZT"),
                new KategoriaPociagu("LJ - EZT luzem (próżny)", 0.75, "ZT_LUZ"),

                // C. Pociągi służbowe - Spalinowe lokomotywy
                new KategoriaPociagu("RS - Ratunkowy spalinowy", 0.55, "PAS"),
                new KategoriaPociagu("CS - Próbny i doświadczalny spalinowy", 0.60, "PAS"),
new KategoriaPociagu("NS - Inspekcyjny spalinowy", 0.60, "PAS"),
new KategoriaPociagu("SDŁS - Pocztowo-bagażowy spalinowy", 0.75, "PAS"),
new KategoriaPociagu("SDS - Próżny osobowy (>200 km) spalinowy", 0.75, "PAS"),
new KategoriaPociagu("SPS - Próżny osobowy (<200 km) spalinowy", 0.75, "PAS"),
new KategoriaPociagu("SS - Próżny pasażerski węzłowy spalinowy", 0.75, "PAS"),
new KategoriaPociagu("WS - Pracowniczy spalinowy", 0.75, "PAS"),
new KategoriaPociagu("LS - Lokomotywa spalinowa luzem", 0.65, "LUZ"),
// C. Pociągi służbowe - Spalinowe ZT (próżne -> Masa_Prozny_t, pracowniczy WM -> Masa_Ladowny_t)
new KategoriaPociagu("RM - Ratunkowy SZT (próżny)", 0.55, "ZT_LUZ"),
new KategoriaPociagu("CM - Próbny i doświadczalny SZT (próżny)", 0.60, "ZT_LUZ"),
new KategoriaPociagu("NM - Inspekcyjny SZT (próżny)", 0.60, "ZT_LUZ"),
new KategoriaPociagu("SDM - Próżny osobowy (>200 km) SZT", 0.75, "ZT_LUZ"),
new KategoriaPociagu("SPM - Próżny osobowy (<200 km) SZT", 0.75, "ZT_LUZ"),
new KategoriaPociagu("SM - Próżny pasażerski węzłowy SZT", 0.75, "ZT_LUZ"),
new KategoriaPociagu("WM - Pracowniczy SZT", 0.75, "ZT"),
new KategoriaPociagu("LM - SZT luzem (próżny)", 0.65, "ZT_LUZ")
};
        }
        private void ZbudujInterfejs()
        {
            Font duzaCzcionka = new Font("Segoe UI", 13.5f);
            Font duzaCzcionkaPogrubiona = new Font("Segoe UI", 13.5f, FontStyle.Bold);
            this.Text = "OT - Obliczenia Trakcyjne (SKPZ / PKP do 1989 r.)";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1024, 600);
            this.Font = duzaCzcionka;
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            // Górny panel parametrów
            Panel pnlGora = new Panel { Dock = DockStyle.Top, Height = 190, Padding = new Padding(8), BackColor = Color.FromArgb(240, 244, 248) };
            // Wiersz 0: Przewoźnik oraz przyciski słowników i instrukcji
            Label lblPrzew = new Label { Text = "Przewoźnik:", Location = new Point(12, 12), AutoSize = true, Font = duzaCzcionkaPogrubiona };
            cmbPrzewoznik = new ComboBox { Location = new Point(150, 8), Width = 380, DropDownStyle = ComboBoxStyle.DropDownList };
            Button btnOdswiezSlowniki = new Button { Text = "Odśwież tabor/przewoźnicy (.txt)", Location = new Point(550, 6), Size = new Size(320, 36), Cursor = Cursors.Hand };
            btnOdswiezSlowniki.Click += (s, e) =>
            {
                WczytajPrzewoznikowZPliku(sciezkaPrzewoznicy);
                WczytajTaborZPliku(sciezkaTabor);
                OdswiezKoloryPol();
                MessageBox.Show("Odświeżono listy z plików przewoznicy.txt oraz tabor.txt!", "Słowniki", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Button btnPomocTrasa = new Button
            {
                Text = "Instrukcja trasy (.txt)",
                Location = new Point(885, 6),
                Size = new Size(220, 36),
                Cursor = Cursors.Hand
            };
            btnPomocTrasa.Click += (s, e) => OtworzPlikInstrukcji("INSTRUKCJA_TRASY.txt");
            Button btnPomocTabor = new Button
            {
                Text = "Instrukcja taboru (.txt)",
                Location = new Point(1115, 6),
                Size = new Size(225, 36),
                Cursor = Cursors.Hand
            };
            btnPomocTabor.Click += (s, e) => OtworzPlikInstrukcji("INSTRUKCJA_TABORU.txt");
            // Wiersz 1: Pojazd I, II oraz III
            Label lblP1 = new Label { Text = "Pojazd I (Czoło):", Location = new Point(12, 58), AutoSize = true, Font = duzaCzcionkaPogrubiona };
            cmbPojazd1 = new ComboBox { Location = new Point(175, 54), Width = 175, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPojazd1.SelectedIndexChanged += (s, e) =>
            {
                AktualizujListeTrakcji(cmbPojazd1, cmbTrakcja1);
                AktualizujDostepneRolePojazdow();
                OdswiezKoloryPol();
            };
            Label lblP2 = new Label { Text = "Pojazd II:", Location = new Point(365, 58), AutoSize = true };
            cmbPojazd2 = new ComboBox { Location = new Point(455, 54), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbPojazd2.SelectedIndexChanged += (s, e) =>
            {
                AktualizujListeTrakcji(cmbPojazd2, cmbTrakcja2);
                AktualizujDostepneRolePojazdow();
                OdswiezKoloryPol();
            };
            cmbRola2 = new ComboBox { Location = new Point(628, 54), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbRola2.SelectedIndexChanged += (s, e) =>
            {
                AktualizujRolePojazdu3();
                OdswiezKoloryPol();
            };
            Label lblP3 = new Label { Text = "Pojazd III:", Location = new Point(865, 58), AutoSize = true };
            cmbPojazd3 = new ComboBox { Location = new Point(960, 54), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbPojazd3.SelectedIndexChanged += (s, e) =>
            {
                AktualizujListeTrakcji(cmbPojazd3, cmbTrakcja3);
                AktualizujRolePojazdu3();
                OdswiezKoloryPol();
            };
            cmbRola3 = new ComboBox { Location = new Point(1133, 54), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbRola3.SelectedIndexChanged += (s, e) => OdswiezKoloryPol();
            // Wiersz 2: Trakcja bezpośrednio pod każdym pojazdem (I, II, III)
            Label lblTr1 = new Label { Text = "Trakcja:", Location = new Point(95, 102), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 110) };
            cmbTrakcja1 = new ComboBox { Location = new Point(175, 98), Width = 175, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblTr2 = new Label { Text = "Trakcja:", Location = new Point(375, 102), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 110) };
            cmbTrakcja2 = new ComboBox { Location = new Point(455, 98), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            Label lblTr3 = new Label { Text = "Trakcja:", Location = new Point(880, 102), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 110) };
            cmbTrakcja3 = new ComboBox { Location = new Point(960, 98), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            // Wiersz 3: Rodzaj pociągu (R-11), Brutto, Długość, Wagony, Ilość osi
            Label lblKat = new Label { Text = "Rodz. poc.:", Location = new Point(12, 148), AutoSize = true };
            cmbKodPociagu = new ComboBox { Location = new Point(125, 144), Width = 385, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int k = 0; k < tablicaKategorii.Length; k++)
            {
                KategoriaPociagu katItem = (KategoriaPociagu)tablicaKategorii.GetValue(k);
                cmbKodPociagu.Items.Add(katItem.Opis);
            }
            cmbKodPociagu.SelectedIndex = 0;
            Label lblMasa = new Label { Text = "Brutto:", Location = new Point(525, 148), AutoSize = true };
            txtMasaBrutto = new TextBox { Location = new Point(600, 144), Width = 85, Text = "1600" };
            txtMasaBrutto.TextChanged += (s, e) => OdswiezKoloryPol();
            Label lblDlugosc = new Label { Text = "Dług.:", Location = new Point(700, 148), AutoSize = true };
            txtDlugoscWagonow = new TextBox { Location = new Point(765, 144), Width = 85, Text = "300" };
            txtDlugoscWagonow.TextChanged += (s, e) => OdswiezKoloryPol();
            Label lblTypWagonow = new Label { Text = "Wagony:", Location = new Point(865, 148), AutoSize = true };
            cmbTypWagonow = new ComboBox { Location = new Point(955, 144), Width = 145, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTypWagonow.Items.AddRange(new string[] { "Pasażerskie", "Towarowe" });
            cmbTypWagonow.SelectedIndex = 0;
            cmbTypWagonow.SelectedIndexChanged += (s, e) => OdswiezKoloryPol();
            Label lblOsie = new Label { Text = "Ilość osi:", Location = new Point(1115, 148), AutoSize = true };
            txtLiczbaOsi = new TextBox { Location = new Point(1210, 144), Width = 75, Text = "24" };
            txtLiczbaOsi.TextChanged += (s, e) => OdswiezKoloryPol();
            pnlGora.Controls.AddRange(new Control[] {
lblPrzew, cmbPrzewoznik, btnOdswiezSlowniki, btnPomocTrasa, btnPomocTabor,
lblP1, cmbPojazd1, lblTr1, cmbTrakcja1,
lblP2, cmbPojazd2, cmbRola2, lblTr2, cmbTrakcja2,
lblP3, cmbPojazd3, cmbRola3, lblTr3, cmbTrakcja3,
lblKat, cmbKodPociagu, lblMasa, txtMasaBrutto, lblDlugosc, txtDlugoscWagonow,
lblTypWagonow, cmbTypWagonow, lblOsie, txtLiczbaOsi
});
            // Dolny pasek z przyciskiem Oblicz (zawsze widoczny na dole ekranu!)
            Panel pnlDol = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(8), BackColor = Color.FromArgb(240, 244, 248) };
            lblPodsumowanie = new Label { Text = "Kliknij przycisk Oblicz, aby uruchomić symulację.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = duzaCzcionkaPogrubiona };
            Button btnEksportWynikow = new Button
            {
                Text = "Eksportuj wyniki (.txt)",
                Dock = DockStyle.Right,
                Width = 175,
                Cursor = Cursors.Hand
            };
            btnEksportWynikow.Click += BtnEksportWynikow_Click;
            btnOblicz = new Button
            {
                Text = "Oblicz",
                Dock = DockStyle.Right,
                Width = 175,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 15.0f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOblicz.Click += BtnOblicz_Click;
            pnlDol.Controls.Add(lblPodsumowanie);
            pnlDol.Controls.Add(btnEksportWynikow);
            pnlDol.Controls.Add(btnOblicz);
            // Środkowy podział ekranu (SplitContainer): góra = Trasa, dół = Wyniki
            SplitContainer splitSrodek = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 6
            };
            // Górna część SplitContainer: Pasek narzędzi + Tabela Trasy
            Panel pnlNarzedziaTrasy = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 4, 8, 4), BackColor = Color.FromArgb(230, 236, 242) };
            lblNaglowekTrasa = new Label { Text = "1. Trasa: " + FormatujNazweTrasy(aktualnyPlikTrasy), Location = new Point(10, 9), AutoSize = true, Font = duzaCzcionkaPogrubiona };
            Button btnZapiszTrase = new Button { Text = "Zapisz trasę (.txt)", Dock = DockStyle.Right, Width = 195, Cursor = Cursors.Hand };
            btnZapiszTrase.Click += BtnZapiszTrase_Click;
            Button btnWczytajTrase = new Button { Text = "Importuj trasę (.txt)", Dock = DockStyle.Right, Width = 205, Cursor = Cursors.Hand };
            btnWczytajTrase.Click += BtnWczytajTrase_Click;
            pnlNarzedziaTrasy.Controls.Add(lblNaglowekTrasa);
            pnlNarzedziaTrasy.Controls.Add(btnWczytajTrase);
            pnlNarzedziaTrasy.Controls.Add(btnZapiszTrase);
            dgvTrasa = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                ColumnHeadersHeight = 38
            };
            dgvTrasa.RowTemplate.Height = 32;
            dgvTrasa.DefaultCellStyle.Font = duzaCzcionka;
            dgvTrasa.ColumnHeadersDefaultCellStyle.Font = duzaCzcionkaPogrubiona;
            dgvTrasa.CellValueChanged += (s, e) => OdswiezKoloryPol();
            dgvTrasa.RowsAdded += (s, e) => OdswiezKoloryPol();
            dgvTrasa.Columns.Add("Punkt", "Punkt eksploatacyjny");
            dgvTrasa.Columns.Add("Km", "Kilometraż [km]");
            dgvTrasa.Columns.Add("Vmax", "V drog. [km/h]");
            dgvTrasa.Columns.Add("Pochylenie", "Pochylenie [‰]");
            DataGridViewComboBoxColumn colPostoj = new DataGridViewComboBoxColumn
            {
                Name = "Postoj",
                HeaderText = "Postój",
                DataSource = new string[] { "TAK", "NIE" }
            };
            dgvTrasa.Columns.Add(colPostoj);
            DataGridViewComboBoxColumn colKod = new DataGridViewComboBoxColumn
            {
                Name = "Kod",
                HeaderText = "KOD",
                DataSource = new string[] { "BRAK", "PRZ_CZOLO", "TKM", "ZMIANA_LINII/TORU", "TKM_ZMIANA_LINII/TORU", "OGR" }
            };
            dgvTrasa.Columns.Add(colKod);
            splitSrodek.Panel1.Controls.Add(dgvTrasa);
            splitSrodek.Panel1.Controls.Add(pnlNarzedziaTrasy);
            // Dolna część SplitContainer: Nagłówek + Tabela Wyników
            Label lblNaglowekWyniki = new Label { Text = "2. Wyniki obliczeń trakcyjnych:", Dock = DockStyle.Top, Height = 34, Padding = new Padding(10, 4, 0, 0), Font = duzaCzcionkaPogrubiona };
            dgvWyniki = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(250, 252, 255),
                ColumnHeadersHeight = 38
            };
            dgvWyniki.RowTemplate.Height = 32;
            dgvWyniki.DefaultCellStyle.Font = duzaCzcionka;
            dgvWyniki.ColumnHeadersDefaultCellStyle.Font = duzaCzcionkaPogrubiona;
            dgvWyniki.Columns.Add("Odcinek", "Odcinek");
            dgvWyniki.Columns.Add("VSymulacja", "V symulacja [km/h]");
            dgvWyniki.Columns.Add("CzasJazdy", "Czas jazdy [min]");
            splitSrodek.Panel2.Controls.Add(dgvWyniki);
            splitSrodek.Panel2.Controls.Add(lblNaglowekWyniki);
            this.Controls.Add(splitSrodek);
            this.Controls.Add(pnlGora);
            this.Controls.Add(pnlDol);
            this.Shown += (s, e) =>
            {
                if (splitSrodek.Height > 200)
                {
                    splitSrodek.SplitterDistance = splitSrodek.Height / 2;
                }
                OdswiezKoloryPol();
            };
        }
        private void OtworzPlikInstrukcji(string nazwaPliku)
        {
            string pelnaSciezka = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nazwaPliku);
            if (File.Exists(pelnaSciezka))
            {
                Process.Start(new ProcessStartInfo(pelnaSciezka) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show($"Nie znaleziono pliku {nazwaPliku} w folderze programu.", "Brak pliku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void OdswiezKoloryPol()
        {
            Color kolorMorski = Color.FromArgb(0, 175, 175);
            Color kolorBialy = Color.White;
            if (txtMasaBrutto != null)
                txtMasaBrutto.BackColor = string.IsNullOrWhiteSpace(txtMasaBrutto.Text) ? kolorMorski : kolorBialy;
            if (txtDlugoscWagonow != null)
                txtDlugoscWagonow.BackColor = string.IsNullOrWhiteSpace(txtDlugoscWagonow.Text) ? kolorMorski : kolorBialy;
            if (cmbTypWagonow != null && txtLiczbaOsi != null)
            {
                bool czyPasazerskie = (cmbTypWagonow.SelectedItem != null && cmbTypWagonow.SelectedItem.ToString() == "Pasażerskie");
                txtLiczbaOsi.Enabled = czyPasazerskie;
                if (!czyPasazerskie)
                {
                    txtLiczbaOsi.BackColor = SystemColors.Control;
                }
                else
                {
                    txtLiczbaOsi.BackColor = string.IsNullOrWhiteSpace(txtLiczbaOsi.Text) ? kolorMorski : kolorBialy;
                }
            }
            if (cmbRola2 != null)
            {
                bool aktywny2 = (cmbRola2.SelectedItem != null && cmbRola2.SelectedItem.ToString() != "Brak");
                cmbPojazd2.BackColor = aktywny2 ? kolorBialy : kolorMorski;
                cmbRola2.BackColor = aktywny2 ? kolorBialy : kolorMorski;
                cmbTrakcja2.BackColor = aktywny2 ? kolorBialy : kolorMorski;
            }
            if (cmbRola3 != null)
            {
                bool aktywny3 = (cmbRola3.SelectedItem != null && cmbRola3.SelectedItem.ToString() != "Brak");
                cmbPojazd3.BackColor = aktywny3 ? kolorBialy : kolorMorski;
                cmbRola3.BackColor = aktywny3 ? kolorBialy : kolorMorski;
                cmbTrakcja3.BackColor = aktywny3 ? kolorBialy : kolorMorski;
            }
            if (dgvTrasa != null)
            {
                foreach (DataGridViewRow row in dgvTrasa.Rows)
                {
                    if (row.IsNewRow) continue;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        // Kolumna 3 to Pochylenie - pomijamy kolorowanie na morsko, bo jest dziedziczone automatycznie
                        if (cell.ColumnIndex == 3)
                        {
                            cell.Style.BackColor = kolorBialy;
                            continue;
                        }
                        string val = cell.Value != null ? cell.Value.ToString().Trim() : "";
                        cell.Style.BackColor = string.IsNullOrEmpty(val) ? kolorMorski : kolorBialy;
                    }
                }
            }
        }
        private void AktualizujListeTrakcji(ComboBox cmbPojazd, ComboBox cmbTrakcja)
        {
            cmbTrakcja.Items.Clear();
            if (tablicaTaboru.Length == 0 || cmbPojazd.SelectedIndex < 0) return;
            PojazdTrakcyjny p = (PojazdTrakcyjny)tablicaTaboru.GetValue(cmbPojazd.SelectedIndex);
            if (p.MocParowaKw > 0) cmbTrakcja.Items.Add("Parowa");
            if (p.MocSpalinowaKw > 0) cmbTrakcja.Items.Add("Spalinowa");
            if (p.Moc3kVKw > 0) cmbTrakcja.Items.Add("3 kV DC");
            if (p.Moc25kVKw > 0) cmbTrakcja.Items.Add("25 kV AC");
            if (cmbTrakcja.Items.Count == 0)
            {
                cmbTrakcja.Items.Add("Spalinowa");
            }
            cmbTrakcja.SelectedIndex = 0;
        }
        private PojazdTrakcyjny PobierzPojazdZCombo(ComboBox cmb)
        {
            if (tablicaTaboru.Length == 0 || cmb.SelectedIndex < 0) return null;
            return (PojazdTrakcyjny)tablicaTaboru.GetValue(cmb.SelectedIndex);
        }
        private bool CzyParowoz(ComboBox cmbPojazd, ComboBox cmbTrakcja)
        {
            if (cmbTrakcja.SelectedItem != null && cmbTrakcja.SelectedItem.ToString() == "Parowa") return true;
            PojazdTrakcyjny p = PobierzPojazdZCombo(cmbPojazd);
            if (p != null && (p.TrakcjaOpory == "P" || p.STA == "P")) return true;
            return false;
        }
        private bool CzyKompatybilneDoTrakcjiWielokrotnej(PojazdTrakcyjny p1, PojazdTrakcyjny p2)
        {
            if (p1 == null || p2 == null) return false;
            if (p1.Typ != p2.Typ) return false;
            if (string.Equals(p1.Seria, p2.Seria, StringComparison.OrdinalIgnoreCase)) return true;
            if (!string.IsNullOrEmpty(p1.RodzinaTW) && p1.RodzinaTW == p2.RodzinaTW) return true;
            if (p1.Typ == "ZT")
            {
                string s1 = p1.Seria.ToUpper();
                string s2 = p2.Seria.ToUpper();
                bool rodzinaEn57_1 = (s1 == "EN57" || s1 == "EN71" || s1 == "ED72" || s1 == "EW58" || s1 == "EW60");
                bool rodzinaEn57_2 = (s2 == "EN57" || s2 == "EN71" || s2 == "ED72" || s2 == "EW58" || s2 == "EW60");
                if (rodzinaEn57_1 && rodzinaEn57_2) return true;
                bool rodzina810_1 = (s1 == "810" || s1 == "830" || s1 == "842");
                bool rodzina810_2 = (s2 == "810" || s2 == "830" || s2 == "842");
                if (rodzina810_1 && rodzina810_2) return true;
            }
            return false;
        }
        private void AktualizujDostepneRolePojazdow()
        {
            PojazdTrakcyjny p1 = PobierzPojazdZCombo(cmbPojazd1);
            PojazdTrakcyjny p2 = PobierzPojazdZCombo(cmbPojazd2);
            string poprzedniaRola2 = cmbRola2.SelectedItem != null ? cmbRola2.SelectedItem.ToString() : "Brak";
            bool parowoz1Lub2 = CzyParowoz(cmbPojazd1, cmbTrakcja1) || CzyParowoz(cmbPojazd2, cmbTrakcja2);
            bool czyZespolTrakcyjny = (p1 != null && p1.Typ == "ZT") || (p2 != null && p2.Typ == "ZT");
            cmbRola2.Items.Clear();
            cmbRola2.Items.Add("Brak");
            if (!parowoz1Lub2 && CzyKompatybilneDoTrakcjiWielokrotnej(p1, p2))
            {
                cmbRola2.Items.Add("Trakcja wielokrotna");
            }
            if (!czyZespolTrakcyjny)
            {
                cmbRola2.Items.Add("Doprzęg");
                cmbRola2.Items.Add("Popychacz");
            }
            else if (p1 != null && p1.Typ == "LOK" && p2 != null && p2.Typ == "ZT")
            {
                cmbRola2.Items.Add("Doprzęg");
            }
            int idx2 = cmbRola2.Items.IndexOf(poprzedniaRola2);
            cmbRola2.SelectedIndex = (idx2 >= 0) ? idx2 : 0;
            AktualizujRolePojazdu3();
        }
        private void AktualizujRolePojazdu3()
        {
            PojazdTrakcyjny p1 = PobierzPojazdZCombo(cmbPojazd1);
            PojazdTrakcyjny p3 = PobierzPojazdZCombo(cmbPojazd3);
            string rola2 = cmbRola2.SelectedItem != null ? cmbRola2.SelectedItem.ToString() : "Brak";
            string poprzedniaRola3 = cmbRola3.SelectedItem != null ? cmbRola3.SelectedItem.ToString() : "Brak";
            bool czyKtorykolwiekParowoz = CzyParowoz(cmbPojazd1, cmbTrakcja1)
            || CzyParowoz(cmbPojazd2, cmbTrakcja2)
            || CzyParowoz(cmbPojazd3, cmbTrakcja3);
            bool czyZespolTrakcyjny = (p1 != null && p1.Typ == "ZT") || (p3 != null && p3.Typ == "ZT");
            cmbRola3.Items.Clear();
            cmbRola3.Items.Add("Brak");
            if (czyKtorykolwiekParowoz)
            {
                if (!czyZespolTrakcyjny)
                {
                    if (rola2 != "Popychacz")
                    {
                        cmbRola3.Items.Add("Doprzęg");
                    }
                    cmbRola3.Items.Add("Popychacz");
                }
            }
            else if (czyZespolTrakcyjny)
            {
                if ((rola2 == "Trakcja wielokrotna" || rola2 == "Brak") && CzyKompatybilneDoTrakcjiWielokrotnej(p1, p3))
                {
                    cmbRola3.Items.Add("Trakcja wielokrotna");
                }
            }
            else
            {
                if ((rola2 == "Trakcja wielokrotna" || rola2 == "Brak") && CzyKompatybilneDoTrakcjiWielokrotnej(p1, p3))
                {
                    cmbRola3.Items.Add("Trakcja wielokrotna");
                }
                cmbRola3.Items.Add("Popychacz");
            }
            int idx3 = cmbRola3.Items.IndexOf(poprzedniaRola3);
            cmbRola3.SelectedIndex = (idx3 >= 0) ? idx3 : 0;
        }
        private string PobierzElement(string[] tablica, int indeks)
        {
            if (tablica == null || indeks < 0 || indeks >= tablica.Length) return "";
            object wartosc = tablica.GetValue(indeks);
            return wartosc != null ? wartosc.ToString().Trim() : "";
        }
        private string Komorka(DataGridViewRow row, int indeks)
        {
            if (row == null || indeks < 0 || indeks >= row.Cells.Count) return "";
            int licznik = 0;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (licznik == indeks)
                {
                    return cell.Value != null ? cell.Value.ToString().Trim() : "";
                }
                licznik++;
            }
            return "";
        }
        private void UtworzDomyslnePlikiJesliBrak(bool tylkoTrasa)
        {
            if (!File.Exists(sciezkaPrzewoznicy))
            {
                string[] liniePrzewoznicy = new string[]
                {
"# Lista przewoznikow / zarzadow kolejowych w Polsce do 1989 roku",
"PKP",
"PMP-PW (Koleje Piaskowe)",
"KP - Hutnictwo",
"KP - Górnictwo i Energetyka",
"KP - Chemia i Petrochemia",
"Wojsko Polskie",
"Inne bocznice zakładowe"
                };
                File.WriteAllLines(sciezkaPrzewoznicy, liniePrzewoznicy, Encoding.UTF8);
            }
            if (!File.Exists(sciezkaTabor))
            {
                string[] linieTabor = new string[]
                {
"# Seria;Typ(LOK/ZT);Opory(E/S/P/Klasyczne/Nowoczesne/HS/HS+);STA(T/N/P);P_Parowa;P_Spalinowa;P_3kV;P_25kV;Masa_Prozny_t;Masa_Ladowny_t;Tender_t;Dlugosc_m;L_osi;N_napednych;Vmax_kmh;PsiA",
"ST44;LOK;S;N;;1472;;;116.5;116.5;0;18;6;6;100;",
"SU42;LOK;S;N;;588;;;70.0;70.0;0;14;4;4;90;",
"SM42;LOK;S;N;;588;;;70.0;70.0;0;14;4;4;90;",
"2M62Y;LOK;S;N;;2942;;;240.0;240.0;0;36;12;12;100;",
"EU07;LOK;E;N;;;2000;;83.0;83.0;0;16;4;4;125;",
"ET22;LOK;E;N;;;3000;;120.0;120.0;0;19;6;6;125;",
"111Ed Gama;LOK;E;T;;400;5600;;84.0;84.0;0;20;4;4;160;0.48",
"Ty2;LOK;P;P;1200;;;;86.1;86.1;60;23;6;5;80;",
"EN57;ZT;Klasyczne;N;;;580;;123.0;138.0;0;65;12;4;110;",
"SA133;ZT;Nowoczesne;N;;780;;;78.0;92.0;0;42;8;4;120;"
                };
                File.WriteAllLines(sciezkaTabor, linieTabor, Encoding.UTF8);
            }
            if (tylkoTrasa || !File.Exists(sciezkaTrasa))
            {
                string[] linieTrasa = new string[]
                {
"# Punkt;Km;Vmax_kmh;Pochylenie_promile;Postoj;Kod",
"Bydgoszcz Wschód Tow.;21.250;80;;TAK;BRAK",
"Bydgoszcz Leśna (POGP);24.178;80;3.2;NIE;BRAK",
"Maksymilianowo (ST);33.472;80;6.1;NIE;BRAK",
"Serock (ST);50.306;80;4.6;NIE;BRAK",
"Wierzchucin (ST);71.349;80;4.5;NIE;BRAK",
"Lipowa Tucholska (ST);94.370;80;3.7;NIE;BRAK",
"Bąk (ST);116.699;80;4.1;NIE;BRAK",
"Kościerzyna (ST - zmiana linii 201/211);137.322/69.320;80;5.1;NIE;ZMIANA_LINII/TORU",
"Lipusz (ST - linia 211);52.612;80;9.1;TAK;BRAK"
                };
                File.WriteAllLines(sciezkaTrasa, linieTrasa, Encoding.UTF8);
            }
        }
        private void WczytajPrzewoznikowZPliku(string sciezka)
        {
            cmbPrzewoznik.Items.Clear();
            if (!File.Exists(sciezka)) return;
            string[] linie = File.ReadAllLines(sciezka, Encoding.UTF8);
            for (int i = 0; i < linie.Length; i++)
            {
                string linia = PobierzElement(linie, i);
                if (string.IsNullOrWhiteSpace(linia) || linia.StartsWith("#")) continue;
                cmbPrzewoznik.Items.Add(linia);
            }
            if (cmbPrzewoznik.Items.Count > 0) cmbPrzewoznik.SelectedIndex = 0;
        }
        private void WczytajTaborZPliku(string sciezka)
        {
            if (!File.Exists(sciezka)) return;
            string[] wszystkieLinie = File.ReadAllLines(sciezka, Encoding.UTF8);
            PojazdTrakcyjny[] tymczasowa = new PojazdTrakcyjny[wszystkieLinie.Length];
            int licznik = 0;
            for (int i = 0; i < wszystkieLinie.Length; i++)
            {
                string linia = PobierzElement(wszystkieLinie, i);
                if (string.IsNullOrWhiteSpace(linia) || linia.StartsWith("#")) continue;
                string[] cz = linia.Split(';');
                if (cz.Length < 15) continue;
                PojazdTrakcyjny p = new PojazdTrakcyjny();
                p.Seria = PobierzElement(cz, 0);
                p.Typ = PobierzElement(cz, 1).ToUpper();
                p.TrakcjaOpory = PobierzElement(cz, 2).ToUpper();
                p.STA = PobierzElement(cz, 3).ToUpper();
                p.MocParowaKw = ParsujDouble(PobierzElement(cz, 4), 0);
                p.MocSpalinowaKw = ParsujDouble(PobierzElement(cz, 5), 0);
                p.Moc3kVKw = ParsujDouble(PobierzElement(cz, 6), 0);
                p.Moc25kVKw = ParsujDouble(PobierzElement(cz, 7), 0);
                p.MasaProznyT = ParsujDouble(PobierzElement(cz, 8), 116.5);
                p.MasaLadownyT = ParsujDouble(PobierzElement(cz, 9), p.MasaProznyT);
                p.TenderT = ParsujDouble(PobierzElement(cz, 10), 0);
                p.DlugoscM = ParsujDouble(PobierzElement(cz, 11), 18);
                p.LiczbaOsi = ParsujDouble(PobierzElement(cz, 12), 6);
                p.OsieNapedne = ParsujDouble(PobierzElement(cz, 13), 6);
                p.VmaxKmh = ParsujDouble(PobierzElement(cz, 14), 100);
                string psiStr = PobierzElement(cz, 15);
                p.PsiA = string.IsNullOrWhiteSpace(psiStr) ? (double?)null : ParsujDouble(psiStr, 0.33);
                p.RodzinaTW = (cz.Length > 16) ? PobierzElement(cz, 16).ToUpper() : "";
                tymczasowa.SetValue(p, licznik);
                licznik++;
            }
            tablicaTaboru = new PojazdTrakcyjny[licznik];
            cmbPojazd1.Items.Clear();
            cmbPojazd2.Items.Clear();
            cmbPojazd3.Items.Clear();
            for (int i = 0; i < licznik; i++)
            {
                PojazdTrakcyjny poj = (PojazdTrakcyjny)tymczasowa.GetValue(i);
                tablicaTaboru.SetValue(poj, i);
                cmbPojazd1.Items.Add(poj.Seria);
                cmbPojazd2.Items.Add(poj.Seria);
                cmbPojazd3.Items.Add(poj.Seria);
            }
            if (licznik > 0)
            {
                cmbPojazd1.SelectedIndex = 0;
                cmbPojazd2.SelectedIndex = 0;
                cmbPojazd3.SelectedIndex = 0;
                AktualizujListeTrakcji(cmbPojazd1, cmbTrakcja1);
                AktualizujListeTrakcji(cmbPojazd2, cmbTrakcja2);
                AktualizujListeTrakcji(cmbPojazd3, cmbTrakcja3);
                AktualizujDostepneRolePojazdow();
            }
        }
        private void WczytajTraseZPliku(string sciezka)
        {
            dgvTrasa.Rows.Clear();
            if (!File.Exists(sciezka)) return;
            string[] linie = File.ReadAllLines(sciezka, Encoding.UTF8);
            for (int i = 0; i < linie.Length; i++)
            {
                string linia = PobierzElement(linie, i);
                if (string.IsNullOrWhiteSpace(linia) || linia.StartsWith("#")) continue;
                string[] cz = linia.Split(';');
                if (cz.Length < 6) continue;
                string postoj = PobierzElement(cz, 4).ToUpper() == "TAK" ? "TAK" : "NIE";
                string kod = PobierzElement(cz, 5).ToUpper();
                // Zgodność wsteczna ze starszymi plikami tras
                if (kod == "ZMIANA_LINII") kod = "ZMIANA_LINII/TORU";
                if (kod == "TKM_ZMIANA") kod = "TKM_ZMIANA_LINII/TORU";
                if (kod != "PRZ_CZOLO" && kod != "TKM" && kod != "ZMIANA_LINII/TORU" && kod != "TKM_ZMIANA_LINII/TORU" && kod != "OGR")
                    kod = "BRAK";
                dgvTrasa.Rows.Add(
                PobierzElement(cz, 0),
                PobierzElement(cz, 1),
                PobierzElement(cz, 2),
                PobierzElement(cz, 3),
                postoj,
                kod
                );
            }
            aktualnyPlikTrasy = Path.GetFileName(sciezka);
            if (lblNaglowekTrasa != null) lblNaglowekTrasa.Text = "1. Trasa: " + FormatujNazweTrasy(aktualnyPlikTrasy);
            OdswiezKoloryPol();
        }
        private string FormatujNazweTrasy(string sciezkaLubNazwa)
        {
            string nazwa = Path.GetFileName(sciezkaLubNazwa);
            if (nazwa.StartsWith("trasa ", StringComparison.OrdinalIgnoreCase))
            {
                nazwa = nazwa.Substring(6).Trim();
            }
            return nazwa;
        }
        private void BtnWczytajTrase_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Pliki tekstowe (.txt)|*.txt", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WczytajTraseZPliku(ofd.FileName);
                }
            }
        }
        private void BtnZapiszTrase_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Pliki tekstowe (.txt)|*.txt", FileName = "trasa.txt", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    int ileWierszy = 0;
                    foreach (DataGridViewRow r in dgvTrasa.Rows) if (!r.IsNewRow) ileWierszy++;
                    string[] linie = new string[ileWierszy + 1];
                    linie.SetValue("# Punkt;Km;Vmax_kmh;Pochylenie_promile;Postoj;Kod", 0);
                    int idx = 1;
                    foreach (DataGridViewRow row in dgvTrasa.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string linia = $"{Komorka(row, 0)};{Komorka(row, 1)};{Komorka(row, 2)};{Komorka(row, 3)};{Komorka(row, 4)};{Komorka(row, 5)}";
                        linie.SetValue(linia, idx);
                        idx++;
                    }
                    File.WriteAllLines(sfd.FileName, linie, Encoding.UTF8);
                    MessageBox.Show("Zapisano trasę do pliku .txt!", "Zapis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void BtnEksportWynikow_Click(object sender, EventArgs e)
        {
            if (dgvWyniki.Rows.Count == 0)
            {
                MessageBox.Show("Najpierw kliknij Oblicz, aby wygenerować wyniki.", "Brak wyników", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nazwaBazowa = FormatujNazweTrasy(aktualnyPlikTrasy);
            if (nazwaBazowa.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                nazwaBazowa = nazwaBazowa.Substring(0, nazwaBazowa.Length - 4);
            }
            string domyslnaNazwaPliku = $"wyniki trasy {nazwaBazowa}.txt";

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Pliki tekstowe (.txt)|*.txt", FileName = domyslnaNazwaPliku, InitialDirectory = AppDomain.CurrentDomain.BaseDirectory })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string[] linie = new string[dgvWyniki.Rows.Count + 2];
                    linie.SetValue($"# Przewoznik: {cmbPrzewoznik.Text} | Pojazd I: {cmbPojazd1.Text} ({cmbTrakcja1.Text})", 0);
                    linie.SetValue("Odcinek;V_symulacja_kmh;Czas_jazdy_min", 1);
                    for (int i = 0; i < dgvWyniki.Rows.Count; i++)
                    {
                        DataGridViewRow row = dgvWyniki.Rows[i];
                        string linia = $"{Komorka(row, 0)};{Komorka(row, 1)};{Komorka(row, 2)}";
                        linie.SetValue(linia, i + 2);
                    }
                    File.WriteAllLines(sfd.FileName, linie, Encoding.UTF8);
                    MessageBox.Show("Wyeksportowano wyniki do pliku .txt!", "Eksport", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private double MasaAktywnaPojazdu(PojazdTrakcyjny p, KategoriaPociagu kat)
        {
            if (p == null) return 0.0;
            if (p.Typ == "ZT")
            {
                return (kat.TypOporow == "ZT_LUZ") ? p.MasaProznyT : p.MasaLadownyT;
            }
            return p.MasaLadownyT;
        }
        private double PobierzAktywnaMocKw(PojazdTrakcyjny p, string wybranaTrakcja)
        {
            if (p == null) return 0.0;
            if (wybranaTrakcja == "Parowa") return p.MocParowaKw;
            if (wybranaTrakcja == "Spalinowa") return p.MocSpalinowaKw;
            if (wybranaTrakcja == "3 kV DC") return p.Moc3kVKw;
            if (wybranaTrakcja == "25 kV AC") return p.Moc25kVKw;
            if (p.MocSpalinowaKw > 0) return p.MocSpalinowaKw;
            if (p.Moc3kVKw > 0) return p.Moc3kVKw;
            if (p.Moc25kVKw > 0) return p.Moc25kVKw;
            return p.MocParowaKw;
        }
        private bool ParsujKilometraz(string tekst, out double kmWjazd, out double kmWyjazd)
        {
            kmWjazd = double.NaN;
            kmWyjazd = double.NaN;
            if (string.IsNullOrWhiteSpace(tekst)) return false;
            string[] cz = tekst.Split('/');
            kmWjazd = ParsujDouble(PobierzElement(cz, 0), double.NaN);
            if (double.IsNaN(kmWjazd)) return false;
            if (cz.Length > 1 && !string.IsNullOrWhiteSpace(PobierzElement(cz, 1)))
            {
                kmWyjazd = ParsujDouble(PobierzElement(cz, 1), kmWjazd);
            }
            else
            {
                kmWyjazd = kmWjazd;
            }
            return true;
        }
        private void BtnOblicz_Click(object sender, EventArgs e)
        {
            if (tablicaTaboru.Length == 0 || cmbPojazd1.SelectedIndex < 0) return;
            dgvWyniki.Rows.Clear();
            PojazdTrakcyjny p1 = (PojazdTrakcyjny)tablicaTaboru.GetValue(cmbPojazd1.SelectedIndex);
            PojazdTrakcyjny p2 = (cmbRola2.SelectedIndex > 0 && cmbPojazd2.SelectedIndex >= 0) ? (PojazdTrakcyjny)tablicaTaboru.GetValue(cmbPojazd2.SelectedIndex) : null;
            PojazdTrakcyjny p3 = (cmbRola3.SelectedIndex > 0 && cmbPojazd3.SelectedIndex >= 0) ? (PojazdTrakcyjny)tablicaTaboru.GetValue(cmbPojazd3.SelectedIndex) : null;
            string trakcja1 = cmbTrakcja1.SelectedItem != null ? cmbTrakcja1.SelectedItem.ToString() : "Spalinowa";
            string trakcja2 = cmbTrakcja2.SelectedItem != null ? cmbTrakcja2.SelectedItem.ToString() : "Spalinowa";
            string trakcja3 = cmbTrakcja3.SelectedItem != null ? cmbTrakcja3.SelectedItem.ToString() : "Spalinowa";
            string rola2Tekst = cmbRola2.SelectedItem != null ? cmbRola2.SelectedItem.ToString() : "Brak";
            string rola3Tekst = cmbRola3.SelectedItem != null ? cmbRola3.SelectedItem.ToString() : "Brak";
            double wsp2 = (rola2Tekst == "Trakcja wielokrotna") ? 1.0 : 0.90;
            double wsp3 = (rola3Tekst == "Trakcja wielokrotna") ? 1.0 : 0.90;
            KategoriaPociagu kat = (KategoriaPociagu)tablicaKategorii.GetValue(cmbKodPociagu.SelectedIndex);
            double dlugoscWagonowM = Math.Max(0.0, ParsujDouble(txtDlugoscWagonow.Text, 300));
            double calkowitaDlugoscM = dlugoscWagonowM + p1.DlugoscM + (p2 != null ? p2.DlugoscM : 0.0) + (p3 != null ? p3.DlugoscM : 0.0);
            double masaWagonowT = (kat.TypOporow == "ZT" || kat.TypOporow == "LUZ" || kat.TypOporow == "ZT_LUZ") ? 0.0 : Math.Max(0.0, ParsujDouble(txtMasaBrutto.Text, 1600));
            double masaPojazdowT = (MasaAktywnaPojazdu(p1, kat) + p1.TenderT)
            + (p2 != null ? MasaAktywnaPojazdu(p2, kat) + p2.TenderT : 0.0)
            + (p3 != null ? MasaAktywnaPojazdu(p3, kat) + p3.TenderT : 0.0);
            double masaCalkowitaT = masaWagonowT + masaPojazdowT;
            double masaCalkowitaKg = masaCalkowitaT * 1000.0;
            bool wagonyTowarowe = (cmbTypWagonow.SelectedItem != null && cmbTypWagonow.SelectedItem.ToString() == "Towarowe");
            double liczbaOsiWagonow = Math.Max(2.0, ParsujDouble(txtLiczbaOsi.Text, 24.0));
            double naciskOsiT = (masaWagonowT > 0) ? (masaWagonowT / liczbaOsiWagonow) : 16.0;
            double vMaxPociaguKmh = p1.VmaxKmh;
            if (p2 != null && p2.VmaxKmh < vMaxPociaguKmh) vMaxPociaguKmh = p2.VmaxKmh;
            if (p3 != null && p3.VmaxKmh < vMaxPociaguKmh) vMaxPociaguKmh = p3.VmaxKmh;
            int maxWierszy = dgvTrasa.Rows.Count;
            PunktTrasy[] tempPunkty = new PunktTrasy[maxWierszy];
            int ilePunktow = 0;
            for (int r = 0; r < maxWierszy; r++)
            {
                DataGridViewRow row = dgvTrasa.Rows[r];
                if (row.IsNewRow) continue;
                string nazwa = Komorka(row, 0);
                if (string.IsNullOrEmpty(nazwa)) nazwa = "Punkt";
                if (!ParsujKilometraz(Komorka(row, 1), out double kmWjazd, out double kmWyjazd)) continue;
                double vmax = ParsujDouble(Komorka(row, 2), 80);
                string pochTekst = Komorka(row, 3);
                double? poch = string.IsNullOrWhiteSpace(pochTekst) ? (double?)null : ParsujDouble(pochTekst, 0);
                bool postoj = (Komorka(row, 4).ToUpper() == "TAK");
                string kod = Komorka(row, 5).ToUpper();
                if (string.IsNullOrEmpty(kod)) kod = "BRAK";
                PunktTrasy pt = new PunktTrasy();
                pt.Nazwa = nazwa;
                pt.KmWjazd = kmWjazd;
                pt.KmWyjazd = kmWyjazd;
                pt.Vmax = vmax;
                pt.Pochylenie = poch;
                pt.Postoj = postoj;
                pt.Kod = kod;
                tempPunkty.SetValue(pt, ilePunktow);
                ilePunktow++;
            }
            if (ilePunktow < 2)
            {
                MessageBox.Show("Wprowadź co najmniej 2 punkty trasy.", "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double aktualnePochylenie = 0.0;
            for (int p = ilePunktow - 1; p >= 0; p--)
            {
                PunktTrasy pt = (PunktTrasy)tempPunkty.GetValue(p);
                if (pt.Pochylenie.HasValue)
                    aktualnePochylenie = pt.Pochylenie.Value;
                else
                    pt.Pochylenie = aktualnePochylenie;
            }
            double[] dystansNarastajacyM = new double[ilePunktow];
            dystansNarastajacyM.SetValue(0.0, 0);
            for (int p = 1; p < ilePunktow; p++)
            {
                PunktTrasy ptPoprz = (PunktTrasy)tempPunkty.GetValue(p - 1);
                PunktTrasy ptAkt = (PunktTrasy)tempPunkty.GetValue(p);
                double dlugoscOdcinkaM = Math.Abs(ptAkt.KmWjazd - ptPoprz.KmWyjazd) * 1000.0;
                double poprzedniNarast = (double)dystansNarastajacyM.GetValue(p - 1);
                dystansNarastajacyM.SetValue(poprzedniNarast + dlugoscOdcinkaM, p);
            }
            double ds = 10.0;
            double dystansCalkowityM = (double)dystansNarastajacyM.GetValue(ilePunktow - 1);
            if (dystansCalkowityM <= 0)
            {
                MessageBox.Show("Całkowita długość trasy musi być większa od zera.", "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int liczbaKrokow = (int)Math.Ceiling(dystansCalkowityM / ds);
            double[] vLimitMs = new double[liczbaKrokow + 1];
            double[] pochylenieKrok = new double[liczbaKrokow];
            int[] indeksPunktu = new int[ilePunktow];
            for (int p = 0; p < ilePunktow; p++)
            {
                double distM = (double)dystansNarastajacyM.GetValue(p);
                int idx = (int)Math.Round(distM / ds);
                if (idx > liczbaKrokow) idx = liczbaKrokow;
                indeksPunktu.SetValue(idx, p);
            }
            // Przypisanie pochyleń oraz podstawowych prędkości drogowych na odcinkach
            for (int p = 0; p < ilePunktow - 1; p++)
            {
                int start = (int)indeksPunktu.GetValue(p);
                int koniec = (int)indeksPunktu.GetValue(p + 1);
                PunktTrasy ptNast = (PunktTrasy)tempPunkty.GetValue(p + 1);
                double vDrog = Math.Min(ptNast.Vmax, vMaxPociaguKmh);
                double poch = ptNast.Pochylenie.Value;
                for (int i = start; i < koniec; i++)
                {
                    pochylenieKrok.SetValue(poch, i);
                    vLimitMs.SetValue(vDrog / 3.6, i);
                }
            }
            vLimitMs.SetValue((double)vLimitMs.GetValue(liczbaKrokow - 1), liczbaKrokow);
            // Wymuszenie postoju na stacji początkowej oraz końcowej (Punkt 3)
            ((PunktTrasy)tempPunkty.GetValue(0)).Postoj = true;
            ((PunktTrasy)tempPunkty.GetValue(ilePunktow - 1)).Postoj = true;

            // Obsługa ZMIANA_LINII/TORU oraz TKM_ZMIANA_LINII/TORU (przejazd przez rozjazdy na bok 40 km/h - działa z postojem i bez postoju) (Punkt 4)
            int krokiZmianyLinii = (int)Math.Ceiling((calkowitaDlugoscM + 50.0) / ds);
            double vBokRozjazdowMs = Math.Min(40.0, vMaxPociaguKmh) / 3.6;
            for (int p = 1; p < ilePunktow - 1; p++)
            {
                PunktTrasy pt = (PunktTrasy)tempPunkty.GetValue(p);
                if (pt.Kod == "ZMIANA_LINII/TORU" || pt.Kod == "TKM_ZMIANA_LINII/TORU")
                {
                    int idxStacji = (int)indeksPunktu.GetValue(p);
                    int odKroku = idxStacji;
                    int doKroku = Math.Min(liczbaKrokow, idxStacji + krokiZmianyLinii);
                    for (int i = odKroku; i < doKroku; i++)
                    {
                        if (vBokRozjazdowMs < (double)vLimitMs.GetValue(i))
                        {
                            vLimitMs.SetValue(vBokRozjazdowMs, i);
                        }
                    }
                }
            }

            // Zaciągnięcie ograniczenia prędkości na całą długość składu (Punkt 1)
            // Obowiązuje przy wzroście prędkości (np. z 60 na 100 km/h) oraz dla kodów OGR/TKM/BRAK (pomijane dla PRZ_CZOLO i zmiany linii/toru)
            int krokiCalkowitejDlugosci = (int)Math.Ceiling(calkowitaDlugoscM / ds);
            double[] vLimitZUwzglSkladuMs = (double[])vLimitMs.Clone();
            for (int p = 0; p < ilePunktow - 2; p++)
            {
                PunktTrasy ptPunkt = (PunktTrasy)tempPunkty.GetValue(p + 1);
                PunktTrasy ptKolejny = (PunktTrasy)tempPunkty.GetValue(p + 2);

                if (ptPunkt.Kod == "PRZ_CZOLO" || ptPunkt.Kod == "ZMIANA_LINII/TORU" || ptPunkt.Kod == "TKM_ZMIANA_LINII/TORU") continue;

                double vPrzedMs = Math.Min(ptPunkt.Vmax, vMaxPociaguKmh) / 3.6;
                double vPoMs = Math.Min(ptKolejny.Vmax, vMaxPociaguKmh) / 3.6;

                // Zaciągamy niższą prędkość na długość składu tylko wtedy, gdy prędkość rośnie (pociąg musi wyjechać całym składem ze strefy ograniczenia)
                if (vPoMs > vPrzedMs)
                {
                    int koniec = (int)indeksPunktu.GetValue(p + 1);
                    int koniecZaciagniecia = Math.Min(liczbaKrokow, koniec + krokiCalkowitejDlugosci);
                    for (int i = koniec; i < koniecZaciagniecia; i++)
                    {
                        if (vPrzedMs < (double)vLimitZUwzglSkladuMs.GetValue(i))
                        {
                            vLimitZUwzglSkladuMs.SetValue(vPrzedMs, i);
                        }
                    }
                }
            }
            vLimitMs = vLimitZUwzglSkladuMs;

            // Wymuszenie postoju (0 km/h) w punktach z postojem
            for (int p = 0; p < ilePunktow; p++)
            {
                PunktTrasy pt = (PunktTrasy)tempPunkty.GetValue(p);
                if (pt.Postoj)
                {
                    vLimitMs.SetValue(0.0, (int)indeksPunktu.GetValue(p));
                }
            }

            // Wyznaczenie krzywej hamowania wstecz przed postojami i ograniczeniami
            double ah = kat.OpoznienieHamowania;
            for (int i = liczbaKrokow - 1; i >= 0; i--)
            {
                double vNast = (double)vLimitMs.GetValue(i + 1);
                double vAkt = (double)vLimitMs.GetValue(i);
                double vMaxZHamowania = Math.Sqrt(vNast * vNast + 2.0 * ah * ds);
                if (vMaxZHamowania < vAkt)
                {
                    vLimitMs.SetValue(vMaxZHamowania, i);
                }
            }
            double g = 9.80665;
            double wspMasWirujacych = 1.0;
            // ETAP 1: Przebieg bazowy (bez TKM), aby poznać maksymalną prędkość osiągalną na każdym odcinku
            double[] vBazoweMs = (double[])vLimitMs.Clone();
            for (int i = 0; i < liczbaKrokow; i++)
            {
                double vMs = vBazoweMs[i];
                double vKmh = vMs * 3.6;
                double silaPociagowaN = ObliczSilePojazdu(p1, kat, trakcja1, vMs, vKmh, 1.0)
                + (p2 != null ? ObliczSilePojazdu(p2, kat, trakcja2, vMs, vKmh, wsp2) : 0.0)
                + (p3 != null ? ObliczSilePojazdu(p3, kat, trakcja3, vMs, vKmh, wsp3) : 0.0);
                double oporPojazdowN = ObliczOporPojazdu(p1, kat, trakcja1, vKmh)
                + (p2 != null ? ObliczOporPojazdu(p2, kat, trakcja2, vKmh) : 0.0)
                + (p3 != null ? ObliczOporPojazdu(p3, kat, trakcja3, vKmh) : 0.0);
                double oporWagonowN = 0.0;
                if (masaWagonowT > 0)
                {
                    if (wagonyTowarowe)
                    {
                        double stosunekV = vKmh / 100.0;
                        oporWagonowN = (0.9 + 1.4 * stosunekV * stosunekV) * masaWagonowT * g;
                    }
                    else
                    {
                        oporWagonowN = (1.5 + (vKmh * vKmh) / (2000.0 * naciskOsiT)) * masaWagonowT * g;
                    }
                }
                double pochKroku = pochylenieKrok[i];
                double oporWzniesieniaN = masaCalkowitaT * g * pochKroku;
                double silaNettoN = silaPociagowaN - (oporPojazdowN + oporWagonowN + oporWzniesieniaN);
                double a = silaNettoN / (masaCalkowitaKg * wspMasWirujacych);
                double vNastepneMs = Math.Sqrt(Math.Max(0.0, vMs * vMs + 2.0 * a * ds));
                double vLimitNast = vLimitMs[i + 1];
                if (vNastepneMs < 0.8 && vLimitNast > 1.0)
                {
                    MessageBox.Show($"Pociąg o masie całkowitej {masaCalkowitaT:0.0} t jest za ciężki na wzniesieniu {pochKroku} ‰!", "Przeciążenie składu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (vNastepneMs > vLimitNast) vNastepneMs = vLimitNast;
                vBazoweMs[i + 1] = vNastepneMs;
            }
            // Wyznaczenie maksymalnej prędkości osiągalnej na każdym odcinku w przebiegu bazowym
            double[] maxVNaOdcinkuKmh = new double[ilePunktow - 1];
            for (int p = 0; p < ilePunktow - 1; p++)
            {
                int start = indeksPunktu[p];
                int koniec = indeksPunktu[p + 1];
                double maxV = 0.0;
                for (int i = start; i <= koniec; i++)
                {
                    double vk = vBazoweMs[i] * 3.6;
                    if (vk > maxV) maxV = vk;
                }
                maxVNaOdcinkuKmh[p] = maxV;
            }
            // ETAP 2: Główna symulacja ruchu z adaptacyjną obsługą TKM
            double[] vRzeczMs = (double[])vLimitMs.Clone();
            double[] czasKrokuS = new double[liczbaKrokow];
            bool tkmAktywne = false;
            bool tkmFazaHamowania = false;
            bool tkmWykonanoNaOdcinku = false;
            double tkmCelHamowaniaMs = 0.0;
            int aktualnyOdcinekIdx = -1;
            bool[] odcinekWykonanoTkm = new bool[ilePunktow - 1];
            for (int i = 0; i < liczbaKrokow; i++)
            {
                int odcIdx = 0;
                for (int p = 0; p < ilePunktow - 1; p++)
                {
                    if (i >= indeksPunktu[p] && i < indeksPunktu[p + 1])
                    {
                        odcIdx = p;
                        break;
                    }
                }
                if (odcIdx != aktualnyOdcinekIdx)
                {
                    aktualnyOdcinekIdx = odcIdx;
                    PunktTrasy ptStart = tempPunkty[odcIdx];
                    PunktTrasy ptKoniec = tempPunkty[odcIdx + 1];
                    tkmAktywne = (ptStart.Postoj && (ptKoniec.Kod == "TKM" || ptKoniec.Kod == "TKM_ZMIANA_LINII/TORU"));
                    tkmFazaHamowania = false;
                    tkmWykonanoNaOdcinku = false;
                }
                double vMs = vRzeczMs[i];
                double vKmh = vMs * 3.6;
                double silaPociagowaN = ObliczSilePojazdu(p1, kat, trakcja1, vMs, vKmh, 1.0)
                + (p2 != null ? ObliczSilePojazdu(p2, kat, trakcja2, vMs, vKmh, wsp2) : 0.0)
                + (p3 != null ? ObliczSilePojazdu(p3, kat, trakcja3, vMs, vKmh, wsp3) : 0.0);
                double oporPojazdowN = ObliczOporPojazdu(p1, kat, trakcja1, vKmh)
                + (p2 != null ? ObliczOporPojazdu(p2, kat, trakcja2, vKmh) : 0.0)
                + (p3 != null ? ObliczOporPojazdu(p3, kat, trakcja3, vKmh) : 0.0);
                double oporWagonowN = 0.0;
                if (masaWagonowT > 0)
                {
                    if (wagonyTowarowe)
                    {
                        double stosunekV = vKmh / 100.0;
                        oporWagonowN = (0.9 + 1.4 * stosunekV * stosunekV) * masaWagonowT * g;
                    }
                    else
                    {
                        oporWagonowN = (1.5 + (vKmh * vKmh) / (2000.0 * naciskOsiT)) * masaWagonowT * g;
                    }
                }
                double pochKroku = pochylenieKrok[i];
                double oporWzniesieniaN = masaCalkowitaT * g * pochKroku;
                double silaNettoN = silaPociagowaN - (oporPojazdowN + oporWagonowN + oporWzniesieniaN);
                double a = silaNettoN / (masaCalkowitaKg * wspMasWirujacych);
                double vNastepneMs = Math.Sqrt(Math.Max(0.0, vMs * vMs + 2.0 * a * ds));
                double vLimitNast = vLimitMs[i + 1];
                if (vNastepneMs > vLimitNast)
                {
                    vNastepneMs = vLimitNast;
                }
                // Obsługa próby hamulców w biegu (TKM) na bieżącym odcinku
                if (tkmAktywne && !tkmWykonanoNaOdcinku)
                {
                    PunktTrasy ptKoniec = tempPunkty[aktualnyOdcinekIdx + 1];
                    double vRozkladowaOdcKmh = Math.Min(ptKoniec.Vmax, vMaxPociaguKmh);
                    double vProgStandardowyKmh = Math.Min(60.0, vRozkladowaOdcKmh);
                    double vProgWyzwoleniaKmh = Math.Min(vProgStandardowyKmh, maxVNaOdcinkuKmh[aktualnyOdcinekIdx]);
                    if (!tkmFazaHamowania && vProgWyzwoleniaKmh > 10.0 && vKmh >= vProgWyzwoleniaKmh - 0.5)
                    {
                        tkmFazaHamowania = true;
                        double celKmh = (vKmh >= 59.5) ? 30.0 : (vKmh / 2.0);
                        tkmCelHamowaniaMs = celKmh / 3.6;
                    }
                    if (tkmFazaHamowania)
                    {
                        double vHamMs = Math.Sqrt(Math.Max(0.0, vMs * vMs - 2.0 * ah * ds));
                        vNastepneMs = Math.Min(vHamMs, vLimitNast);
                        if (vNastepneMs <= tkmCelHamowaniaMs)
                        {
                            vNastepneMs = Math.Min(tkmCelHamowaniaMs, vLimitNast);
                            tkmFazaHamowania = false;
                            tkmWykonanoNaOdcinku = true;
                            tkmAktywne = false;
                            odcinekWykonanoTkm[aktualnyOdcinekIdx] = true;
                        }
                    }
                }
                vRzeczMs[i + 1] = vNastepneMs;
                double vSrednieMs = Math.Max(0.5, (vMs + vNastepneMs) / 2.0);
                czasKrokuS[i] = ds / vSrednieMs;
            }
            // Zestawienie wyników z automatycznym sumowaniem odcinków z kodem OGR
            int startGrupyIdx = 0;
            double sumaCzasuS = 0.0;
            double maxVGrupy = 0.0;
            bool byloTkmWGrupie = false;
            double calkowityCzasS = 0.0;
            for (int p = 0; p < ilePunktow - 1; p++)
            {
                int start = indeksPunktu[p];
                int koniec = indeksPunktu[p + 1];
                for (int i = start; i < koniec; i++)
                {
                    sumaCzasuS += czasKrokuS[i];
                    calkowityCzasS += czasKrokuS[i];
                    double vk = vRzeczMs[i] * 3.6;
                    if (vk > maxVGrupy) maxVGrupy = vk;
                }
                if (odcinekWykonanoTkm[p]) byloTkmWGrupie = true;
                PunktTrasy ptNastepny = tempPunkty[p + 1];
                if (ptNastepny.Kod != "OGR" || p == ilePunktow - 2)
                {
                    double czasMinuty = sumaCzasuS / 60.0;
                    PunktTrasy ptStartGrupy = tempPunkty[startGrupyIdx];
                    string opisOdcinka = byloTkmWGrupie
                    ? $"{ptStartGrupy.Nazwa} -> {ptNastepny.Nazwa} [TKM]"
                    : $"{ptStartGrupy.Nazwa} -> {ptNastepny.Nazwa}";
                    dgvWyniki.Rows.Add(
                    opisOdcinka,
                    Math.Round(maxVGrupy, 0).ToString("0"),
                    Math.Round(czasMinuty, 1).ToString("0.0")
                    );
                    startGrupyIdx = p + 1;
                    sumaCzasuS = 0.0;
                    maxVGrupy = 0.0;
                    byloTkmWGrupie = false;
                }
            }
            double dystansKm = dystansCalkowityM / 1000.0;
            double calkowityCzasMin = calkowityCzasS / 60.0;
            lblPodsumowanie.Text = $"Długość trasy [km]: {dystansKm:0.000}   |   Całkowity czas jazdy [min]: {calkowityCzasMin:0.0}";
        }
        private double ObliczSilePojazdu(PojazdTrakcyjny p, KategoriaPociagu kat, string wybranaTrakcja, double vMs, double vKmh, double wspolczynnikSily)
        {
            double g = 9.80665;
            double masaPojazdu = MasaAktywnaPojazdu(p, kat);
            double masaNapednaT = (p.LiczbaOsi > 0) ? (p.OsieNapedne / p.LiczbaOsi) * masaPojazdu : masaPojazdu;

            double mocKw = PobierzAktywnaMocKw(p, wybranaTrakcja);
            double mocW = mocKw * 1000.0;
            double silaKoncowaN = 0.0;

            // 1. PAROWOZY (STA = P)
            if (p.STA == "P" || wybranaTrakcja == "Parowa")
            {
                double psi = Math.Max(0.05, 0.25 - 0.0071 * Math.Sqrt(Math.Max(0.0, vKmh)));
                double fPrzyczepnosciN = masaNapednaT * g * psi * 1000.0;
                double fMocyN = (vMs < 1.0) ? fPrzyczepnosciN : (mocW / vMs);
                silaKoncowaN = Math.Min(fPrzyczepnosciN, fMocyN);
            }
            // 2. SILNIKI ASYNCHRONICZNE AC (STA = T oraz podane PsiA)
            else if (p.STA == "T" && p.PsiA.HasValue)
            {
                // Stała siła maksymalna Fmax obliczona z PsiA (płaska linia od 0 do Vbazowe)
                double fMaxN = masaNapednaT * g * p.PsiA.Value * 1000.0;

                // Prędkość bazowa Vb [m/s] ze wzoru: Vb = P / Fmax
                double vBazoweMs = (fMaxN > 0) ? (mocW / fMaxN) : 0.0;

                if (vMs <= vBazoweMs)
                {
                    silaKoncowaN = fMaxN;
                }
                else
                {
                    silaKoncowaN = mocW / vMs;
                }
            }
            // 3. SILNIKI DC (STA = N lub brak wartości w PsiA) - wzór Andrewsa
            else
            {
                double psi = 0.33 / (1.0 + 0.0288 * vMs);
                double fPrzyczepnosciN = masaNapednaT * g * psi * 1000.0;
                double fMocyN = (vMs < 1.0) ? fPrzyczepnosciN : (mocW / vMs);
                silaKoncowaN = Math.Min(fPrzyczepnosciN, fMocyN);
            }

            return silaKoncowaN * wspolczynnikSily;
        }

        private double ObliczOporPojazdu(PojazdTrakcyjny p, KategoriaPociagu kat, string wybranaTrakcja, double vKmh)
        {
            double g = 9.80665;
            double a = 1.6, b = 0.011, c = 0.00035;
            double masaPojazdu = MasaAktywnaPojazdu(p, kat);
            if (p.Typ == "ZT")
            {
                if (p.TrakcjaOpory == "KLASYCZNE" || p.TrakcjaOpory == "ZT_KLAS") { a = 2.0; b = 0.01; c = 0.0005; }
                else if (p.TrakcjaOpory == "NOWOCZESNE" || p.TrakcjaOpory == "ZT_NOW") { a = 1.5; b = 0.005; c = 0.0002; }
                else if (p.TrakcjaOpory == "HS" || p.TrakcjaOpory == "ZT_HS") { a = 1.2; b = 0.004; c = 0.00011; }
                else if (p.TrakcjaOpory == "HS+" || p.TrakcjaOpory == "ZT_HSP") { a = 1.1; b = 0.003; c = 0.000085; }
                return (a + b * vKmh + c * vKmh * vKmh) * g * masaPojazdu;
            }
            else
            {
                if (p.TrakcjaOpory == "E") { a = 2.4; b = 0.0; c = 0.00035; }
                else if (p.TrakcjaOpory == "S") { a = 1.6; b = 0.011; c = 0.00035; }
                else if (p.TrakcjaOpory == "P") { a = 4.5; b = 0.03; c = 0.002; }
                else if (wybranaTrakcja == "3 kV DC" || wybranaTrakcja == "25 kV AC") { a = 2.4; b = 0.0; c = 0.00035; }
                else if (wybranaTrakcja == "Spalinowa") { a = 1.6; b = 0.011; c = 0.00035; }
                else if (wybranaTrakcja == "Parowa") { a = 4.5; b = 0.03; c = 0.002; }
                return (a + b * vKmh + c * vKmh * vKmh) * g * (masaPojazdu + p.TenderT);
            }
        }
        private double ParsujDouble(string tekst, double domyslna)
        {
            if (string.IsNullOrWhiteSpace(tekst)) return domyslna;
            if (double.TryParse(tekst.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double wynik))
                return wynik;
            return domyslna;
        }
        private class PunktTrasy
        {
            public string Nazwa { get; set; }
            public double KmWjazd { get; set; }
            public double KmWyjazd { get; set; }
            public double Vmax { get; set; }
            public double? Pochylenie { get; set; }
            public bool Postoj { get; set; }
            public string Kod { get; set; }
        }
        private class PojazdTrakcyjny
        {
            public string Seria { get; set; }
            public string Typ { get; set; }
            public string TrakcjaOpory { get; set; }
            public string STA { get; set; }
            public double MocParowaKw { get; set; }
            public double MocSpalinowaKw { get; set; }
            public double Moc3kVKw { get; set; }
            public double Moc25kVKw { get; set; }
            public double MasaProznyT { get; set; }
            public double MasaLadownyT { get; set; }
            public double TenderT { get; set; }
            public double DlugoscM { get; set; }
            public double LiczbaOsi { get; set; }
            public double OsieNapedne { get; set; }
            public double VmaxKmh { get; set; }
            public double? PsiA { get; set; }
            public string RodzinaTW { get; set; }
        }
        private class KategoriaPociagu
        {
            public string Opis { get; set; }
            public double OpoznienieHamowania { get; set; }
            public string TypOporow { get; set; }
            public KategoriaPociagu(string opis, double ah, string typOporow)
            {
                Opis = opis;
                OpoznienieHamowania = ah;
                TypOporow = typOporow;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}