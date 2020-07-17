#include <fstream>
#include <string>
#include <limits>
#include <iomanip>
#include <cctype>
#include <iostream>
#include <ctime>
#include <sstream>
#include <locale.h>
#include <cstring>
#pragma warning (disable: 4996)

void record_data();
void menu();
void getTime(int*, int*, int*);
void module(short xchoice);
void read_data();
void error_menu();
void menu_interface();
void search_data();
void delete_data();
void unpaid_debt();
void read_debt();
void clear_debt();
void view_cls_debt();
void view_list();
void view_debt_list();
void view_cleared_list();
void help();
void calcuBudget();
void backupSys();
bool backupReader();
void quit_program();

int main(int argc, char *argv[])
{
	menu();
	std::cout << "Press ENTER to continue...";
	std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
	return 0;
}

void getTime(int* day, int* month, int* year) {
	std::time_t t = std::time(NULL);   // get time now
	std::tm* now = std::localtime(&t);
	*month = (now->tm_mon + 1); *day = now->tm_mday; *year = (now->tm_year + 1900);
}
void menu() {
	system("CLS");
	menu_interface();
	short choice;
	do { //error checking function until user enters the right input
		for (;;) { //infinite loop until correct input is obtained
			std::cout << "\tEnter your choice: "; std::cin >> choice;
			if (std::cin.fail()) { //loop when user enters characters, strings, spaces and symbol
				std::cin.clear();
				std::cin.ignore(512, '\n');
				error_menu();
				continue;
			}

			break; //exit from loop if input is integer and within type 'short' range
		}
		if (choice < 17 && choice > 0) { //only executes when input is valid integer 1 to 7
			module(choice);
			continue;
		}
		if (!(choice < 17 && choice > 0)) { //else
			error_menu();
			continue;
		}
	} while (!(choice < 17 && choice > 0)); //loops if the user enters invalid integer
}
void error_menu() {
	system("CLS");
	menu_interface();
	std::cout << "\n\a";
	std::cout << "\t!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n";
	std::cout << "\t!!!                                            !!!\n";
	std::cerr << "\t!!!     Please enter the CORRECT selection     !!!\n";
	std::cout << "\t!!!                                            !!!\n";
	std::cout << "\t!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n";

}
void menu_interface() {
	int day, months, year;
	getTime(&day, &months, &year);
	char month[13][10] = { "xax", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
	std::cout << "\n\t--------Finance Tracking Application-----------mksoft copyright 2018\n";
	std::cout << "\t[][][][][][][][][][][][][][]||[][][][][][][][][][][][][][]||+=+=+=+=\\\n";
	std::cout << "\t[][][]                      ||                            ||=+=+=+=+=\\\n";
	std::cout << "\t[][]                        ||                            ||+=+=+=+=+=\\\n";
	std::cout << "\t[]         Main Menu        ||                            ||=+=+=+=+=+=\\\n";
	std::cout << "\t[][]                        ||                            ||+=+=+=+=+=+=\\\n";
	std::cout << "\t[][][]                      ||                            ||=+=+=+=+=+=+=\\\n";
	std::cout << "\t[][][][][][][][][][][][][][]||[][][][][][][][][][][][][][]||+=+=+=+=+=+=+=\\   " << month[months] << " " << day << ", " << year << "\n";
	std::cout << "\n\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txx       1. Insert object        xx xx      6. Insert debt info      xx xx      11. View raw record      xx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxsxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txx        2. View records        xx xx       7. View debt info       xx xx       12. View raw debt       xx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txx       3. Search records       xx xx         8. Clear debt         xx xx     13. View raw cleared      xx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txx       4. Delete records       xx xx       9. View clearlist       xx xx                               xx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txx      5. Calculate budget      xx xx      10. Rules and Help       xx xx       15. Quit program        xx\n";
	std::cout << "\txx                               xx xx                               xx xx                               xx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\n";
	std::cout << "\txxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx xxxxxxxxxxxxxxxxxxxxxxxxxx v1.4.2f3\n";
}
void module(short xchoice) {
	switch (xchoice) {
	case 1:
		system("CLS");
		record_data();
		break;
	case 2:
		system("CLS");
		read_data();
		break;
	case 3:
		system("CLS");
		search_data();
		break;
	case 4:
		system("CLS");
		delete_data();
		break;
	case 5:
		system("CLS");
		calcuBudget();
		break;
	case 6:
		system("CLS");
		unpaid_debt();
		break;
	case 7:
		system("CLS");
		read_debt();
		break;
	case 8:
		system("CLS");
		clear_debt();
		break;
	case 9:
		system("CLS");
		view_cls_debt();
		break;
	case 10:
		system("CLS");
		help();
		break;
	case 11:
		system("CLS");
		view_list();
		break;
	case 12:
		system("CLS");
		view_debt_list();
		break;
	case 13:
		system("CLS");
		view_cleared_list();
		break;
	case 14:
		system("CLS");
		std::cout << "{{{>_<}}}\n";
		std::cout << "  >> "; std::cin.ignore(512, '\n'); std::cin.get(); menu();
		break;
	case 15:
		system("CLS");
		quit_program();
		break;
	case 16:
		system("CLS");
		backupSys();
		break;
	}
}
void record_data() {
	int day, month, year; double price, today_price = 0;
	std::string item; char choice;
	std::cout << "\tIs it on today? 'Y' for yes\n";
	std::cout << "\t  >> ";
	std::cin >> choice;
	if (choice == 'Y' || choice == 'y') {
		getTime(&day, &month, &year);
	}
	else {
		std::cout << "\t==========================\n";
		std::cout << "\tPlease enter date.\n";
		std::cout << "\tInsert '-1' in any part of date to cancel insertion. .\n";
		std::cout << "\t==========================\n";
		std::cout << "\t==========================\n";
		std::cout << "\tDay: ";
		std::cin >> day;
		if (day <= 0) {
			std::cout << "\n";
			std::cout << "\tInput cancelation protocol triggered, hit enter to return to main menu. . .\n";
			std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();
		}
		std::cout << "\t==========================\n";
		std::cout << "\tMonth: ";
		std::cin >> month;
		if (month <= 0) {
			std::cout << "\n";
			std::cout << "\tInput cancelation protocol triggered, hit enter to return to main menu. . .\n";
			std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();
		}
		std::cout << "\t==========================\n";
		std::cout << "\tYear: ";
		std::cin >> year;
		if (year <= 0) {
			std::cout << "\n";
			std::cout << "\tInput cancelation protocol triggered, hit enter to return to main menu. . .\n";
			std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();
		}
		std::cout << "\t==========================\n";
		std::cout << "\t==========================\n";
		std::cout << "\n";
	}
	std::ofstream outFile("records.txt", std::ios::app);
	do {
		std::cin.ignore();
		outFile << day << '-' << month << '-' << year << ',';
		std::cout << "\t[][][][][][][][][][][][][][][][][][][]\n";
		std::cout << "\t[][] Item: "; getline(std::cin, item);
		std::cout << "\t--------------------------------------\n";
		std::cout << "\t[][] Price: RM"; std::cin >> price;
		std::cout << "\t[][][][][][][][][][][][][][][][][][][]\n";
		outFile << item << '*' << price << std::endl;
		std::cin.ignore();
		std::cout << "\n\tTo add another item, enter 'Y'\n";
		std::cout << "\t  >> "; std::cin >> choice; today_price += price;
	} while (choice == 'Y' || choice == 'y');
	outFile.close();
	std::cout << "\tTotal expenditure: RM" << std::fixed << std::setprecision(2) << today_price << " based on today's input.\n";
	std::cout << "\tEnd of input. . Hit enter to continue. . .";
	std::cin.ignore();
	std::cin.get();
	system("CLS");
	menu();
}
void read_data() {
	double price, total_price = 0; unsigned int index = 0;
	std::string item, date;
	std::cout << std::left;
	std::cout << "History\n\n";
	std::cout << "\t==================================================================================\n";
	std::cout << "\t|         |                     |                               |                |\n";
	std::cout << "\t|  Index  |   Date              |   Item                        |  Price (RM)    |\n";
	std::cout << "\t|         |                     |                               |                |\n";
	std::cout << "\t$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$\n";
	std::ifstream inFile("records.txt");
	while (getline(inFile, date, ','), getline(inFile, item, '*'), inFile >> price) {
		index++; inFile.ignore();
		std::cout << "\t$$   " << std::setw(9) << index << std::setw(22) << date << std::setw(31) << item << std::setw(10) << std::fixed << std::setprecision(2) << price << "   $$\n";
		total_price += price;
	}
	inFile.close();
	std::cout << "\t==================================================================================\n";
	std::cout << "\t                                                        Total :  RM" << std::fixed << std::setprecision(2) << total_price << std::endl;
	std::cout << "\t==================================================================================\n";
	std::cout << "\tHit enter to return . . .\n\t  >> ";
	std::cin.ignore();
	std::cin.get();
	system("CLS");
	menu();
}
void search_data() {
	char item[50], date[50], input[50], inputcpy[50]; double price, total_search_price = 0;
	std::cout << "\t<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-\n";
	std::cout << "\t<->                                                     <->-\n";
	std::cout << "\t<->   Enter date, name or price to search for item      <->-\n";
	std::cout << "\t<->                                                     <->-\n";
	std::cout << "\t<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-<->-\n\n";
	std::cout << "\tInput: "; std::cin.ignore(); std::cin.getline(input, 50);
	system("CLS");
	std::cout << std::left;
	for (int i = 0; i < 50; i++) {
		inputcpy[i] = toupper(input[i]);
	}
	std::cout << "\tSearch results for \'" << inputcpy << "\'\n\n";
	std::cout << "\t==================================================================================\n";
	std::cout << "\t|         |                     |                               |                |\n";
	std::cout << "\t|  Index  |   Date              |   Item                        |  Price (RM)    |\n";
	std::cout << "\t|         |                     |                               |                |\n";
	std::cout << "\t$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$\n";
	int index = 0, indicator = 0;
	std::ifstream inFile("records.txt");
	while (inFile.getline(date, 50, ','), inFile.getline(item, 50, '*'), inFile >> price >> std::ws) {
		if (strcmp(date, input) == 0 || strcmp(item, input) == 0 || strstr(item, input) || strstr(date, input)) {
			index++; indicator = 1;
			std::cout << "\t$$   " << std::setw(9) << index << std::setw(22) << date << std::setw(31) << item << std::setw(10) << std::fixed << std::setprecision(2) << price << "   $$\n";
			total_search_price += price;
		}
	}
	if (indicator == 0) {
		std::cout << "\n\n\a";
		std::cout << "\t&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&\n";
		std::cout << "\t&&&                                           &&&\n";
		std::cout << "\t&&&     Sorry, NO relevant results found!     &&&\n";
		std::cout << "\t&&&                                           &&&\n";
		std::cout << "\t&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&\n";
	}
	inFile.close();
	std::cout << "\t==================================================================================\n";
	std::cout << "\t                                                        Total :  RM" << std::fixed << std::setprecision(2) << total_search_price << std::endl;
	std::cout << "\t==================================================================================\n\n";
	std::cout << "\tHit return key to continue. . .\n";
	std::cout << "\t  >> "; std::cin.get();
	menu();

}
void delete_data() { /*Add list*/
	char date[50], item[50], input[50], dateinput[50], choice; double price;
	std::cin.ignore();
	std::cout << "\t[]============================================[]\n";
	std::cout << "\t||   Enter date and name of item to delete    ||\n";
	std::cout << "\t[]============================================[]\n";
	std::cout << "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n";
	std::cout << "\t%%%   Enter exact date (CTRL+Z to cancel)   %%%\n";
	std::cout << "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n";
	std::cout << "\t  >> "; std::cin.getline(dateinput, 50); if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
	std::cout << "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n";
	std::cout << "\t%%%   Enter item (CTRL+Z to cancel)         %%%\n";
	std::cout << "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n";
	std::cout << "\t  >> "; std::cin.getline(input, 50); if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
	std::cout << "\n\t>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n";
	std::cout << "\t<<<                                                                                                 <<<\n";
	std::cout << "\t>>>        Are you sure about this? Enter 'Y' to proceed and others to return to main menu. . .     >>>\n";
	std::cout << "\t<<<                                                                                                 <<<\n";
	std::cout << "\t>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n";
	std::cout << "\t  >> "; std::cin >> choice;
	std::ifstream inFile("records.txt");
	std::ofstream outFile("temp.txt");
	if (choice == 'Y' || choice == 'y') {
		while (inFile.getline(date, 50, ','), inFile.getline(item, 50, '*'), inFile >> price >> std::ws) {
			if (strcmp(date, dateinput) != 0 || strcmp(item, input) != 0) {
				outFile << date << ',' << item << '*' << price << std::endl;
			}
		}
	}
	else {
		inFile.close(); outFile.close(); std::cin.ignore(512, '\n'); menu();
	}
	inFile.close();
	outFile.close();
	remove("records.txt");
	rename("temp.txt", "records.txt");
	std::cout << "\t0%      25%     50%     75%     100%\n"; //loading animation
	std::cout << "\t==================================\n";
	std::string s = "\t||||||||||||||||||||||||||||||||||\n";
	for (int j = 0; j < s.size(); j++)
	{
		for (int i = 0; i <= 99999999; i++);
		std::cout << s[j];
	}
	std::cout << "\n\tRecord erased successfully! Hit enter to continue. . .\n";
	std::cout << "\t  >> "; std::cin.ignore();
	std::cin.get(); menu();
}
void quit_program() {
	std::cout << "\n\a";
	std::cout << "\t|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|\n";
	std::cout << "\t|-|                                                                               |-|\n";
	std::cout << "\t|-|     Are you sure? Enter 'Y' for yes and other to return to main menu. . .     |-|\n";
	std::cout << "\t|-|                                                                               |-|\n";
	std::cout << "\t|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|\n";
	std::cout << "\n\t  >> "; char choice; std::cin >> choice;
	if (choice == 'Y' || choice == 'y') {
		exit(100);
	}
	else {
		menu();
	}
}
void unpaid_debt() {
	std::string person, item, comment; double amount; int day, month, year; char choice;
	std::cout << "\tIs it on today? 'Y' for yes\n";
	std::cout << "\t  >> ";
	std::cin >> choice;
	if (choice == 'Y' || choice == 'y') {
		getTime(&day, &month, &year);
	}
	else {
		std::cout << "\n\tActive list: \n";
		std::cout << "\t----Date-------------------------\n";
		std::cout << "\tDay    | "; std::cin >> day; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\tMonth  | "; std::cin >> month; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\tYear   | "; std::cin >> year; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\t---------------------------------\n\n";
	}
	std::ofstream outFile("debt.txt", std::ios::app);
	do {
		std::cin.ignore();
		outFile << day << '-' << month << '-' << year << ',';
		std::cout << "\tc c c c c c c c c c c c c c c c c c c c c c c c c c\n";
		std::cout << "\t    Enter creditor's name: "; getline(std::cin, person);
		std::cout << "\tc c c c c c c c c c c c c c c c c c c c c c c c c c\n";
		std::cout << "\t    Amount: RM"; std::cin >> amount;
		std::cout << "\tc c c c c c c c c c c c c c c c c c c c c c c c c c\n";
		std::cout << "\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n";
		std::cout << "\t~~~ Comment~[Not more than a few words]~~~~~~~~~~~~\n";
		std::cout << "\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n";
		std::cout << "\t /> "; std::cin.ignore(); getline(std::cin, comment);
		std::cout << "\n";
		outFile << person << '*' << comment << '|' << amount << std::endl;
		std::cout << "\n\tAnymore creditors? 'Y' for yes\n";
		std::cout << "\t  >> ";
		std::cin >> choice;
	} while (choice == 'Y' || choice == 'y');
	outFile.close();
	std::cout << "\tThe data have successfully saved. . .\n";
	std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();
}
void read_debt() {
	std::string date, person, comment; double amount, total_debt = 0; unsigned int index = 0;
	std::cin.ignore();
	std::cout << std::left;
	std::cout << "\n\t#&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&#\n";
	std::cout << "\t# Index  #  Date           #  Creditor            #  Amount(RM)   #\n";
	std::cout << "\t#&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&#\n";
	std::ifstream inFile("debt.txt");
	while (getline(inFile, date, ','), getline(inFile, person, '*'), getline(inFile, comment, '|'), inFile >> amount >> std::ws) {
		index++;
		std::cout << "\t# " << std::setw(10) << index << std::setw(18) << date << std::setw(23) << person << std::setw(13) << std::fixed << std::setprecision(2) << amount << "#" << std::endl;
		std::cout << "\t===================================================================\n";
		std::cout << "\t> Comment: " << std::setw(55) << comment << "#\n";
		std::cout << "\t#&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&#\n";
		total_debt += amount;
	}
	inFile.close();
	std::cout << "\tTotal debt: RM" << total_debt << std::endl;
	std::cout << "\t  >> "; std::cin.get(); menu();

}
void clear_debt() {
	char date[50], person[50], comment[50], inputPerson[50], choice; int day_paid, month_paid, year_paid; double amount, inputAmount;
	std::cout << "\tIs it on today? 'Y' for yes\n";
	std::cout << "\t  >> ";
	std::cin >> choice;
	if (choice == 'Y' || choice == 'y') {
		getTime(&day_paid, &month_paid, &year_paid);
	}
	else {
		std::cout << "\n\tooooooooooooooooooooooooooooo\n";
		std::cout << "\tooo  Enter payday's date  ooo\n";
		std::cout << "\tooooooooooooooooooooooooooooo\n\tCTRL+Z to quit\n";
		std::cout << "\n\t==============================\n";
		std::cout << "\tDay     >> "; std::cin >> day_paid; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\tMonth   >> "; std::cin >> month_paid; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\tYear    >> "; std::cin >> year_paid; if (!std::cin) { std::cin.ignore(512, '\n'); menu(); }
		std::cout << "\t==============================\n";
	}
	std::cin.ignore(512, '\n');
	std::cout << "\tEnter Creditor's name : "; std::cin.getline(inputPerson, 50);
	std::cout << "\tEnter amount of debt  : RM"; std::cin >> inputAmount; std::cin.ignore();
	std::cout << "\tAre you sure you have cleared the debt? 'Y' for yes\n";
	std::cout << "\t  >> ";
	std::cin >> choice;
	std::ifstream inFile("debt.txt");
	std::ofstream outFile("tempor.txt"), oF2("cleared.txt", std::ios::app);
	if (choice == 'Y' || choice == 'y') {
		while (inFile.getline(date, 50, ','), inFile.getline(person, 50, '*'), inFile.getline(comment, 50, '|'), inFile >> amount >> std::ws) {
			if (strcmp(person, inputPerson) == 0 && amount == inputAmount) {
				oF2 << date << ',' << day_paid << '-' << month_paid << '-' << year_paid << ',' << person << '*' << amount << std::endl;
			}
			else {
				outFile << date << ',' << person << '*' << comment << '|' << amount << std::endl;
			}
		}
	}
	else {
		inFile.close(); outFile.close(); oF2.close(); std::cin.ignore(512, '\n'); menu();
	}
	inFile.close(); outFile.close(); oF2.close();
	remove("debt.txt");
	rename("tempor.txt", "debt.txt");
	std::cout << "\tAll changes had been successfully made\n";
	std::cout << "\t  >> ";
	std::cin.ignore(); std::cin.get(); menu();
}
void view_cls_debt() {
	std::ifstream inFile("cleared.txt");
	std::string date_made, date_cleared, person; double amount; unsigned int index = 0;
	std::cout << std::endl << std::left;
	std::cout << "\tCleared debts\n";
	std::cout << "\t:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::;:::::::::::::::::::::::::::\n";
	std::cout << "\t:::         ::                 ::                 ::                  ::                  :::\n";
	std::cout << "\t:::  Index  ::  Date of debt   ::   Date cleared  ::   Name           ::    Amount (RM)   :::\n";
	std::cout << "\t:::         ::                 ::                 ::                  ::                  :::\n";
	std::cout << "\t:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n";
	while (getline(inFile, date_made, ','), getline(inFile, date_cleared, ','), getline(inFile, person, '*'), inFile >> amount >> std::ws) {
		index++;
		std::cout << "\t:::   " << std::setw(10) << index << std::setw(20) << date_made << std::setw(19) << date_cleared << std::setw(22) << person << std::setw(13) << amount << ":::\n";

	}
	inFile.close();
	std::cout << "\t:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n";
	std::cout << "\n\tEnd of File. . . Hit ENTER to continue. . .\n";
	std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();
}
void view_list() {
	std::cout << "Viewing raw list data file. . . Handle with care. . \n";
	system("records.txt");
	menu();

}
void view_debt_list() {
	std::cout << "Viewing raw debt list data file. . . Handle with care. . \n";
	system("debt.txt");
	menu();
}
void view_cleared_list() {
	std::cout << "Viewing raw cleared debt data file. . . Handle with care. . \n";
	system("cleared.txt");
	menu();
}
void help() {
	std::cout << std::endl;
	std::cout << "\t------------------------------------------------------------------------------------------\n";
	std::cout << "\t                                   Rules and Regulation\n";
	std::cout << "\t------------------------------------------------------------------------------------------\n";
	std::cout << "\t  > The name should not exceed 30 characters, if it is long, try to make it shorter.\n";
	std::cout << "\t  > The view raw data function can be used when program displaying visual bugs due to\n";
	std::cout << "\t    corrupted input and make suitable changes to it.\n";
	std::cout << "\t  > Try to follow whats been prompted for input, the program isn't robust enough to\n";
	std::cout << "\t    handle eccentric insertion.\n";
	std::cout << "\t  > Do not simply move the text file generated by the system into other directories.\n";
	std::cout << "\t  > Make sure the text file is kept with the executable file.\n";
	std::cout << "\t  > This program is case-sensitive, meaning \"Coke\" is not equal to \"coke\".\n";
	std::cout << "\t------------------------------------------------------------------------------------------\n";
	std::cout << "\t                                           Help\n";
	std::cout << "\t------------------------------------------------------------------------------------------\n";
	std::cout << "\t  > This application was developed based on Windoes 10, using in Windoes 7 will cause\n";
	std::cout << "\t    visual distortion.\n";
	std::cout << "\t  > Delete function is not advisible to use unless\n";
	std::cout << "\t  > If you wish to view your monthly expense, for example May 2018, in the Search bar,\n";
	std::cout << "\t    type 5-2018 and the whole records will be displayed with your total expenditure that\n";
	std::cout << "\t    particular month.\n";
	std::cout << "\t  > If the data failed to update or save, please restart the program and try it again,\n";
	std::cout << "\t    everything should be working as it should.\n"; setlocale(LC_ALL, "german");
	std::cout << "\t  > Contact Björn Tetsuya (MK): HP (+60)10-9419641 for feedback and recommendation.\n\n\n";
	std::cout << "\t  >> "; std::cin.ignore(); std::cin.get(); menu();

}

void calcuBudget() {
	std::cout << "\n\tEnter this month's budget: RM"; double budget;
	for (;;) { //infinite loop until correct input is obtained
		std::cin >> budget;
		if (std::cin.fail()) { //loop when user enters characters, strings, spaces and symbol
			std::cin.clear();
			std::cin.ignore(512, '\n');
			std::cout << "\n\tzz..Please enter the correct money amount : RM";
			continue;
		}
		break; //exit from loop if input is integer and within type 'short' range
	}
	int day, month, year, dayinmonth;
	getTime(&day, &month, &year);
	if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
		dayinmonth = 31;
	}
	else if (month == 2) {
		dayinmonth = 28;
	}
	else {
		dayinmonth = 30;
	}
	//processing date
	std::ostringstream date;
	date << month << "-" << year;
	std::string s = date.str();
	int n = s.size();
	char* date_array = new char[n + 1];
	strcpy(date_array, s.c_str());
	//processing date
	std::ifstream inFile("records.txt");
	char infdate[30], item[50]; double cost, cost_used = 0;
	while (inFile.getline(infdate, 30, ','), inFile.getline(item, 50, '*'), inFile >> cost >> std::ws) {
		if (strstr(infdate, date_array)) {
			cost_used += cost;
		}
	}
	inFile.close();
	double money_left = budget - cost_used;
	double recommend_ave = money_left / (dayinmonth - day);
	std::cout << "\n\n\tRecommend using RM" << recommend_ave << " or below from today till the end of month.\n\t  >> ";
	delete[] date_array;
	std::cin.ignore(512, '\n'); std::cin.get(); menu();
}

void backupSys() {
	getchar(); int choice; int date, month, year; bool ship;
	getTime(&date, &month, &year);
	std::cout << "\n\tBackup file: Overview.\n";
	ship = backupReader();
	std::cout << "\n\n\tBackup file: Select the action you wish to perform.\n\n";
	std::cout << "\t1. Create backup point.\n";
	std::cout << "\t2. Backup with existing files\n";
	std::cout << "\t3. Return\n";
	if (ship) {
		do {
			std::cout << "\n\t  >> "; std::cin >> choice;
		} while (choice < 1 || choice > 3);
	}
	else {
		do {
			std::cout << "\n\t  >> "; std::cin >> choice;
			if (choice == 2)
				std::cout << "\tYou have not created backups yet!\n";
		} while (choice != 1 && choice != 3);
	}

	if (choice == 1) {
		std::ifstream inFile("records.txt");
		std::ofstream outFile("records-backup.txt");
		outFile << inFile.rdbuf();
		inFile.close(); outFile.close();

		std::ifstream inF1("debt.txt");
		std::ofstream ouF1("debt-backup.txt");
		ouF1 << inF1.rdbuf();
		inF1.close(); ouF1.close();

		std::ifstream inF2("cleared.txt");
		std::ofstream ouF2("cleared-backup.txt");
		ouF2 << inF2.rdbuf();
		inF2.close(); ouF2.close();

		std::ofstream Data("Backup.DATA");
		Data << date << '-' << month << '-' << year << std::endl;
		Data.close();

		std::cout << "\tBackup created successfully.\n\t  >> ";
		std::cin.ignore(512, '/n').get(); menu();
	}
	else if (choice == 2) {
		remove("records.txt"); rename("records-backup.txt", "records.txt");
		remove("debt.txt"); rename("debt-backup.txt", "debt.txt");
		remove("cleared.txt"); rename("cleared-backup.txt", "cleared.txt");
		std::cout << "\tBackup successful\n\t >> ";
		getchar(); menu();
	}
	else {
		menu();
	}
}

bool backupReader() {
	//for cleared
	std::string date_backup, date_made, date_cleared, person; double amount;
	// for debt
	std::string dates, creditor, comments; double debt;
	// for records
	std::string date_of_purchase, item; double price;
	bool flag = true;
	int count = 0, count2 = 0, count3 = 0;
	std::ifstream Data("Backup.DATA");
	if (!Data) {
		std::cerr << "\tBackup file not created!\n"; flag = false;


	}
	else {
		Data >> date_backup >> std::ws;
		std::ifstream inFile("cleared-backup.txt");
		while (getline(inFile, date_made, ','), getline(inFile, date_cleared, ','), getline(inFile, person, '*'), inFile >> amount >> std::ws) {
			count3++;
		}
		inFile.close();
		std::ifstream inF2("debt-backup.txt");
		while (getline(inF2, dates, ','), getline(inF2, creditor, '*'), getline(inF2, comments, '|'), inFile >> debt >> std::ws) {
			count2++;
		}
		inF2.close();
		std::ifstream inF1("records-backup.txt");
		while (getline(inF1, date_of_purchase, ','), getline(inF1, item, '*'), inFile >> price >> std::ws) {
			count++;
		}
		inF1.close();
		std::cout << "\tBackup created on: " << date_backup << std::endl;
		std::cout << "\tItem list(\"records-backup.txt\") --- " << count << " item(s).\n";
		std::cout << "\tCreditor list(\"debt-backup.txt\") --- " << count2 << " item(s).\n";
		std::cout << "\tCleared debt list(\"cleared-backup.txt\") --- " << count3 << " item(s).\n";
	}
	Data.close();
	return flag;
}
