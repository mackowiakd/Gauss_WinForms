# Parallel Gauss Elimination Solver 
> 🎓 **Academic Project**: This application was developed as the final project for the *Assembly Languages* course during the 5th semester of Computer Science at the **Silesian University of Technology** (Politechnika Śląska).

A high-performance desktop application for solving systems of linear equations using the Gauss Elimination method. This project demonstrates the integration of high-level UI, multithreading, and low-level hardware optimizations (SIMD).

##  Key Features
* **Multi-language Integration:** The core algorithms are written in **C++** and **x64 Assembly (MASM)**, compiled as dynamic libraries (`.dll`), and called from the **C#** GUI using P/Invoke.
* **SIMD Vectorization:** The Assembly implementation utilizes Advanced Vector Extensions (**AVX**) and `YMM` registers to process 8 single-precision floating-point numbers concurrently, significantly speeding up row operations.
* **Multithreading:** The Task Parallel Library (TPL `Parallel.For`) is used in C# to parallelize the elimination steps across multiple CPU cores.
* **Performance Comparison:** The WinForms GUI allows users to run and compare execution times between the C++ and Assembly implementations.

## 🛠️ Technologies & Tools
* **C# / .NET:** WinForms, Task Parallel Library (TPL), Interop/P/Invoke
* **C++:** Unmanaged DLL generation
* **x64 Assembly:** MASM, AVX Instructions
* **IDE:** Visual Studio 2022

## ⚙️ Architecture
1. **`GUI` (C# WinForms):** Handles user input, file parsing, and displays execution time.
2. **`Gauss_elim` (C# Class Library):** Manages data formatting, threading (`ParallelExecutor`), and serves as a bridge to native libraries.
3. **`Gauss_c++` (C++ DLL):** Native implementation of the Gauss elimination and back-substitution.
4. **`Gauss_asm` (Assembly DLL):** Highly optimized, vectorized implementation of the row elimination logic and dot product calculations.

## 📸 Screenshots
<img width="616" height="529" alt="image" src="https://github.com/user-attachments/assets/ff6edcd3-ec8d-45f5-a2c8-9fb134767499" />

Fig 1: User Interface. The C# WinForms frontend allowing dynamic configuration of the execution context (native C++ vs. x64 AVX Assembly) and thread allocation.
<img width="407" height="441" alt="image" src="https://github.com/user-attachments/assets/0329adc1-7c6b-4c83-b3b7-2cefdc52532f" />
<img width="453" height="324" alt="image" src="https://github.com/user-attachments/assets/64e94f9b-08c9-407d-831e-5fb0a773f2ea" />
Fig 2, 3:Error handling: If an incorrect file format is specified or a DLL library is missing, the application displays an appropriate error message (MessageBox), preventing the program from closing unexpectedly.

<img width="945" height="639" alt="image" src="https://github.com/user-attachments/assets/ee48bfd9-cef3-49dc-8caa-3597caea34b5" />
Fig 4: Performance Scaling (N=1250). Log-log plot comparing execution times of C++ and AVX Assembly implementations across different compiler optimization levels. The flattening curves indicate the hardware hitting the memory bandwidth limit (Memory Bound).

6. ## 📊 Performance Analysis & Conclusions
Extensive benchmarking revealed several interesting hardware-level phenomena:

* **Compute vs. Memory Bound:** For smaller matrices, the algorithm scales almost perfectly with the thread count (Compute Bound). However, for highly optimized code (ASM or Release mode) on large matrices ($N=1250$), the performance curve flattens. The CPU becomes starved for data, hitting the RAM bandwidth limit (**Memory Bound**).
* **Compiler Optimizations (O2) vs. Raw Assembly:** While the unoptimized C++ code is severely outperformed by the custom Assembly implementation (up to 4x slower), enabling `/O2` flags in the MSVC compiler flips the result. The C++ compiler utilizes Fused Multiply-Add (**FMA**) instructions, which were not manually implemented in the ASM code, resulting in higher throughput and slightly better floating-point precision.
* **Overhead:** Spawning a large number of threads (e.g., 32-64) for small matrices (N=50) introduces noticeable OS synchronization overhead, actually degrading performance.

* ##  How to Run
1. Clone the repository.
2. Open the solution (`Gauss_WinForms.sln`) in Visual Studio.
3. Ensure the build architecture is set to **x64** (required for the Assembly DLL and AVX instructions).
4. Build the solution and run the `GUI` project.
5. Use the provided test datasets (e.g., `matrix1250x1250.txt`) to test performance.

