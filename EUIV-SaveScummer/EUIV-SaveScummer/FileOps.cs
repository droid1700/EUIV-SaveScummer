using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EUIV_SaveScummer
{
	internal class FileOps
	{
		private byte[] savedHash;
		private string savedGameFilePath;
		private string gameFileName;
		private string dirToSaveScumTo;

		public FileOps(string filePath)
		{
			SetFileHash(filePath);
			savedGameFilePath = filePath;
			gameFileName = filePath.Split('\\')[filePath.Split('\\').Length - 1].Substring(0, filePath.Split('\\')[filePath.Split('\\').Length - 1].Length - 4);
		}

		/// <summary>
		/// Sets the savedHash based on the passed filepath
		/// </summary>
		/// <param name="filepath"></param>
		private void SetFileHash(string filepath)
		{
			try
			{
				HashAlgorithm sha1 = HashAlgorithm.Create();
				using(FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
				{
					savedHash = sha1.ComputeHash(stream);
				}
			}
			catch { }
		}

		/// <summary>
		/// Check if the file state has changed
		/// </summary>
		/// <returns></returns>
		public bool FileChanged()
		{
			if(String.IsNullOrEmpty(savedGameFilePath))
			{
				return false; //Game file doesn't exist
			}

			byte[] hashCheck;
			HashAlgorithm sha1 = HashAlgorithm.Create();

			using(FileStream stream = new FileStream(savedGameFilePath, FileMode.Open, FileAccess.Read))
			{
				hashCheck = sha1.ComputeHash(stream);
			}

			bool returnValue = !hashCheck.SequenceEqual(savedHash);

			savedHash = hashCheck;

			return returnValue;
		}

		/// <summary>
		/// Set the save scum directory
		/// </summary>
		/// <param name="dirPath"></param>
		public void SetSaveDir(string dirPath)
		{
			dirToSaveScumTo = dirPath;
		}

		/// <summary>
		/// Save Scum
		/// </summary>
		public void SaveScum()
		{
			if(String.IsNullOrEmpty(savedGameFilePath) || String.IsNullOrEmpty(dirToSaveScumTo))
			{
				return;
			}

			DateTime time = DateTime.Now;
			string path = dirToSaveScumTo + "\\" + gameFileName + "_" + time.Year + time.Month.ToString().PadLeft(2, '0') + time.Day.ToString().PadLeft(2, '0') + time.Hour.ToString().PadLeft(2, '0') + time.Minute.ToString().PadLeft(2, '0') + ".eu4";

			File.Copy(savedGameFilePath, path);
		}

	}
}
