using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace LicenceKeyGenerator
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the Click event of the btnGenerateLicenceKeys control to generate the specified number of GUIDs.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnGenerateLicenceKeys_Click(object sender, EventArgs e)
		{
			// Variables to hold the generated GUID Key and Hash
			string key = string.Empty;
			string hash = string.Empty;
			string salt = txtSalt.Text.Trim();

			// MD5 Provider used to calculate the MD5 Hash the generated Keys
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			// Clear out the existing keys
			listLicenceKeys.Items.Clear();
			listHashedLicenceKeyValues.Items.Clear();

			// Generate the specified number of GUIDs.
			int numberOfKeysToGenerate = (int)numericNumberOfKeysToGenerate.Value;
			for (int count = 0; count < numberOfKeysToGenerate; count++)
			{
				// Generate the new Key and calculate its Hash
				key = System.Guid.NewGuid().ToString();
				hash = getMd5Hash(salt + key);

				// Add the Key and Hash to the appropriate listboxes
				listLicenceKeys.Items.Add(key);
				listHashedLicenceKeyValues.Items.Add(hash);
			}
		}

		/// <summary>
		/// Handles the Click event of the btnSelectAllLicenceKeys control to Select All Licence Keys.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSelectAllLicenceKeys_Click(object sender, EventArgs e)
		{
			// Select all of the Licence Keys
			for (int index = 0; index < listLicenceKeys.Items.Count; index++)
				listLicenceKeys.SetSelected(index, true);
		}

		/// <summary>
		/// Handles the Click event of the btnSelectAllHashedKeyValues control to Select All Hashed Key Values.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSelectAllHashedKeyValues_Click(object sender, EventArgs e)
		{
			// Select all of the Hashed Licence Key Values
			for (int index = 0; index < listHashedLicenceKeyValues.Items.Count; index++)
				listHashedLicenceKeyValues.SetSelected(index, true);
		}

		/// <summary>
		/// Handles the Click event of the btnCopySelectedLicenceKeysToClipboard control to Copy the selected Hash Key Values to the Clipboard.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnCopySelectedLicenceKeysToClipboard_Click(object sender, EventArgs e)
		{
			// Build the string to hold all of the listbox contents
			StringBuilder buffer = new StringBuilder();
			for (int i = 0; i < listLicenceKeys.SelectedItems.Count; i++)
			{
				buffer.Append(listLicenceKeys.SelectedItems[i].ToString());
				buffer.Append("\n");
			}

			// Copy the string to the clipboard
			if (!string.IsNullOrWhiteSpace(buffer.ToString()))
				Clipboard.SetText(buffer.ToString());
		}

		/// <summary>
		/// Handles the Click event of the btnCopySelectedHashedKeyValuesToClipboard control to Copy the selected Licence Keys to the Clipboard.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnCopySelectedHashedKeyValuesToClipboard_Click(object sender, EventArgs e)
		{
			// Build the string to hold all of the listbox contents
			StringBuilder buffer = new StringBuilder();
			for (int i = 0; i < listHashedLicenceKeyValues.SelectedItems.Count; i++)
			{
				buffer.Append(listHashedLicenceKeyValues.SelectedItems[i].ToString());
				buffer.Append("\n");
			}

			// Copy the string to the clipboard
			if (!string.IsNullOrWhiteSpace(buffer.ToString()))
				Clipboard.SetText(buffer.ToString());
		}


		// MD5 functions below taken from: http://msdn.microsoft.com/en-us/library/system.security.cryptography.md5cryptoserviceprovider.aspx

		// Hash an input string and return the hash as
		// a 32 character hexadecimal string.
		static string getMd5Hash(string input)
		{
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}

		// Verify a hash against a string.
		static bool verifyMd5Hash(string input, string hash)
		{
			// Hash the input.
			string hashOfInput = getMd5Hash(input);

			// Create a StringComparer an compare the hashes.
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;

			if (0 == comparer.Compare(hashOfInput, hash))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	}
}
