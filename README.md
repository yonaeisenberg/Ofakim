# Ofakim
Tool for fetching currencies rates from website and print them

The executable is : Ofakim2/bin/Release/Ofakim2.exe

Usage Ofakim2.exe <fetch | print>

Fetch = connect to the website and fetch the updated rates for the specified currencies, saves them to CSV file.
Print = print most updated rates for currencies from the file.

Exemple of running from command line (using Mono since I am on a mac):

$ cd Ofakim2/bin/Release/
$ mono Ofakim2.exe 
Wrong input. Usage : <fetch | print>
$ mono Ofakim2.exe print
Printing last updated rates...
No rate was found for EUR/USD, please fetch rates first.
No rate was found for EUR/JPY, please fetch rates first.
No rate was found for USD/ILS, please fetch rates first.
No rate was found for GBP/EUR, please fetch rates first.
$ mono Ofakim2.exe fetch
Fetching rates for currencies EUR/USD...
Fetching rates for currencies EUR/JPY...
Fetching rates for currencies GBP/EUR...
Fetching rates for currencies USD/ILS...
Rates were successfully fetched for currencies USD/ILS and saved to local file /Users/Jonas/Projects/Ofakim2/Ofakim2/bin/Release/xe-currency.csv.
Rates were successfully fetched for currencies EUR/JPY and saved to local file /Users/Jonas/Projects/Ofakim2/Ofakim2/bin/Release/xe-currency.csv.
Rates were successfully fetched for currencies GBP/EUR and saved to local file /Users/Jonas/Projects/Ofakim2/Ofakim2/bin/Release/xe-currency.csv.
Rates were successfully fetched for currencies EUR/USD and saved to local file /Users/Jonas/Projects/Ofakim2/Ofakim2/bin/Release/xe-currency.csv.
$ mono Ofakim2.exe print
Printing last updated rates...
GBP/EUR,1.14035,Feb 4 2021 13:33 UTC
EUR/JPY,126.245,5 Feb 2021 13:33 UTC
EUR/USD,1.19919,Feb 4 2021 13:33 UTC
USD/ILS,3.29522,4 Feb 2021 13:33 UTC
