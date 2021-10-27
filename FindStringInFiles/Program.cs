using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace FindStringInFiles {
	class Program {

		static void PrintUsage() {
			Console.WriteLine("Usage:\nEnter the phrase you're searching for, followed by the location you want to search in. " +
				"The program will ask if you want to search subdirectories. Answer with Y or N only. In addition, it will ask" +
				"if you wish for a case sensitive search. The answer is the same. You will be shown a list of" +
				" all files containing that string.");
		}

		/// <summary>
		/// Recursively search for the item in sub-directories
		/// </summary>
		/// <param name="needle">The phrase to search for</param>
		/// <param name="haystack">The top-level directory to search in</param>
		/// <param name="subdirect">True if searching in sub-directories, otherwise false</param>
		/// <param name="insensitive">True if the results are case-insensitive, otherwise false</param>
		/// <returns>A list of directories containing the phrase</returns>
		static List<string> FindFilesInDirectory(string needle, string haystack, bool subdirect, bool insensitive) {
			List<string> toReturn = new List<string>();
			Console.WriteLine($"Testing: {haystack}");

			if (subdirect) {
				// Check all subdirectories
				foreach (string directory in Directory.GetDirectories(haystack)) {
					// TODO : Check if has read access
					toReturn.AddRange(FindFilesInDirectory(needle, directory, subdirect, insensitive));
				}
			}

			

			foreach (string file in Directory.GetFiles(haystack)) {
				// Search each file in the directory
				using (var reader = new StreamReader(file)) {
					string input = reader.ReadToEnd();
					if (insensitive) { input = input.ToLower(); }
					if (input.Contains(needle)) {
						toReturn.Add(file);
					}
				}
			}

			return toReturn;
		}

		static void Main(string[] args) {
			PrintUsage();

			Console.WriteLine("====================");

			// Get necessary inputs
			Console.WriteLine("Enter the string to search for: ");
			string needle = Console.ReadLine();

			Console.WriteLine("Enter the directory you want to search in: ");
			string haystack = Console.ReadLine();

			Console.WriteLine("Would you like to search through sub-directories too (Y/N)? ");
			string answer = Console.ReadLine();

			while (answer != "Y" && answer != "N") {
				Console.WriteLine("Invalid option! Would you like to search through sub-directories too (Y/N)? ");
				answer = Console.ReadLine();
			}

			Console.WriteLine("Would you like a case-insensitive search (Y/N)? For example, \"Hello\" would be the same as \"hello\".");
			string sensitive = Console.ReadLine();
			while (sensitive != "Y" && sensitive != "N") {
				Console.WriteLine("Invalid option! Would you like a case-insensitive search? (Y/N)? ");
				sensitive = Console.ReadLine();
			}

			Console.WriteLine("====================");
			Console.WriteLine("Searching through directories... Please be patient.");

			// Alter with sensitivity if necessary
			if (sensitive == "Y") { needle = needle.ToLower(); }

			// Find and print items
			List<string> files = FindFilesInDirectory(needle, haystack, answer == "Y", sensitive == "Y");
			files.ForEach(x => Console.WriteLine(x));
			
			Console.WriteLine("====================");

			// File I/O if desired
			Console.WriteLine($"Would you like to to save the {files.Count} files to a file (Y/N)?");
			answer = Console.ReadLine();
			while (answer != "Y" && answer != "N") {
				Console.WriteLine("Invalid option! Would you like to save this to a file (Y/N)? ");
				answer = Console.ReadLine();
			}

			if (answer == "Y") {
				using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "files.txt"))) {
					files.ForEach(x => writer.WriteLine(x));
				}
			}

			Console.WriteLine("====================");
			Console.WriteLine("Thank you for using the file searcher!");
		}
	}
}
