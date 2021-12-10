![Prerelease UI](/Images/emu72.png) # lh5801_Emu

Sharp lh5801 Microprocessor Disassembler  

Written in C# using VS 2017

This lh5801 disassembler grew out of the lh5801 Emulator project. It is a 
work in progress and not user friendly at this time. It does understand 
PC-1500 $FF page vector jumps.

![Prerelease UI](/Images/Dis_Window.PNG)

Overly simplistic change log:
10/12/2021 - 1) Fixed bug where Carry Flag was carried into INC and DEC
             2) Added very simple threading to allow UI to break a running CPU.
