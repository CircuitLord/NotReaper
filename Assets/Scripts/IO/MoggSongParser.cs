using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotReaper.IO {
	public static class MoggSongParser {

		//static string METADATA_START = "0xd000";

		//Extracts the DTA as raw text from the CONfile specified in "filepath"
		//Like everything else in this script, it's probably a terrible way of
		//doing it but it functions well enough.
		/*

		void read_metadata(string filepath) {
			var file = File.ReadAllBytes(filepath);


			f.seek(METADATA_START)

		metadata = []


		byte = f.read(1)


		while byte != '\x00':
            metadata.append(byte)
			byte = f.read(1)


			return ''.join(metadata)
		}
		*/

		//This will parse in the DTA file as a single text string.
		//raw_string is the text of the DTA file.
		//tree at first should be an empty list that you can access later
		//i is the tree level (start with 0)
		//# Ex: parse_metadata(dta_text, tree, 0)
		public static List<string> parse_metadata(string raw_text) {
			int i = 0;
			string s = "";
			//bool read_active = false;
			List<string> tree = new List<string>();

			while (raw_text[i] != '(') {
				i++;
			}
			i++;

			//Do this until the matching close paren is found
			while (raw_text[i] != ')') {

				//Elements can have single or double quotation marks
				if (raw_text[i] == '\'') {
					i++;
					while (raw_text[i] != '\'') {
						s = s + raw_text[i];
						i++;
					}

					tree.Add(s);
					s = "";

				} else if (raw_text[i] == '\"') {
					i++;
					while (raw_text[i] != '\"') {
						s = s + raw_text[i];
						i++;
					}

					tree.Add(s);
					s = "";

					//Hopefully we don't need this.
				} else if (raw_text[i] == ';') {
					i++;

					//while (raw_text[i])

					//Internal subsection, start parsing recursively
				} else if (raw_text[i] == '(') {
					//var subtree 
				}

				i++;
			}

			return tree;

		}
		/*

    # Do this until the matching close paren is found
    while raw_text[i] != ')':


        # Internal subsection, start parsing recursively
        elif raw_text[i] == '(':

			subtree = []
		i = parse_metadata(raw_text, subtree, i)

			tree.append(subtree)
# Grabs the extra shit
		elif not raw_text[i].isspace() and not read_active:
            s = s + raw_text[i]
			read_active = True

		elif not raw_text[i].isspace() and read_active:

			s = s + raw_text[i]
		elif raw_text[i].isspace and read_active:
            tree.append(s)
			s = ''

			read_active = False
		i = i + 1

    if read_active:
        tree.append(s)

    return i



	

	*/

	}
}