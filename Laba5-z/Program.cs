using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laba5_z
{

  class Program
  {
    public const string ram = @"RAM.txt";
    public const string swap = @"SWAP.txt";
    public const int memory = 65535;
    public const int limit = 524288;
    public Random random = new Random();
    public static bool write;
    public static string[] text;
    public static string ramIndex = "";
    public static string swapIndex = "";

    public static string lastpRam;
    public static string lastpSwap;
    public static string lastcRam;
    public static string lastcSwap;
    public static string lastaRam;
    public static string lastaSwap;

    public static int SWAP_SIZE = 0;

    public static readonly List<string> _processes = new List<string>
        {
            "Задача1",
            "Задача2",
            "Задача3"
        };

    public static readonly List<string> _programData = new List<string>
        {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h"
        };

    public static void Initialization()
    {
      File.WriteAllText(ram, String.Empty);
      File.WriteAllText(swap, String.Empty);


      using (FileStream fstream = new FileStream(ram, FileMode.OpenOrCreate))
      {
        for (int i = 0; i <= memory; i++)
        {
          var array = Encoding.Default.GetBytes($"[{new string('0', memory.ToString().Length - i.ToString().Length) + i}]\n");
          fstream.Write(array, 0, array.Length);
        }
      }

    }

    public static void Program_Loading_To_SWAP(int writeKB)
    {

      List<string> arr = new List<string>();
      List<char> arrChar = new List<char>();
      byte[] array;

      if (SWAP_SIZE >= limit)
        throw new Exception("SWAP_FILE_OVERFLOW");

      for (int k = 0; k < writeKB; k += 4)
      {
        for (int i = k; i < k + 4; i++)
        {
          arr.Add($"[{new string('0', limit.ToString().Length - SWAP_SIZE.ToString().Length) + SWAP_SIZE}]");
          SWAP_SIZE++;
        }

        arr[k] += _processes[RandNumber(0, _processes.Count - 1)];
        lastpSwap = arr[k].Remove(0, 8);
        arr[k + 1] += _programData[RandNumber(0, _programData.Count - 1)];
        lastcSwap = arr[k + 1].Remove(0, 8);
        arr[k + 2] += $"%{RandNumber(10, 99)} , %{RandNumber(10, 99)}";
        swapIndex = arr[k + 3];
        arr[k + 3] += $"%{RandNumber(10, 99)} , %{RandNumber(10, 99)}";
        lastaSwap = arr[k + 2].Remove(0, 8) + " ; " + arr[k + 3].Remove(0, 8);
      }

      for (int i = 0; i < arr.Count; i++)
        arr[i] += '\n';

      foreach (var str in arr)
        foreach (var chr in str)
          arrChar.Add(chr);

      array = Encoding.Default.GetBytes(arrChar.ToArray());

      using (FileStream fstream = new FileStream(swap, FileMode.Append))
      {
        fstream.Write(array, 0, array.Length);
      }
      write = false;

    }

    public static void Program_Loading_To_RAM(int writeKB)
    {

      write = true;

      if (ramIndex.Length != 0 && Convert.ToInt32(ramIndex.Replace("[", "").Replace("]", "")) >= memory) //swapping
      {
        Program_Loading_To_SWAP(writeKB);
        return;
      }


      byte[] array;
      List<char> arr = new List<char>();
      int fromWrite = 0, toWrite = 0;

      using (FileStream fstream = new FileStream(ram, FileMode.Open))
      {
        array = new byte[fstream.Length];
        fstream.Read(array, 0, array.Length);
      }

      var a = Encoding.Default.GetString(array);
      a = a.Remove(a.LastIndexOf('\n'), 1);
      text = a.Split('\n');


      for (int i = 0; i < text.Length; i++)
      {
        if (text[i].Remove(0, 7) == string.Empty)
        {
          fromWrite = i;
          break;
        }
      }

      for (int k = fromWrite; k < fromWrite + writeKB; k += 4)
      {
        if (k + 1 >= text.Length || k + 2 >= text.Length || k + 3 >= text.Length)
          break;
        text[k] += _processes[RandNumber(0, _processes.Count - 1)];
        lastpRam = text[k].Remove(0, 7);
        text[k + 1] += _programData[RandNumber(0, _programData.Count - 1)];
        lastcRam = text[k + 1].Remove(0, 7);
        text[k + 2] += $"%{RandNumber(10, 99)} , %{RandNumber(10, 99)}";
        ramIndex = text[k + 3];
        text[k + 3] += $"%{RandNumber(10, 99)} , %{RandNumber(10, 99)}";
        lastaRam = text[k + 2].Remove(0, 7) + " ; " + text[k + 3].Remove(0, 7);

      }

      for (int i = 0; i < text.Length; i++)
        text[i] += '\n';


      File.WriteAllText(ram, string.Empty);

      foreach (var str in text)
        foreach (var chr in str)
          arr.Add(chr);

      array = Encoding.Default.GetBytes(arr.ToArray());
      using (FileStream fstream = new FileStream(ram, FileMode.Open))
      {
        fstream.Write(array, 0, array.Length);
      }

      write = false;
    }


    public static int RandNumber(int Low, int High)
    {
      return new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber)).Next(Low, High + 1);
    }



    public static bool y = false;
    static void Main(string[] args)
    {
      Console.CursorVisible = false;
      KeyListener();
      Program.Initialization();

      while (!y)
      {
        Update();
        Thread.Sleep(20);
      }
    }

    public static void Update()
    {
      Console.ForegroundColor = ConsoleColor.White;
      Clear(1, 1, Console.BufferWidth, 25);
      Console.SetCursorPosition(1, 1);
      Console.Write("Нажмите пробел, чтобы добавить задачу в ");
      Console.Write(Program.ramIndex == "[65535]" ? "SWAP_FILE: " : "RAM: ");

      if (!Program.write)
      {
        Console.Write("Ожидание");
      }
      else
      {
        Console.Write("Запись...");
      }
      Console.SetCursorPosition(1, 3);
      Console.Write($"Последняя команда: ");
      Console.Write(Program.lastcRam);
      Console.Write(" Аргументы: ");
      Console.Write(Program.lastaRam);
      Console.Write(" В ");
      Console.Write(Program.lastpRam);
      Console.SetCursorPosition(1, 7);
      Console.Write($"Размер swap= {Program.SWAP_SIZE}");
      Console.SetCursorPosition(1, 9);
      Console.Write($"Последняя команда, записанная в swap: ");
      Console.Write(Program.lastcSwap);
      Console.Write(" Аргументы: ");
      Console.Write(Program.lastaSwap);
      Console.Write(" В ");
      Console.Write(Program.lastpSwap);
    }

    public static async Task KeyListener()
    {
      await Task.Run(() =>
      {
        while (true)
        {
          switch (Console.ReadKey().KeyChar)
          {
            case (char)ConsoleKey.Backspace:
              y = true;
              break;
            case '\\':

              break;
            case (char)ConsoleKey.Enter:

              break;
            case (char)ConsoleKey.D0:
              Environment.Exit(0);
              break;
            case ' ':
              try
              {
                Program.Program_Loading_To_RAM(RandNumber(0, 65535));
              }
              catch (Exception e)
              {
                Console.WriteLine(e);
                throw;
              }
              break;
          }
        }

      });
    }

    public static void Clear(int x, int y, int width, int height)
    {
      int curTop = Console.CursorTop;
      int curLeft = Console.CursorLeft;
      for (; height > 0;)
      {
        Console.SetCursorPosition(x, y + --height);
        Console.Write(new string(' ', width));
      }
      Console.SetCursorPosition(curLeft, curTop);
    }

  }
}