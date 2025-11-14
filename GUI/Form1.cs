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
        ParallelExecutor P_exe;
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
            //bierze sciezke do pliku dla progrmu glownego main w cs
            // pobiera ścieżkę do pliku wejściowego
            inputPath = textBox1.Text.Trim();

            if (!File.Exists(inputPath))
            {
                textBox1.BackColor = System.Drawing.Color.LightCoral;
            }
            else
            {
                textBox1.BackColor = System.Drawing.Color.White;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            outputPath = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(outputPath))
            {
                textBox2.BackColor = System.Drawing.Color.White; // Lub domyślny
                return;
            }

            try
            {
                // 1. Pobierz ścieżkę do samego FOLDERU
                string directoryPath = Path.GetDirectoryName(outputPath);

                // 2. Sprawdź, czy folder istnieje (lub czy ścieżka jest pusta/nieprawidłowa)
                if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                {
                    // Folder istnieje - wszystko jest OK
                    textBox2.BackColor = System.Drawing.Color.LightGreen;
                }
                else
                {
                    // Folder nie istnieje - zaznacz na czerwono
                    textBox2.BackColor = System.Drawing.Color.Salmon;
                }
            }
            catch (ArgumentException)
            {
                // Ścieżka zawiera nieprawidłowe znaki (np. "C:\test*?.txt")
                textBox2.BackColor = System.Drawing.Color.Salmon;
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
            // 1. SPRAWDZENIE KRYTYCZNE (WEJŚCIE)
            if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                MessageBox.Show(
                    "Plik wejściowy nie istnieje lub ścieżka jest nieprawidłowa.",
                    "Błąd krytyczny",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return; // PRZERWIJ! Nie idź dalej.
            }

            // 2. Sprawdzenie wyjścia (folderu)
            try
            {
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można utworzyć katalogu wyjściowego: {ex.Message}");
                return; // PRZERWIJ!
            }

            // 3. Jeśli wszystko jest OK - uruchom obliczenia
            MessageBox.Show("Rozpoczynam obliczenia...");
            //if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            //{
            //    MessageBox.Show("Podaj poprawną ścieżkę do pliku wejściowego.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
          
           
            if (useAsm) {
                P_exe.run_asm(inputPath, threadCount, outputPath);
            }
            if (useCpp) {
                P_exe.run_asm(inputPath, threadCount, outputPath);
            }
            else if (!useAsm && !useCpp)
            {
                MessageBox.Show("Wybierz bibliotekę (ASM lub C++).", "Brak wyboru", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            threadCount = (int)thread_count.Value;
         
        }

        public void SetExecutionTime(long milliseconds)
        {
            time_exe.Text = $"czas wykonania: {milliseconds} ms";
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

        
    }
}
