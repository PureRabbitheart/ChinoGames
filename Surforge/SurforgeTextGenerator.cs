using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public class SurforgeTextGenerator : MonoBehaviour {
	
	public bool lettersAndDigits;
	public bool collocations;


	string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

	// first word from array + one other random from array
	// "1" for random number
	// "A 1" - random letter and number, etc
	List<string>[] collocationsContent = new List<string>[] {
		new List<string>() { "active", "coil", "protection", "control", "gear", "pneumatic", "systems", "unit", "facility", 
							 "gateway", "components", "environment" },

		new List<string>() { "area", "1", "A 1"},
		new List<string>() { "AI", "mainframe" },
		new List<string>() { "air", "supply", "block" },
		new List<string>() { "alpha", "1"},
		new List<string>() { "armory" },
		new List<string>() { "ammo", "supply", "storage" },
		new List<string>() { "artillery" },
		new List<string>() { "access", "denied" },
		new List<string>() { "alkaline", "condition", "battery" },
		new List<string>() { "block", "1", "A 1" },
		new List<string>() { "beta", "1" },
		new List<string>() { "battery", "charger", "block" },
		new List<string>() { "high", "capacity", "security", "frequency", "priority", "interference" },
		new List<string>() { "cryonics" },
		new List<string>() { "cryo", "facility" },
		new List<string>() { "core", "access", "module" },
		new List<string>() { "control", "center", "module", "unit", "system" },
		new List<string>() { "command", "area" },
		new List<string>() { "central", "mainframe", "storage" },
		new List<string>() { "container", "1", "A 1" },
		new List<string>() { "class", "1", "A 1" },
		new List<string>() { "compressed", "air" },
		new List<string>() { "data", "center", "port", "storage", "cable" },
		new List<string>() { "DNA", "storage" },
		new List<string>() { "dock", "area" },
		new List<string>() { "engine", "bay", "compartment" },
		new List<string>() { "engineering", "area", "gateway" },
		new List<string>() { "energy", "source" },
		new List<string>() { "elevator" },
		new List<string>() { "electronic", "components" },
		new List<string>() { "equipment", "storage" },
		new List<string>() { "emergency", "exit", "power" },
		new List<string>() { "free", "access" },
		new List<string>() { "freon", "supply", "reservoir" },
		new List<string>() { "fluid", "supply", "reservoir" },
		new List<string>() { "ground", "area" },
		new List<string>() { "gate", "1" },
		new List<string>() { "gateway", "1" },
		new List<string>() { "gamma", "1" },
		new List<string>() { "hydraulics" },
		new List<string>() { "heavy", "gear", "equipment" },
		new List<string>() { "ID", "required", "A 123456" },
		new List<string>() { "identification", "required" },
		new List<string>() { "lab", "1", "A 1" },
		new List<string>() { "laboratory", "1", "A 1" },
		new List<string>() { "loading bay", "A", "1", "A 1" },
		new List<string>() { "medical", "area", "unit" },
		new List<string>() { "military", "area", "unit", "storage" },
		new List<string>() { "mainframe" },
		new List<string>() { "no", "photography", "access" },
		new List<string>() { "open", "area" },
		new List<string>() { "oil", "supply", "reservoir", "filter" },
		new List<string>() { "optical", "cable", "port" },
		new List<string>() { "personnel", "area", "access only" },
		new List<string>() { "protected", "area" },
		new List<string>() { "power", "supply", "plant", "unit", "station", "facility", "source", "cable" },
		new List<string>() { "pressure" },
		new List<string>() { "pneumatics" },
		new List<string>() { "residential", "area" },
		new List<string>() { "refrigerant", "block", "storage", "filter", "system" },
		new List<string>() { "remote", "control" },
		new List<string>() { "sewer", "pipe", "gate" },
		new List<string>() { "section", "1", "A 1" },
		new List<string>() { "security", "area", "post", "control", "gateway" },
		new List<string>() { "storage", "1", "A 1" },
		new List<string>() { "terminal", "1", "A 1" },
		new List<string>() { "unit", "1", "A 1" },
		new List<string>() { "water", "supply", "reservoir" },
		new List<string>() { "zero", "access", "security" }

	};
	

	public void Generate() {
		string result = "";


		if (lettersAndDigits && collocations) {
			int random = Random.Range(0,1);
			if (random == 0) GenerateLettersAndDigits();
			else GenerateCollocation();
		}
		else {
			if (lettersAndDigits) result = GenerateLettersAndDigits();
			if (collocations) result = GenerateCollocation();
		}


		TextMesh textMesh = (TextMesh)GetComponent<TextMesh>();
		if (textMesh) {
			textMesh.text = result;
		}
	}
	

	string GenerateLettersAndDigits() {
		string result = "";

		int random = Random.Range(0,11);
		if (random == 0) result = GenerateCharacterAndDigitA();
		if (random == 1) result = GenerateCharacterAndDigitB();
		if (random == 2) result = GenerateCharacterAndDigitC();
		if (random == 3) result = GenerateCharacterAndDigitM();
		if (random == 4) result = GenerateCharacterAndDigitE();
		if (random == 5) result = GenerateCharacterAndDigitF();
		if (random == 6) result = GenerateCharacterAndDigitG();
		if (random == 7) result = GenerateCharacterAndDigitN();
		if (random == 8) result = GenerateCharacterAndDigitI();
		if (random == 9) result = GenerateCharacterAndDigitJ();
		if (random == 10) result = GenerateCharacterAndDigitK();
		if (random == 11) result = GenerateCharacterAndDigitL();

		return result;
	}

	string GenerateCollocation() {
		string result = "";

		string wordFirst = "";
		string wordSecond = "";
		
		int randomFirst = Random.Range(0, collocationsContent.Length);
		int randomSecond = Random.Range(1, collocationsContent[randomFirst].Count);
		
		wordFirst = collocationsContent[randomFirst][0];
		if (collocationsContent[randomFirst].Count > 1) {
			wordSecond = collocationsContent[randomFirst][randomSecond];
		}
		if (wordSecond == "1") {
			wordSecond = GenerateRandomDigit();
		}
		
		if (wordSecond == "A 1") {
			int random = Random.Range(0,3);
			if (random == 0) wordSecond = GenerateCharacterAndDigitA();
			if (random == 1) wordSecond = GenerateCharacterAndDigitB();
			if (random == 2) wordSecond = GenerateCharacterAndDigitC();
			if (random == 3) wordSecond = GenerateCharacterAndDigitD();
		}
		
		if (wordSecond == "A 123456") {
			int random = Random.Range(0,3);
			if (random == 0) wordSecond = GenerateCharacterAndDigitE();
			if (random == 1) wordSecond = GenerateCharacterAndDigitF();
			if (random == 2) wordSecond = GenerateCharacterAndDigitG();
			if (random == 3) wordSecond = GenerateCharacterAndDigitH();
		}
		
		if (collocationsContent[randomFirst].Count > 1) {
			result = wordFirst + " " + wordSecond;
		}
		else {
			result = wordFirst;
		}
		return result;
	}

	string GenerateRandomDigit() {
		string result = "";
		int random = Random.Range(0,5);
		if (random == 0) result = Random.Range(0, 100).ToString();
		if (random == 1) result = Random.Range(0, 100).ToString() + "0";
		if (random == 2) result = Random.Range(0, 10).ToString();
		if (random == 3) result = "0" + Random.Range(0, 10).ToString();
		if (random == 4) result = "0" + Random.Range(0, 100).ToString();
		if (random == 5) result = Random.Range(0, 10).ToString() + "0" + Random.Range(0, 10).ToString();
		
		return result;
	}


	string GenerateCharacterAndDigitA() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + GenerateRandomDigit();
	}

	string GenerateCharacterAndDigitB() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + c.ToString();
	}

	string GenerateCharacterAndDigitC() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + "-" + GenerateRandomDigit();
	}

	string GenerateCharacterAndDigitD() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + "/" + c.ToString();
	}

	string GenerateCharacterAndDigitE() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit();
	}
	string GenerateCharacterAndDigitF() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + GenerateRandomDigit() + c.ToString();
	}

	string GenerateCharacterAndDigitG() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + "-" + GenerateRandomDigit() + GenerateRandomDigit();
	}

	string GenerateCharacterAndDigitH() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + GenerateRandomDigit() + "/" + c.ToString();
	}

	string GenerateCharacterAndDigitI() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit();
	}
	string GenerateCharacterAndDigitJ() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit() + c.ToString();
	}
	
	string GenerateCharacterAndDigitK() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + "-" + GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit();
	}
	
	string GenerateCharacterAndDigitL() {
		char c = st[Random.Range(0, st.Length)];
		return GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit() + " " + c.ToString();
	}

	string GenerateCharacterAndDigitM() {
		char c = st[Random.Range(0, st.Length)];
		return c.ToString() + " " + GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit();
	}

	string GenerateCharacterAndDigitN() {
		char c = st[Random.Range(0, st.Length)];
		char c2 = st[Random.Range(0, st.Length)];
		return c.ToString() + c2.ToString() + " " + GenerateRandomDigit() + GenerateRandomDigit() + GenerateRandomDigit();
	}



}


