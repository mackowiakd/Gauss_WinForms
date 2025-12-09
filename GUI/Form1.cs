using Gauss_elim.threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;





namespace GUI
{
    public partial class Form1 : Form
    {
        private string inputPath = "";
        private string outputPath = "GUI_outp.txt";
        private bool useAsm = false;
        private bool useCpp = false;
        private int threadCount = 1;
        // Ścieżka bazowa do folderu z testami (obok pliku .exe)
        private string baseTestDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_data");
        public Form1()
        {
            InitializeComponent();
          
            SetupPresetData(); // Konfiguracja listy rozwijanej
       
        }
        // --- CZĘŚĆ 1: Gotowe zestawy danych (S, M, L) ---
        private void SetupPresetData()
        {
            // Dodaj te elementy do ComboBoxa w Designerze lub tutaj kodem:
            cbTestData.Items.Clear();
            cbTestData.Items.Add("Wybierz zestaw...");
            cbTestData.Items.Add("Mały (S) - N=50");
            cbTestData.Items.Add("Średni (M) - N=250");
            cbTestData.Items.Add("Duży (L) - N=1250");
            cbTestData.SelectedIndex = 0;
        }

        public string GetInputFilePath()
        {
            return inputPath;
        }
        public bool IsUsingAsm()
        {
            return useAsm;
        }
        public bool IsUsingCpp()
        {
            return useCpp;
        }
        public int ThreadCount { get { return threadCount; } }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Aktualizujemy zmienną globalną
            inputPath = textBox1.Text.Trim();

            // Wołamy walidację w trybie INPUT (true)
            ValidatePath(textBox1, true);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Aktualizujemy zmienną globalną
            outputPath = textBox2.Text.Trim();

            // Wołamy walidację w trybie OUTPUT (false)
            ValidatePath(textBox2, false);
        }

        private async void Run_button_Click(object sender, EventArgs e)
        {
            // 1. Walidacja
            if (!ValidateRunConditions()) return;

            // --- PRZYGOTOWANIE ŚCIEŻEK (TWORZENIE 4 ARGUMENTÓW) ---
            string inputFileName = Path.GetFileNameWithoutExtension(inputPath);

            // Jeśli output pusty, bierzemy folder pliku wejściowego
            string outputDir = string.IsNullOrWhiteSpace(outputPath)
                ? Path.GetDirectoryName(inputPath)
                : outputPath;

            // Generujemy pełne ścieżki dla plików wyjściowych
            string file_outp = Path.Combine(outputDir, $"{inputFileName}_stepped.txt");
            string file_res = Path.Combine(outputDir, $"{inputFileName}_result.txt");


            // 2. BLOKADA UI i PASEK POSTĘPU
            Run_button.Enabled = false;

            // Zakładam, że masz te kontrolki na Form1:
            progressBar1.Visible = true;
            lblStatus.Visible = true;
            lblStatus.Text = "Obliczenia w toku...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;

            this.Cursor = Cursors.WaitCursor;

            // Tu używamy pełnej ścieżki do klasy, żeby ominąć problemy z usingami
            Gauss_elim.threading.ParallelExecutor P_exe = new Gauss_elim.threading.ParallelExecutor();
            long executionTime = 0;

            try
            {
                // 3. OBLICZENIA W TLE
                await Task.Run(() =>
                {
                    if (useAsm)
                    {
                        // TERAZ MAMY 4 ARGUMENTY
                        executionTime = P_exe.run_asm(inputPath, threadCount, file_outp, file_res);
                    }
                    else
                    {
                        // TERAZ MAMY 4 ARGUMENTY
                        executionTime = P_exe.run_cpp(inputPath, threadCount, file_outp, file_res);
                    }
                });

                // 4. SUKCES
                SetExecutionTime(executionTime);
                lblStatus.Text = "Gotowe!";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                MessageBox.Show($"Wyniki zapisano w katalogu:\n{outputDir}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Błąd!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show($"Błąd: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 5. SPRZĄTANIE
                Run_button.Enabled = true;
                progressBar1.Visible = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void cbTestData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTestData.SelectedIndex <= 0) return;

            string fileName = "";
            switch (cbTestData.SelectedIndex)
            {
                case 1: fileName = "matrix50x50.txt"; break;
                case 2: fileName = "matrix250x250.txt"; break;
                case 3: fileName = "matrix1250x1250.txt"; break;
            }

            // Automatycznie ustawiamy ścieżkę w textBox1
            string fullPath = Path.Combine(baseTestDir, fileName);
            textBox1.Text = fullPath;

            // Opcjonalnie: Sprawdź czy plik istnieje i daj znać, jeśli go brakuje
            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"Uwaga: Nie znaleziono pliku testowego w:\n{fullPath}\n\nUpewnij się, że wygenerowałaś dane!", "Brak pliku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private bool ValidateRunConditions()
        {
            // Walidacja Inputu (musi istnieć i być .txt)
            if (!File.Exists(inputPath) || !Path.GetExtension(inputPath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Nieprawidłowy plik wejściowy!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Walidacja Outputu (musi być istniejącym katalogiem LUB być pusty)
            if (!string.IsNullOrWhiteSpace(outputPath) && !Directory.Exists(outputPath))
            {
                // Opcjonalnie: spróbuj utworzyć katalog
                try
                {
                    Directory.CreateDirectory(outputPath);
                }
                catch
                {
                    MessageBox.Show("Podany katalog wyjściowy jest nieprawidłowy i nie można go utworzyć.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (!useAsm && !useCpp)
            {
                MessageBox.Show("Wybierz bibliotekę (ASM lub C++).", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ValidatePath(TextBox box, bool isInputMode)
        {
            string path = box.Text.Trim();

            if (string.IsNullOrEmpty(path))
            {
                // Dla Outputu puste pole jest OK -> domyślnie folder z plikiem wejściowym
                box.BackColor = isInputMode ? System.Drawing.Color.LightCoral : System.Drawing.Color.White;
                return;
            }

            try
            {
                if (isInputMode)
                {
                    // --- LOGIKA DLA WEJŚCIA (Plik .txt) ---
                    string extension = Path.GetExtension(path);
                    bool isTxt = extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
                    bool exists = File.Exists(path);

                    box.BackColor = (isTxt && exists) ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightCoral;
                }
                else
                {
                    // --- LOGIKA DLA WYJŚCIA (Katalog) ---
                    // Sprawdzamy tylko, czy to poprawna ścieżka katalogu
                    bool isDir = Directory.Exists(path);

                    // Opcjonalnie: Jeśli katalog nie istnieje, ale ścieżka jest poprawna składniowo
                    // (bo program go utworzy), to też możemy uznać za OK.
                    // Ale dla bezpieczeństwa lepiej wymagać istniejącego:
                    box.BackColor = isDir ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightCoral;
                }
            }
            catch
            {
                box.BackColor = System.Drawing.Color.LightCoral;
            }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            threadCount = (int)thread_count.Value;

        }

        public void SetExecutionTime(long time)
        {
            time_exe.Text = $"{time} [ms]";
        }

        private void ASM_button_CheckedChanged(object sender, EventArgs e)
        {
            //powinna byc typu bool aby sprawdzac czy jest zaznaczona
            useAsm = ASM_button.Checked;
        }


        private void CPP_button_CheckedChanged(object sender, EventArgs e)
        {
            //powinna byc typu bool aby sprawdzac czy jest zaznaczona
            useCpp = CPP_button.Checked;
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void time_exe_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void time_Click(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
