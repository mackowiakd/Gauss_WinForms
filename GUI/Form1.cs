using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Gauss_elim.threading;




namespace GUI
{
    public partial class Form1 : Form
    {
        private string inputPath = "";
        private string outputPath = "GUI_res.txt";
        private bool useAsm = false;
        private bool useCpp = false;
        private int threadCount = 1;
        
        public Form1()
        {
            InitializeComponent();
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
            inputPath = textBox1.Text.Trim();
            string extension = "";

            try
            {
                // Pobierz samo rozszerzenie pliku, np. ".txt"
                extension = Path.GetExtension(inputPath);
            }
            catch (ArgumentException)
            {
                // Ścieżka zawiera nieprawidłowe znaki
                textBox1.BackColor = System.Drawing.Color.LightCoral;
                return;
            }

            // Sprawdź obie rzeczy:
            // 1. Czy plik fizycznie istnieje?
            // 2. Czy rozszerzenie to ".txt" (ignorując wielkość liter)?
            if (File.Exists(inputPath) && extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                // Jest OK
                textBox1.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                // Plik nie istnieje LUB ma złe rozszerzenie
                textBox1.BackColor = System.Drawing.Color.LightCoral;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            outputPath = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(outputPath))
            {
                textBox2.BackColor = System.Drawing.Color.White;
                return;
            }

            try
            {
                string directoryPath = Path.GetDirectoryName(outputPath);
                string extension = Path.GetExtension(outputPath); // <-- NOWA LINIA

                // Sprawdź folder:
                // (Musi istnieć LUB być pusty - dla zapisu w folderze roboczym)
                bool isDirOk = string.IsNullOrEmpty(directoryPath) || Directory.Exists(directoryPath);

                // Sprawdź rozszerzenie:
                bool isExtOk = extension.Equals(".txt", StringComparison.OrdinalIgnoreCase); // <-- NOWA LINIA

                if (isDirOk && isExtOk)
                {
                    // Folder jest poprawny ORAZ rozszerzenie to .txt
                    textBox2.BackColor = System.Drawing.Color.LightGreen;
                }
                else
                {
                    // Folder jest niepoprawny LUB rozszerzenie jest złe
                    textBox2.BackColor = System.Drawing.Color.LightCoral;
                }
            }
            catch (ArgumentException)
            {
                // Ścieżka zawiera nieprawidłowe znaki (np. "C:\test*?.txt")
                textBox2.BackColor = System.Drawing.Color.LightCoral;
            }
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

        private void Run_button_Click(object sender, EventArgs e)
        {
            ParallelExecutor P_exe= new ParallelExecutor();
            // === BRAMKA 1: Sprawdzenie rozszerzeń .txt ===
            // (Sprawdzamy to na początku, zanim zaczniemy sprawdzać resztę)
            try
            {
                if (!Path.GetExtension(inputPath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Plik wejściowy musi mieć rozszerzenie .txt", "Błędna ścieżka", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // PRZERWIJ
                }

                if (!Path.GetExtension(outputPath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Plik wyjściowy musi mieć rozszerzenie .txt", "Błędna ścieżka", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // PRZERWIJ
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Ścieżki zawierają niedozwolone znaki.", "Błędna ścieżka", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // PRZERWIJ
            }

            // === BRAMKA 2: Sprawdzenie CZY PLIK WEJŚCIOWY ISTNIEJE ===
            if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                MessageBox.Show(
                    "Plik wejściowy nie istnieje lub ścieżka jest nieprawidłowa.",
                    "Błąd krytyczny",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return; // PRZERWIJ
            }

            // === BRAMKA 3: Sprawdzenie CZY FOLDER WYJŚCIOWY JEST POPRAWNY ===
            try
            {
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    // Próbujemy utworzyć folder, jeśli go nie ma
                    Directory.CreateDirectory(outputDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można utworzyć katalogu wyjściowego: {ex.Message}", "Błąd wyjścia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // PRZERWIJ
            }



            if (useAsm && useCpp)
            {
                MessageBox.Show("Wybierz tylko jedną bibliotekę (ASM lub C++).", "Błąd wyboru", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!useAsm && !useCpp)
            {
                MessageBox.Show("Wybierz bibliotekę (ASM lub C++).", "Brak wyboru", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Jeśli wszystko jest OK - uruchom obliczenia
            MessageBox.Show("Rozpoczynam obliczenia...");

            if (useAsm)
            {
                SetExecutionTime(P_exe.run_asm(inputPath, threadCount, outputPath));
            }
            else if (useCpp) // <-- POPRAWKA 1
            {
                // POPRAWKA 2: Wywołaj run_cpp!
                SetExecutionTime(P_exe.run_cpp(inputPath, threadCount, outputPath));
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
    }
}
